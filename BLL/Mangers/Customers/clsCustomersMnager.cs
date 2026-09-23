using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entites.Customer;
using DAL.Repo.Customers;

namespace BLL.Mangers.Customers
{
    public class clsCustomersManager
    {
        private readonly clsCustomersRepo _customersRepo;

        public clsCustomersManager()
        {
            _customersRepo = new clsCustomersRepo();
        }

        #region 1. جلب كافة العملاء

        public async Task<List<clsCust>> GetAllCustomersAsync()
        {
            try
            {
                return await _customersRepo.GetAllCustomersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة العملاء: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب عميل برقم المعرف

        public async Task<clsCust?> GetCustomerByIdAsync(long? custId)
        {
            if (custId <= 0)
            {
                throw new ArgumentException("معرف العميل غير صالح.");
            }

            try
            {
                return await _customersRepo.GetCustomerByIdAsync(custId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات العميل: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. إضافة عميل جديد

        public async Task<long> AddCustomerAsync(clsCust customer)
        {
            ValidateCustomerData(customer);

            try
            {
                return await _customersRepo.AddCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات العميل: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. تعديل بيانات عميل

        public async Task<bool> UpdateCustomerAsync(clsCust customer)
        {
            if (customer.CUST_ID <= 0)
            {
                throw new ArgumentException("معرف العميل المطلوب تعديله غير صحيح.");
            }

            ValidateCustomerData(customer);

            try
            {
                return await _customersRepo.UpdateCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات العميل: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. حذف عميل

        public async Task<bool> DeleteCustomerAsync(long custId)
        {
            if (custId <= 0)
            {
                throw new ArgumentException("معرف العميل المراد حذفه غير صالح.");
            }

            try
            {
                return await _customersRepo.DeleteCustomerAsync(custId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف العميل: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateCustomerData(clsCust customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "بيانات العميل فارغة.");
            }

            if (string.IsNullOrWhiteSpace(customer.CUST_F_NAME))
            {
                throw new Exception("الاسم الأول للعميل مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (customer.CUST_F_NAME.Length > 100)
            {
                throw new Exception("الاسم الأول للعميل يجب ألا يتجاوز 100 حرف.");
            }

            if (!string.IsNullOrEmpty(customer.CUST_MOBILE1) && customer.CUST_MOBILE1.Length > 50)
            {
                throw new Exception("رقم الهاتف الأول يجب ألا يتجاوز 50 حرف.");
            }

            if (!string.IsNullOrEmpty(customer.CUST_MOBILE2) && customer.CUST_MOBILE2.Length > 50)
            {
                throw new Exception("رقم الهاتف الثاني يجب ألا يتجاوز 50 حرف.");
            }
        }

        #endregion

        #region 6. جلب العملاء بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من العملاء بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsCust>> GetCustomersPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _customersRepo.GetCustomersPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة العملاء بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب إجمالي عدد العملاء مع البحث

        /// <summary>
        /// جلب إجمالي عدد العملاء المطابقين لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalCustomersCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _customersRepo.GetTotalCustomersCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد العملاء: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. توليد كود تلقائي للعميل الجديد

        public async Task<long> GetNewCust_CodeAsync()
        {
            try
            {
                long maxId = await _customersRepo.GetMaxCustomerIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود العميل الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
