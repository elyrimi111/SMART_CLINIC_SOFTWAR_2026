using Core.Entites.Vist;
using DAL.Repo.Vists;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Vists
{
    public class clsVistManger
    {
        private readonly clsVistRepo _vistRepo;

        public clsVistManger()
        {
            _vistRepo = new clsVistRepo();
        }

        #region 1. جلب كافة الزيارات

        public async Task<List<clsVist>> GetAllVisitsAsync()
        {
            try
            {
                return await _vistRepo.GetAllVisitsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الزيارات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب زيارة برقم المعرف

        public async Task<clsVist?> GetVisitByIdAsync(long? visitId)
        {
            if (visitId <= 0)
            {
                throw new ArgumentException("معرف الزيارة غير صالح.");
            }

            try
            {
                return await _vistRepo.GetVisitByIdAsync(visitId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الزيارة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. إضافة زيارة جديدة

        public async Task<long> AddVisitAsync(clsVist visit)
        {
            ValidateVisitData(visit);

            try
            {
                return await _vistRepo.AddVisitAsync(visit);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الزيارة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. تعديل بيانات زيارة

        public async Task<bool> UpdateVisitAsync(clsVist visit)
        {
            if (visit.VIS_ID <= 0)
            {
                throw new ArgumentException("معرف الزيارة المطلوب تعديلها غير صحيح.");
            }

            ValidateVisitData(visit);

            try
            {
                return await _vistRepo.UpdateVisitAsync(visit);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الزيارة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. حذف زيارة

        public async Task<bool> DeleteVisitAsync(long visitId)
        {
            if (visitId <= 0)
            {
                throw new ArgumentException("معرف الزيارة المراد حذفها غير صالح.");
            }

            try
            {
                return await _vistRepo.DeleteVisitAsync(visitId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الزيارة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateVisitData(clsVist visit)
        {
            if (visit == null)
            {
                throw new ArgumentNullException(nameof(visit), "بيانات الزيارة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(visit.VIS_NAME))
            {
                throw new Exception("عنوان/اسم الزيارة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (visit.VIS_NAME.Length > 200)
            {
                throw new Exception("عنوان/اسم الزيارة يجب ألا يتجاوز 200 حرف.");
            }

            if (visit.CUST_ID <= 0)
            {
                throw new Exception("معرف العميل/المريض غير صالح.");
            }

            if (visit.CLI_ID <= 0)
            {
                throw new Exception("معرف العيادة غير صالح.");
            }

            if (visit.DOC_ID <= 0)
            {
                throw new Exception("معرف الطبيب غير صالح.");
            }

            // التحقق من الحقول المالية الجديدة ووسيلة الدفع
            if (visit.VIS_PRICE < 0)
            {
                throw new Exception("سعر الزيارة لا يمكن أن يكون بقيمة سالبة.");
            }

            if (visit.VIS_DISCOUNT < 0)
            {
                throw new Exception("قيمة الخصم لا يمكن أن تكون سالبة.");
            }

            if (visit.VIS_DISCOUNT > visit.VIS_PRICE)
            {
                throw new Exception("قيمة الخصم لا يمكن أن تكون أكبر من سعر الزيارة.");
            }

            // احتساب الصافي تلقائياً لتأكيد صحة المبالغ
            visit.VIS_TOTAL = visit.VIS_PRICE - visit.VIS_DISCOUNT;

            if (!string.IsNullOrWhiteSpace(visit.VIS_PAY_TYPE) && visit.VIS_PAY_TYPE.Length > 50)
            {
                throw new Exception("نوع/وسيلة الدفع يجب ألا تتجاوز 50 حرفاً.");
            }
        }

        #endregion

        #region 6. جلب الزيارات بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من الزيارات بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsVist>> GetVisitsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _vistRepo.GetVisitsPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الزيارات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب إجمالي عدد الزيارات مع البحث

        /// <summary>
        /// جلب إجمالي عدد الزيارات المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalVisitsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _vistRepo.GetTotalVisitsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الزيارات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. توليد كود تلقائي للزيارة الجديدة

        public async Task<long> GetNewVis_CodeAsync()
        {
            try
            {
                long maxId = await _vistRepo.GetMaxVisitIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الزيارة الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
