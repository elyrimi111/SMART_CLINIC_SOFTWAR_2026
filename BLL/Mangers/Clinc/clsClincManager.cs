using Core.Entites.Clinc;
using DAL.Repo.Clinc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Clinc
{
    public class clsClincManager
    {
        private readonly clsClincRepo _clincRepo;

        public clsClincManager()
        {
            _clincRepo = new clsClincRepo();
        }

        #region 1. جلب كافة العيادات

        public async Task<List<clsClinc>> GetAllClinicsAsync()
        {
            try
            {
                return await _clincRepo.GetAllClinicsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة العيادات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب عيادة برقم المعرف

        public async Task<clsClinc?> GetClinicByIdAsync(long? cliId)
        {
            if (cliId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _clincRepo.GetClinicByIdAsync(cliId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. إضافة عيادة جديدة

        public async Task<long> AddClinicAsync(clsClinc clinic)
        {
            ValidateClinicData(clinic);

            try
            {
                return await _clincRepo.AddClinicAsync(clinic);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. تعديل بيانات عيادة

        public async Task<bool> UpdateClinicAsync(clsClinc clinic)
        {
            if (clinic.CLI_ID <= 0)
            {
                throw new ArgumentException("معرف العيادة المطلوب تعديلها غير صحيح.");
            }

            ValidateClinicData(clinic);

            try
            {
                return await _clincRepo.UpdateClinicAsync(clinic);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. حذف عيادة

        public async Task<bool> DeleteClinicAsync(long cliId)
        {
            if (cliId <= 0)
            {
                throw new ArgumentException("معرف العيادة المراد حذفها غير صالح.");
            }

            try
            {
                return await _clincRepo.DeleteClinicAsync(cliId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateClinicData(clsClinc clinic)
        {
            if (clinic == null)
            {
                throw new ArgumentNullException(nameof(clinic), "بيانات العيادة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(clinic.CLI_NAME))
            {
                throw new Exception("اسم العيادة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (clinic.CLI_NAME.Length > 200)
            {
                throw new Exception("اسم العيادة يجب ألا يتجاوز 200 حرف.");
            }

            if (!string.IsNullOrEmpty(clinic.CLI_LOC) && clinic.CLI_LOC.Length > 250)
            {
                throw new Exception("موقع العيادة يجب ألا يتجاوز 250 حرف.");
            }
        }

        #endregion

        #region 6. جلب العيادات بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من العيادات بناءً على رقم الصفحة وعدد العناصر واسم العيادة
        /// </summary>
        public async Task<List<clsClinc>> GetClinicsPagedAsync(int pageNumber, int rowsPerPage, string? clinicName = null)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("رقم الصفحة يجب أن يكون أكبر من صفر.", nameof(pageNumber));
            }

            if (rowsPerPage <= 0)
            {
                throw new ArgumentException("عدد الصفوف في الصفحة يجب أن يكون أكبر من صفر.", nameof(rowsPerPage));
            }

            try
            {
                return await _clincRepo.GetClinicsPagedAsync(pageNumber, rowsPerPage, clinicName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة العيادات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب إجمالي عدد العيادات مع البحث

        /// <summary>
        /// جلب إجمالي عدد العيادات المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalClinicsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _clincRepo.GetTotalClinicsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد العيادات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. توليد كود تلقائي للعيادة الجديدة

        public async Task<long> GetNewCli_CodeAsync()
        {
            try
            {
                long maxId = await _clincRepo.GetNextServiceCodeAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود العيادة الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
