using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entites.Med_Report;
using DAL.Repo.MedReport;

namespace BLL.Mangers.MedRepo
{
    public class clsMedRepoManager
    {
        private readonly clsMedReportRepo _medReportRepo;

        public clsMedRepoManager()
        {
            _medReportRepo = new clsMedReportRepo();
        }

        #region GetAll

        public async Task<List<clsMed_Report>> GetAllMedReportsAsync()
        {
            try
            {
                return await _medReportRepo.GetAllMedReportsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة التقارير الطبية: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Id

        public async Task<clsMed_Report?> GetMedReportByIdAsync(long? mrepId)
        {
            if (mrepId == null || mrepId <= 0)
            {
                throw new ArgumentException("معرف التقرير الطبي غير صالح.");
            }

            try
            {
                return await _medReportRepo.GetMedReportByIdAsync(mrepId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات التقرير الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Clinic

        public async Task<List<clsMed_Report>> GetMedReportsByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _medReportRepo.GetMedReportsByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب تقارير العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New MedReport

        public async Task<long> AddMedReportAsync(clsMed_Report medReport)
        {
            ValidateMedReportData(medReport);

            try
            {
                return await _medReportRepo.AddMedReportAsync(medReport);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات التقرير الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update MedReport

        public async Task<bool> UpdateMedReportAsync(clsMed_Report medReport)
        {
            if (medReport.MREP_ID <= 0)
            {
                throw new ArgumentException("معرف التقرير الطبي المطلوب تعديله غير صحيح.");
            }

            ValidateMedReportData(medReport);

            try
            {
                return await _medReportRepo.UpdateMedReportAsync(medReport);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات التقرير الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete MedReport

        public async Task<bool> DeleteMedReportAsync(long mrepId)
        {
            if (mrepId <= 0)
            {
                throw new ArgumentException("معرف التقرير الطبي المراد حذفه غير صالح.");
            }

            try
            {
                return await _medReportRepo.DeleteMedReportAsync(mrepId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف التقرير الطبي: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void ValidateMedReportData(clsMed_Report medReport)
        {
            if (medReport == null)
            {
                throw new ArgumentNullException(nameof(medReport), "بيانات التقرير الطبي فارغة.");
            }

            if (string.IsNullOrWhiteSpace(medReport.MREP_NAME))
            {
                throw new Exception("اسم/عنوان التقرير الطبي مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (medReport.MREP_NAME.Length > 200)
            {
                throw new Exception("اسم التقرير الطبي يجب ألا يتجاوز 200 حرف.");
            }

            if (!string.IsNullOrEmpty(medReport.MREP_TEXT) && medReport.MREP_TEXT.Length > 200)
            {
                throw new Exception("نص التقرير الطبي يجب ألا يتجاوز 200 حرف.");
            }

            if (!string.IsNullOrEmpty(medReport.MREP_NOTE) && medReport.MREP_NOTE.Length > 500)
            {
                throw new Exception("الملاحظات يجب ألا تتجاوز 500 حرف.");
            }
        }

        #endregion

        #region Get MedReport Async (Paged)

        public async Task<List<clsMed_Report>> GetMedReportsPagedAsync(int pageNumber, int rowsPerPage, string? reportName = null)
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
                return await _medReportRepo.GetMedReportsPagedAsync(pageNumber, rowsPerPage, reportName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة التقارير الطبية بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Total MedReports

        public async Task<int> GetTotalMedReportsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _medReportRepo.GetTotalMedReportsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد التقارير الطبية: " + ex.Message, ex);
            }
        }

        #endregion

        #region Generate New MedReport Code

        public async Task<long> GetNewMrep_CodeAsync()
        {
            try
            {
                long maxId = await _medReportRepo.GetMaxMedReportIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود التقرير الطبي الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
