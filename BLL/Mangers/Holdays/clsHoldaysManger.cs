using Core.Entites.Holday;
using DAL.Repo.Holdays;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Holdays
{
    public class clsHoldaysManger
    {
        private readonly clsHoldaysRepo _holdaysRepo;

        public clsHoldaysManger()
        {
            _holdaysRepo = new clsHoldaysRepo();
        }

        #region GetAll

        public async Task<List<clsHolday>> GetAllHolidaysAsync()
        {
            try
            {
                return await _holdaysRepo.GetAllHolidaysAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الإجازات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Id

        public async Task<clsHolday?> GetHolidayByIdAsync(long? holId)
        {
            if (holId <= 0)
            {
                throw new ArgumentException("معرف الإجازة غير صالح.");
            }

            try
            {
                return await _holdaysRepo.GetHolidayByIdAsync(holId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الإجازة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Clinic

        public async Task<List<clsHolday>> GetHolidaysByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _holdaysRepo.GetHolidaysByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجازات العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New Holiday

        public async Task<long> AddHolidayAsync(clsHolday holiday)
        {
            ValidateHolidayData(holiday);

            try
            {
                return await _holdaysRepo.AddHolidayAsync(holiday);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الإجازة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update Holiday

        public async Task<bool> UpdateHolidayAsync(clsHolday holiday)
        {
            if (holiday.HOL_ID <= 0)
            {
                throw new ArgumentException("معرف الإجازة المطلوب تعديلها غير صحيح.");
            }

            ValidateHolidayData(holiday);

            try
            {
                return await _holdaysRepo.UpdateHolidayAsync(holiday);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الإجازة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete Holiday

        public async Task<bool> DeleteHolidayAsync(long holId)
        {
            if (holId <= 0)
            {
                throw new ArgumentException("معرف الإجازة المراد حذفها غير صالح.");
            }

            try
            {
                return await _holdaysRepo.DeleteHolidayAsync(holId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الإجازة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void ValidateHolidayData(clsHolday holiday)
        {
            if (holiday == null)
            {
                throw new ArgumentNullException(nameof(holiday), "بيانات الإجازة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(holiday.HOL_NAME))
            {
                throw new Exception("اسم الإجازة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (holiday.HOL_NAME.Length > 50)
            {
                throw new Exception("اسم الإجازة يجب ألا يتجاوز 50 حرف.");
            }
        }

        #endregion

        #region Get Holidays Async (Paged)

        /// <summary>
        /// جلب صفحة محددة من الإجازات بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsHolday>> GetHolidaysPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _holdaysRepo.GetHolidaysPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الإجازات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Total Holidays

        public async Task<int> GetTotalHolidaysCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _holdaysRepo.GetTotalHolidaysCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الإجازات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Generate New Holiday Code

        public async Task<long> GetNewHol_CodeAsync()
        {
            try
            {
                long maxId = await _holdaysRepo.GetMaxHolidayIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الإجازة الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
