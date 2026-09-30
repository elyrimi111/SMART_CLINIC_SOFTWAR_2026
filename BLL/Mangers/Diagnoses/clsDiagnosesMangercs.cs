using Core.Entites.Diagnos;
using DAL.Repo.Diagnos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Diagnoses
{
    public class clsDiagnosesManger
    {
        private readonly clsDiagnosRepo _diagnosRepo;

        public clsDiagnosesManger()
        {
            _diagnosRepo = new clsDiagnosRepo();
        }

        #region get All 

        public static async Task<List<clsDiagnos>> GetAllDiagnosAsync()
        {
            try
            {
                return await clsDiagnosRepo.GetAllDiagnosAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة التشخيصات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get by id 

        public static async Task<clsDiagnos> GetDiagnosByIdAsync(long digId)
        {
            if (digId <= 0)
            {
                throw new ArgumentException("معرف التشخيص غير صالح.");
            }

            try
            {
                return await clsDiagnosRepo.GetDiagnosByIdAsync(digId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات التشخيص: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get by Clinc ID

        public async Task<List<clsDiagnos>> GetDiagnosByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _diagnosRepo.GetDiagnosByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب تشخيصات العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add

        public async Task<long> AddDiagnosAsync(clsDiagnos diagnos)
        {
            ValidateDiagnosData(diagnos);

            try
            {
                return await _diagnosRepo.AddDiagnosAsync(diagnos);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات التشخيص: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update

        public async Task<bool> UpdateDiagnosAsync(clsDiagnos diagnos)
        {
            if (diagnos.DIG_ID <= 0)
            {
                throw new ArgumentException("معرف التشخيص المطلوب تعديله غير صحيح.");
            }

            ValidateDiagnosData(diagnos);

            try
            {
                return await _diagnosRepo.UpdateDiagnosAsync(diagnos);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات التشخيص: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete

        public async Task<bool> DeleteDiagnosAsync(long digId)
        {
            if (digId <= 0)
            {
                throw new ArgumentException("معرف التشخيص المراد حذفه غير صالح.");
            }

            try
            {
                return await _diagnosRepo.DeleteDiagnosAsync(digId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف التشخيص: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods  

        private void ValidateDiagnosData(clsDiagnos diagnos)
        {
            if (diagnos == null)
            {
                throw new ArgumentNullException(nameof(diagnos), "بيانات التشخيص فارغة.");
            }

            if (string.IsNullOrWhiteSpace(diagnos.DIG_NAME))
            {
                throw new Exception("اسم التشخيص مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (diagnos.DIG_NAME.Length > 200)
            {
                throw new Exception("اسم التشخيص يجب ألا يتجاوز 200 حرف.");
            }
        }

        #endregion

        #region Get By Pageing 

        /// <summary>
        /// جلب صفحة محددة من التشخيصات بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsDiagnos>> GetDiagnosPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _diagnosRepo.GetDiagnosPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة التشخيصات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Total 

        public async Task<int> GetTotalDiagnosCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _diagnosRepo.GetTotalDiagnosCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد التشخيصات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Genrate New Code

        public async Task<long> GetNewDig_CodeAsync()
        {
            try
            {
                long maxId = await _diagnosRepo.GetMaxDiagnosIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود التشخيص الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
