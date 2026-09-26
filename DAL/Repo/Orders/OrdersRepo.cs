using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Orders;
using DAL.ConnectionString;

namespace DAL.Repo.Orders
{
    public class OrdersRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsOrders MapDataReaderToOrder(SqlDataReader reader)
        {
            return new Core.Entites.Orders.clsOrders
            {
                ORDER_ID = Convert.ToInt64(reader["ORDER_ID"]),
                ORDER_CODE = reader["ORDER_CODE"] != DBNull.Value ? Convert.ToInt64(reader["ORDER_CODE"]) : null,
                ORDER_DATE = reader["ORDER_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["ORDER_DATE"]) : null,
                ORDER_TIME = reader["ORDER_TIME"] != DBNull.Value ? (TimeSpan)reader["ORDER_TIME"] : null,
                ORDER_NOTE = reader["ORDER_NOTE"] != DBNull.Value ? reader["ORDER_NOTE"].ToString() : null,
                CUST_ID = reader["CUST_ID"] != DBNull.Value ? Convert.ToInt64(reader["CUST_ID"]) : null,
                CLI_ID = reader["CLI"] != DBNull.Value ? Convert.ToInt64(reader["CLI"]) : null,
                CUST_NAME = reader["CUST_NAME"] != DBNull.Value ? reader["CUST_NAME"].ToString() : string.Empty
            };
        }


        #endregion

        #region 1. جلب كافة الطلبات (GetAll)

        public async Task<List<Core.Entites.Orders.clsOrders>> GetAllOrdersAsync()
        {
            List<Core.Entites.Orders.clsOrders> ordersList = new List<Core.Entites.Orders.clsOrders>();
            string query = @"SELECT * FROM dbo.fn_GetAllOrders()";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ordersList.Add(MapDataReaderToOrder(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ordersList;
        }

        #endregion

        #region 2. جلب طلب برقم المعرف (GetById)

        public async Task<Core.Entites.Orders.clsOrders?> GetOrderByIdAsync(long orderId)
        {
            Core.Entites.Orders.clsOrders? order = null;
            string query = @"SELECT * FROM dbo.fn_GetOrderById(@ORDER_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ORDER_ID", orderId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                order = MapDataReaderToOrder(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return order;
        }

        #endregion

        #region 3. إضافة طلب جديد (Insert)

        public async Task<long> AddOrderAsync(Core.Entites.Orders.clsOrders order)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.ORDER_TBL 
                            (ORDER_CODE, ORDER_DATE, ORDER_TIME, ORDER_NOTE, CUST_ID, CLI) 
                            VALUES 
                            (@ORDER_CODE, @ORDER_DATE, @ORDER_TIME, @ORDER_NOTE, @CUST_ID, @CLI);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ORDER_CODE", (object?)order.ORDER_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_DATE", (object?)order.ORDER_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_TIME", (object?)order.ORDER_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_NOTE", (object?)order.ORDER_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)order.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI", (object?)order.CLI_ID ?? DBNull.Value);

                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            newInsertedId = Convert.ToInt64(result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return newInsertedId;
        }

        #endregion

        #region 4. تعديل بيانات طلب (Update)

        public async Task<bool> UpdateOrderAsync(Core.Entites.Orders.clsOrders order)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.ORDER_TBL SET 
                            ORDER_CODE = @ORDER_CODE, 
                            ORDER_DATE = @ORDER_DATE, 
                            ORDER_TIME = @ORDER_TIME, 
                            ORDER_NOTE = @ORDER_NOTE, 
                            CUST_ID = @CUST_ID, 
                            CLI = @CLI
                            WHERE ORDER_ID = @ORDER_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ORDER_ID", order.ORDER_ID);
                        command.Parameters.AddWithValue("@ORDER_CODE", (object?)order.ORDER_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_DATE", (object?)order.ORDER_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_TIME", (object?)order.ORDER_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ORDER_NOTE", (object?)order.ORDER_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)order.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI", (object?)order.CLI_ID ?? DBNull.Value);

                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected > 0;
        }

        #endregion

        #region 5. حذف طلب (Delete)

        public async Task<bool> DeleteOrderAsync(long orderId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.ORDER_TBL WHERE ORDER_ID = @ORDER_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ORDER_ID", orderId);

                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return rowsAffected > 0;
        }

        #endregion

        #region 6. جلب الطلبات بنظام الصفحات والبحث (GetPaged)

        public async Task<List<Core.Entites.Orders.clsOrders>> GetOrdersPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<Core.Entites.Orders.clsOrders> ordersList = new List<Core.Entites.Orders.clsOrders>();
            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchOrders(@SearchValue)
                            ORDER BY ORDER_ID DESC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @RowsPerPage ROWS ONLY;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", string.IsNullOrWhiteSpace(searchQuery) ? DBNull.Value : searchQuery.Trim());
                        command.Parameters.AddWithValue("@Offset", offset);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ordersList.Add(MapDataReaderToOrder(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ordersList;
        }

        #endregion

        #region 7. جلب إجمالي عدد الطلبات بنظام البحث (GetTotalCount)

        public static async Task<int> GetTotalOrdersCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;
            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalOrdersCount(@SearchValue)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", string.IsNullOrWhiteSpace(searchQuery) ? DBNull.Value : searchQuery.Trim());

                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int count))
                        {
                            totalCount = count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الطلبات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 8. جلب أكبر معرف طلب مسجل (GetMaxOrderId)

        public async Task<long> GetMaxOrderIdAsync()
        {
            long maxId = 0;
            string query = @"SELECT dbo.fn_GetMaxOrderId()";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            maxId = Convert.ToInt64(result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return maxId;
        }

        #endregion
    }
}
