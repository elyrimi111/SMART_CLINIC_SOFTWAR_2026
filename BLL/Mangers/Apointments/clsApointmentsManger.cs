using BLL.Mangers.Customers;
using Core.Entites.Customer;
using Core.Entities.Appointments;
using DAL.Repo.Apointments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Apointments
{
    public class clsApointmentsManger
    {
        private readonly clsAppointmentsRepo _appointmentsRepo;

        public clsApointmentsManger()
        {
            _appointmentsRepo = new clsAppointmentsRepo();
        }

        #region 1. جلب كافة المواعيد

        public async Task<List<clsAppointments>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _appointmentsRepo.GetAllAppointmentsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة المواعيد: " + ex.Message, ex);
            }
        }

        #endregion



        #region 2. جلب موعد برقم المعرف

        public async Task<clsAppointments?> GetAppointmentByIdAsync(long apoId)
        {
            if (apoId <= 0)
            {
                throw new ArgumentException("معرف الموعد غير صالح.");
            }

            try
            {
                return await _appointmentsRepo.GetAppointmentByIdAsync(apoId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الموعد: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. جلب المواعيد حسب العيادة

        public async Task<List<clsAppointments>> GetAppointmentsByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _appointmentsRepo.GetAppointmentsByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب مواعيد العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. إضافة موعد جديد

        public async Task<long> AddAppointmentAsync(clsAppointments appointment)
        {
            ValidateAppointmentData(appointment);

            try
            {
                return await _appointmentsRepo.AddAppointmentAsync(appointment);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الموعد: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. تعديل بيانات موعد

        public async Task<bool> UpdateAppointmentAsync(clsAppointments appointment)
        {
            if (appointment.APO_ID <= 0)
            {
                throw new ArgumentException("معرف الموعد المطلوب تعديله غير صحيح.");
            }

            ValidateAppointmentData(appointment);

            try
            {
                return await _appointmentsRepo.UpdateAppointmentAsync(appointment);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الموعد: " + ex.Message, ex);
            }
        }

        #endregion

        #region 6. حذف موعد

        public async Task<bool> DeleteAppointmentAsync(long apoId)
        {
            if (apoId <= 0)
            {
                throw new ArgumentException("معرف الموعد المراد حذفه غير صالح.");
            }

            try
            {
                return await _appointmentsRepo.DeleteAppointmentAsync(apoId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الموعد: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب المواعيد بنظام الصفحات مع البحث

        public async Task<List<clsAppointments>> GetAppointmentsPagedAsync(
            int pageNumber,
            int rowsPerPage,
            string? custName = null,
            string? docName = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
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
                return await _appointmentsRepo.GetAppointmentsPagedAsync(
                    pageNumber,
                    rowsPerPage,
                    custName,
                    docName,
                    fromDate,
                    toDate);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة المواعيد بنظام الصفحات: " + ex.Message, ex);
            }
        }


        #endregion

        #region 8. جلب إجمالي عدد المواعيد مع البحث


        public static async Task<int> GetTotalAppointmentsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await  clsAppointmentsRepo.GetTotalAppointmentsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد المواعيد: " + ex.Message, ex);
            }
        }

        #endregion

        #region 9. توليد كود تلقائي للموعد الجديد

        public async Task<long> GetNewApo_CodeAsync()
        {
            try
            {
                long maxId = await _appointmentsRepo.GetMaxAppointmentIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الموعد الجديد: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateAppointmentData(clsAppointments appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "بيانات الموعد فارغة.");
            }


            if (appointment.CUST_ID.HasValue && appointment.CUST_ID.Value <= 0)
            {
                throw new Exception("معرف العميل/المريض المحدد غير صالح.");
            }

            if (appointment.DOC_ID.HasValue && appointment.DOC_ID.Value <= 0)
            {
                throw new Exception("معرف الطبيب المحدد غير صالح.");
            }
        }

        #endregion
    }
}
