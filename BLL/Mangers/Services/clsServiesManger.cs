using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entites.Services;
using DAL.Repo.Services;

namespace BLL.Mangers.Services
{
    public class clsServicesManger
    {
        private readonly clsServicesRepo _servicesRepo;

        public clsServicesManger()
        {
            _servicesRepo = new clsServicesRepo();
        }

        #region GetAll

        public async Task<List<clsService>> GetAllServicesAsync()
        {
            try
            {
                return await _servicesRepo.GetAllServicesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الخدمات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب خدمة برقم المعرف

        public async Task<clsService?> GetServiceByIdAsync(long serId)
        {
            if (serId <= 0)
            {
                throw new ArgumentException("معرف الخدمة غير صالح.");
            }

            try
            {
                return await _servicesRepo.GetServiceByIdAsync(serId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الخدمة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. إضافة خدمة جديدة

        public async Task<long> AddServiceAsync(clsService service)
        {
            ValidateServiceData(service);

            try
            {
                return await _servicesRepo.AddServiceAsync(service);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الخدمة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. تعديل بيانات خدمة

        public async Task<bool> UpdateServiceAsync(clsService service)
        {
            if (service.SER_ID <= 0)
            {
                throw new ArgumentException("معرف الخدمة المطلوب تعديلها غير صحيح.");
            }

            ValidateServiceData(service);

            try
            {
                return await _servicesRepo.UpdateServiceAsync(service);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الخدمة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. حذف خدمة

        public async Task<bool> DeleteServiceAsync(long serId)
        {
            if (serId <= 0)
            {
                throw new ArgumentException("معرف الخدمة المراد حذفها غير صالح.");
            }

            try
            {
                return await _servicesRepo.DeleteServiceAsync(serId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الخدمة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateServiceData(clsService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "بيانات الخدمة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(service.SER_NAME))
            {
                throw new Exception("اسم الخدمة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (service.SER_NAME.Length > 200)
            {
                throw new Exception("اسم الخدمة يجب ألا يتجاوز 200 حرف.");
            }
        }

        #endregion

        #region 6. جلب الخدمات بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من الخدمات بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsService>> GetServicesPagedAsync(int pageNumber, int rowsPerPage, string? serviceName = null)
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
                return await _servicesRepo.GetServicesPagedAsync(pageNumber, rowsPerPage, serviceName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الخدمات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب إجمالي عدد الخدمات مع البحث

        /// <summary>
        /// جلب إجمالي عدد الخدمات المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalServicesCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _servicesRepo.GetTotalServicesCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الخدمات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. توليد كود تلقائي للخدمة الجديدة

        public async Task<long> GetNewSer_CodeAsync()
        {
            try
            {
                long maxId = await _servicesRepo.GetMaxServiceIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الخدمة الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
