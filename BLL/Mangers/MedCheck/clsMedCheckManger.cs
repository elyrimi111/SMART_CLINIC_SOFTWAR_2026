using Core.Entites.Med_Check;
using DAL.Repo.MedCheck;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.MedCheck
{
    public class clsMedCheckManger
    {
        private readonly clsMedCheckRepo _medCheckRepo;

        public clsMedCheckManger()
        {
            _medCheckRepo = new clsMedCheckRepo();
        }

        #region GetAll

        public async Task<List<clsMed_Check>> GetAllMedChecksAsync()
        {
            try
            {
                return await _medCheckRepo.GetAllMedChecksAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الفحوصات الطبية: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Id

        public async Task<clsMed_Check?> GetMedCheckByIdAsync(long? medCheckId)
        {
            if (medCheckId <= 0)
            {
                throw new ArgumentException("معرف الفحص الطبي غير صالح.");
            }

            try
            {
                return await _medCheckRepo.GetMedCheckByIdAsync(medCheckId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الفحص الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New MedCheck

        public async Task<long> AddMedCheckAsync(clsMed_Check medCheck)
        {
            ValidateMedCheckData(medCheck);

            try
            {
                return await _medCheckRepo.AddMedCheckAsync(medCheck);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الفحص الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update MedCheck

        public async Task<bool> UpdateMedCheckAsync(clsMed_Check medCheck)
        {
            if (medCheck.MEDCHECK_ID <= 0)
            {
                throw new ArgumentException("معرف الفحص الطبي المطلوب تعديله غير صحيح.");
            }

            ValidateMedCheckData(medCheck);

            try
            {
                return await _medCheckRepo.UpdateMedCheckAsync(medCheck);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الفحص الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete MedCheck

        public async Task<bool> DeleteMedCheckAsync(long medCheckId)
        {
            if (medCheckId <= 0)
            {
                throw new ArgumentException("معرف الفحص الطبي المراد حذفه غير صالح.");
            }

            try
            {
                return await _medCheckRepo.DeleteMedCheckAsync(medCheckId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الفحص الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void ValidateMedCheckData(clsMed_Check medCheck)
        {
            if (medCheck == null)
            {
                throw new ArgumentNullException(nameof(medCheck), "بيانات الفحص الطبي فارغة.");
            }

            if (string.IsNullOrWhiteSpace(medCheck.MEDCHECK_NAME))
            {
                throw new Exception("اسم الفحص الطبي مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (medCheck.MEDCHECK_NAME.Length > 200)
            {
                throw new Exception("اسم الفحص الطبي يجب ألا يتجاوز 200 حرف.");
            }
        }

        #endregion

        #region Get MedCheck Async

        /// <summary>
        /// جلب صفحة محددة من الفحوصات الطبية بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsMed_Check>> GetMedChecksPagedAsync(int pageNumber, int rowsPerPage, string? medCheckName = null)
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
                return await _medCheckRepo.GetMedChecksPagedAsync(pageNumber, rowsPerPage, medCheckName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الفحوصات الطبية بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Total MedChecks 

        public async Task<int> GetTotalMedChecksCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _medCheckRepo.GetTotalMedChecksCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الفحوصات الطبية: " + ex.Message, ex);
            }
        }

        #endregion

        #region Genrate New MedCheck Code

        public async Task<long> GetNewMedCheck_CodeAsync()
        {
            try
            {
                long maxId = await _medCheckRepo.GetMaxMedCheckIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الفحص الطبي الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
