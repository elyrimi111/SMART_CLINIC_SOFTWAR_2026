using Core.Entites.Orders;
using DAL.Repo.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Orders
{
    public class OrdersManger
    {
        private readonly OrdersRepo _ordersRepo;

        public OrdersManger()
        {
            _ordersRepo = new OrdersRepo();
        }

        #region Get all 

        public async Task<List<Core.Entites.Orders.clsOrders>> GetAllOrdersAsync()
        {
            try
            {
                return await _ordersRepo.GetAllOrdersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الطلبات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By ID

        public async Task<Core.Entites.Orders.clsOrders?> GetOrderByIdAsync(long orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("معرف الطلب غير صالح.");
            }

            try
            {
                return await _ordersRepo.GetOrderByIdAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الطلب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New Order

        public async Task<long> AddOrderAsync(Core.Entites.Orders.clsOrders order)
        {
            ValidateOrderData(order);

            try
            {
                return await _ordersRepo.AddOrderAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الطلب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update Order

        public async Task<bool> UpdateOrderAsync(Core.Entites.Orders.clsOrders order)
        {
            if (order.ORDER_ID <= 0)
            {
                throw new ArgumentException("معرف الطلب المطلوب تعديله غير صحيح.");
            }

            ValidateOrderData(order);

            try
            {
                return await _ordersRepo.UpdateOrderAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الطلب: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. Delete Order

        public async Task<bool> DeleteOrderAsync(long orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("معرف الطلب المراد حذفه غير صالح.");
            }

            try
            {
                return await _ordersRepo.DeleteOrderAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الطلب: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Orders by Paging system

        public async Task<List<Core.Entites.Orders.clsOrders>> GetOrdersPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _ordersRepo.GetOrdersPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الطلبات بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get total Pages Count

        public async Task<int> GetTotalOrdersCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _ordersRepo.GetTotalOrdersCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الطلبات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Genrate New order Code

        public async Task<long> GetNewOrder_CodeAsync()
        {
            try
            {
                long maxId = await _ordersRepo.GetMaxOrderIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الطلب الجديد: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods 

        private void ValidateOrderData(Core.Entites.Orders.clsOrders order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "بيانات الطلب فارغة.");
            }

            if (!order.CUST_ID.HasValue || order.CUST_ID <= 0)
            {
                throw new Exception("يرجى تحديد العميل/المريض الخاص بالطلب.");
            }

            if (!string.IsNullOrEmpty(order.ORDER_NOTE) && order.ORDER_NOTE.Length > 500)
            {
                throw new Exception("ملاحظات الطلب يجب ألا تتجاوز 500 حرف.");
            }
        }

        #endregion
    }
}
