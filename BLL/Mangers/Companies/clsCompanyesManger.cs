using Core.Entites.Company;
using DAL.Repo.Companyes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Companies
{
    public class clsCompanyesManger
    {
        private readonly CompanesRepo _companiesRepo;

        public clsCompanyesManger()
        {
            _companiesRepo = new CompanesRepo();
        }

        #region 1. جلب الشركات بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من الشركات بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsCompany>> GetCompaniesPagedAsync(int pageNumber, int rowsPerPage, string? companyName = null)
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
                return await _companiesRepo.GetCompaniesPagedAsync(pageNumber, rowsPerPage, companyName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الشركات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب إجمالي عدد الشركات مع البحث

        /// <summary>
        /// جلب إجمالي عدد الشركات المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalCompaniesCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _companiesRepo.GetTotalCompaniesCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الشركات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. إضافة شركة جديدة

        public async Task<long> AddCompanyAsync(clsCompany company)
        {
            ValidateCompanyData(company);

            try
            {
                return await _companiesRepo.AddCompanyAsync(company);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الشركة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. تعديل بيانات شركة

        public async Task<bool> UpdateCompanyAsync(clsCompany company)
        {
            if (company.COM_ID <= 0)
            {
                throw new ArgumentException("معرف الشركة المطلوب تعديلها غير صحيح.");
            }

            ValidateCompanyData(company);

            try
            {
                return await _companiesRepo.UpdateCompanyAsync(company);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الشركة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. حذف شركة

        public async Task<bool> DeleteCompanyAsync(long comId)
        {
            if (comId <= 0)
            {
                throw new ArgumentException("معرف الشركة المراد حذفها غير صالح.");
            }

            try
            {
                return await _companiesRepo.DeleteCompanyAsync(comId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الشركة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 6. توليد كود تلقائي للشركة الجديدة

        public async Task<long> GetNewCom_CodeAsync()
        {
            try
            {
                return await _companiesRepo.GetNewComCodeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الشركة الجديد: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateCompanyData(clsCompany company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company), "بيانات الشركة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(company.COM_NAME))
            {
                throw new Exception("اسم الشركة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (company.COM_NAME.Length > 200)
            {
                throw new Exception("اسم الشركة يجب ألا يتجاوز 200 حرف.");
            }

            if (!string.IsNullOrEmpty(company.COM_MOBILE) && company.COM_MOBILE.Length > 50)
            {
                throw new Exception("رقم الهاتف يجب ألا يتجاوز 50 حرف.");
            }

            if (!string.IsNullOrEmpty(company.COM_ADDRESS) && company.COM_ADDRESS.Length > 250)
            {
                throw new Exception("عنوان الشركة يجب ألا يتجاوز 250 حرف.");
            }
        }

        #endregion
    }
}
