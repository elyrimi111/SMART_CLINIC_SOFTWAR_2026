using Core.Entites.Doctors;
using DAL.Repo.Dcotors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Dcotros
{
    public class clsDoctorsManger
    {
        private readonly clsDoctorsRepo _doctorsRepo;

        public clsDoctorsManger()
        {
            _doctorsRepo = new clsDoctorsRepo();
        }

        #region GetAll

        public async Task<List<clsDoctors>> GetAllDoctorsAsync()
        {
            try
            {
                return await _doctorsRepo.GetAllDoctorsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأطباء: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Id

        public async Task<clsDoctors?> GetDoctorByIdAsync(long? docId)
        {
            if (docId <= 0)
            {
                throw new ArgumentException("معرف الطبيب غير صالح.");
            }

            try
            {
                return await _doctorsRepo.GetDoctorByIdAsync(docId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الطبيب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New Doctor

        public async Task<long> AddDoctorAsync(clsDoctors doctor)
        {
            ValidateDoctorData(doctor);

            try
            {
                return await _doctorsRepo.AddDoctorAsync(doctor);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الطبيب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update Doctor

        public async Task<bool> UpdateDoctorAsync(clsDoctors doctor)
        {
            if (doctor.DOC_ID <= 0)
            {
                throw new ArgumentException("معرف الطبيب المطلوب تعديله غير صحيح.");
            }

            ValidateDoctorData(doctor);

            try
            {
                return await _doctorsRepo.UpdateDoctorAsync(doctor);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الطبيب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete Doctor

        public async Task<bool> DeleteDoctorAsync(long docId)
        {
            if (docId <= 0)
            {
                throw new ArgumentException("معرف الطبيب المراد حذفه غير صالح.");
            }

            try
            {
                return await _doctorsRepo.DeleteDoctorAsync(docId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الطبيب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void ValidateDoctorData(clsDoctors doctor)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor), "بيانات الطبيب فارغة.");
            }

            if (string.IsNullOrWhiteSpace(doctor.DOC_NAME))
            {
                throw new Exception("اسم الطبيب مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (doctor.DOC_NAME.Length > 200)
            {
                throw new Exception("اسم الطبيب يجب ألا يتجاوز 200 حرف.");
            }

            if (!string.IsNullOrEmpty(doctor.DOC_MOBILE) && doctor.DOC_MOBILE.Length > 50)
            {
                throw new Exception("رقم الهاتف يجب ألا يتجاوز 50 حرف.");
            }
        }

        #endregion

        #region Get Doctor Async

        /// <summary>
        /// جلب صفحة محددة من الأطباء بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsDoctors>> GetDoctorsPagedAsync(int pageNumber, int rowsPerPage, string? doctorName = null)
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
                return await _doctorsRepo.GetDoctorsPagedAsync(pageNumber, rowsPerPage, doctorName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأطباء بنظام الصفحات: " + ex.Message, ex);
            }
        }


        #endregion

        #region Get Total Doctors 
        public async Task<int> GetTotalDoctorsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _doctorsRepo.GetTotalDoctorsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأطباء: " + ex.Message, ex);
            }
        }

        #endregion

        #region Genrate New Doctor Code

        public async Task<long> GetNewDoc_CodeAsync()
        {
            try
            {
                long maxId = await _doctorsRepo.GetMaxDoctorIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الطبيب الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
