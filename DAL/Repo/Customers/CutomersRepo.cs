using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Customer;
using DAL.ConnectionString;

namespace DAL.Repo.Customers
{
    public class clsCustomersRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsCust MapDataReaderToCustomer(SqlDataReader reader)
        {
            return new clsCust
            {
                CUST_ID = Convert.ToInt64(reader["CUST_ID"]),
                CUST_CODE = reader["CUST_CODE"] != DBNull.Value ? Convert.ToInt64(reader["CUST_CODE"]) : 0,
                CUST_F_NAME = reader["CUST_F_NAME"] != DBNull.Value ? reader["CUST_F_NAME"].ToString()! : string.Empty,
                CUST_S_NAME = reader["CUST_S_NAME"] != DBNull.Value ? reader["CUST_S_NAME"].ToString()! : string.Empty,
                CUST_T_NAME = reader["CUST_T_NAME"] != DBNull.Value ? reader["CUST_T_NAME"].ToString()! : string.Empty,
                CUST_L_NAME = reader["CUST_L_NAME"] != DBNull.Value ? reader["CUST_L_NAME"].ToString()! : string.Empty,
                CUST_AGE = reader["CUST_AGE"] != DBNull.Value ? reader["CUST_AGE"].ToString()! : string.Empty,
                CUST_BD = reader["CUST_BD"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["CUST_BD"])) : default,
                CUST_MOBILE1 = reader["CUST_MOBILE1"] != DBNull.Value ? reader["CUST_MOBILE1"].ToString()! : string.Empty,
                CUST_MOBILE2 = reader["CUST_MOBILE2"] != DBNull.Value ? reader["CUST_MOBILE2"].ToString()! : string.Empty,
                CUST_ADDRESS = reader["CUST_ADDRESS"] != DBNull.Value ? reader["CUST_ADDRESS"].ToString()! : string.Empty,
                CUST_SAVE_STATE = reader["CUST_SAVE_STATE"] != DBNull.Value ? reader["CUST_SAVE_STATE"].ToString()! : string.Empty,
                CARD_ID = reader["CARD_ID"] != DBNull.Value ? Convert.ToInt64(reader["CARD_ID"]) : 0,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region 1. جلب كافة العملاء (GetAll)

        public async Task<List<clsCust>> GetAllCustomersAsync()
        {
            List<clsCust> customersList = new List<clsCust>();

            string query = @"SELECT * FROM dbo.fn_GetAllCustomers()";

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
                                customersList.Add(MapDataReaderToCustomer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return customersList;
        }

        #endregion

        #region 2. جلب عميل برقم المعرف (GetById)

        public async Task<clsCust?> GetCustomerByIdAsync(long? custId)
        {
            clsCust? customer = null;

            string query = @"SELECT * FROM dbo.fn_GetCustomerById(@CUST_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_ID", custId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                customer = MapDataReaderToCustomer(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return customer;
        }

        #endregion

        #region 3. إضافة عميل جديد (Insert)

        public async Task<long> AddCustomerAsync(clsCust customer)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.CUST_TBL 
                            (CUST_CODE, CUST_F_NAME, CUST_S_NAME, CUST_T_NAME, CUST_L_NAME, CUST_AGE, CUST_BD, CUST_MOBILE1, CUST_MOBILE2, CUST_ADDRESS, CUST_SAVE_STATE, CARD_ID, CLI_ID) 
                            VALUES 
                            (@CUST_CODE, @CUST_F_NAME, @CUST_S_NAME, @CUST_T_NAME, @CUST_L_NAME, @CUST_AGE, @CUST_BD, @CUST_MOBILE1, @CUST_MOBILE2, @CUST_ADDRESS, @CUST_SAVE_STATE, @CARD_ID, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_CODE", customer.CUST_CODE > 0 ? customer.CUST_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_F_NAME", string.IsNullOrWhiteSpace(customer.CUST_F_NAME) ? DBNull.Value : customer.CUST_F_NAME);
                        command.Parameters.AddWithValue("@CUST_S_NAME", string.IsNullOrWhiteSpace(customer.CUST_S_NAME) ? DBNull.Value : customer.CUST_S_NAME);
                        command.Parameters.AddWithValue("@CUST_T_NAME", string.IsNullOrWhiteSpace(customer.CUST_T_NAME) ? DBNull.Value : customer.CUST_T_NAME);
                        command.Parameters.AddWithValue("@CUST_L_NAME", string.IsNullOrWhiteSpace(customer.CUST_L_NAME) ? DBNull.Value : customer.CUST_L_NAME);
                        command.Parameters.AddWithValue("@CUST_AGE", int.TryParse(customer.CUST_AGE, out int age) ? age : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_BD", customer.CUST_BD != default ? customer.CUST_BD.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_MOBILE1", string.IsNullOrWhiteSpace(customer.CUST_MOBILE1) ? DBNull.Value : customer.CUST_MOBILE1);
                        command.Parameters.AddWithValue("@CUST_MOBILE2", string.IsNullOrWhiteSpace(customer.CUST_MOBILE2) ? DBNull.Value : customer.CUST_MOBILE2);
                        command.Parameters.AddWithValue("@CUST_ADDRESS", string.IsNullOrWhiteSpace(customer.CUST_ADDRESS) ? DBNull.Value : customer.CUST_ADDRESS);
                        command.Parameters.AddWithValue("@CUST_SAVE_STATE", string.IsNullOrWhiteSpace(customer.CUST_SAVE_STATE) ? DBNull.Value : customer.CUST_SAVE_STATE);
                        command.Parameters.AddWithValue("@CARD_ID", customer.CARD_ID > 0 ? customer.CARD_ID : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", customer.CLI_ID > 0 ? customer.CLI_ID : DBNull.Value);

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

        #region 4. تعديل بيانات عميل (Update)

        public async Task<bool> UpdateCustomerAsync(clsCust customer)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.CUST_TBL SET 
                            CUST_CODE = @CUST_CODE, 
                            CUST_F_NAME = @CUST_F_NAME, 
                            CUST_S_NAME = @CUST_S_NAME, 
                            CUST_T_NAME = @CUST_T_NAME, 
                            CUST_L_NAME = @CUST_L_NAME, 
                            CUST_AGE = @CUST_AGE, 
                            CUST_BD = @CUST_BD, 
                            CUST_MOBILE1 = @CUST_MOBILE1, 
                            CUST_MOBILE2 = @CUST_MOBILE2, 
                            CUST_ADDRESS = @CUST_ADDRESS, 
                            CUST_SAVE_STATE = @CUST_SAVE_STATE, 
                            CARD_ID = @CARD_ID, 
                            CLI_ID = @CLI_ID
                            WHERE CUST_ID = @CUST_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_ID", customer.CUST_ID);
                        command.Parameters.AddWithValue("@CUST_CODE", customer.CUST_CODE > 0 ? customer.CUST_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_F_NAME", string.IsNullOrWhiteSpace(customer.CUST_F_NAME) ? DBNull.Value : customer.CUST_F_NAME);
                        command.Parameters.AddWithValue("@CUST_S_NAME", string.IsNullOrWhiteSpace(customer.CUST_S_NAME) ? DBNull.Value : customer.CUST_S_NAME);
                        command.Parameters.AddWithValue("@CUST_T_NAME", string.IsNullOrWhiteSpace(customer.CUST_T_NAME) ? DBNull.Value : customer.CUST_T_NAME);
                        command.Parameters.AddWithValue("@CUST_L_NAME", string.IsNullOrWhiteSpace(customer.CUST_L_NAME) ? DBNull.Value : customer.CUST_L_NAME);
                        command.Parameters.AddWithValue("@CUST_AGE", int.TryParse(customer.CUST_AGE, out int age) ? age : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_BD", customer.CUST_BD != default ? customer.CUST_BD.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_MOBILE1", string.IsNullOrWhiteSpace(customer.CUST_MOBILE1) ? DBNull.Value : customer.CUST_MOBILE1);
                        command.Parameters.AddWithValue("@CUST_MOBILE2", string.IsNullOrWhiteSpace(customer.CUST_MOBILE2) ? DBNull.Value : customer.CUST_MOBILE2);
                        command.Parameters.AddWithValue("@CUST_ADDRESS", string.IsNullOrWhiteSpace(customer.CUST_ADDRESS) ? DBNull.Value : customer.CUST_ADDRESS);
                        command.Parameters.AddWithValue("@CUST_SAVE_STATE", string.IsNullOrWhiteSpace(customer.CUST_SAVE_STATE) ? DBNull.Value : customer.CUST_SAVE_STATE);
                        command.Parameters.AddWithValue("@CARD_ID", customer.CARD_ID > 0 ? customer.CARD_ID : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", customer.CLI_ID > 0 ? customer.CLI_ID : DBNull.Value);

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

        #region 5. حذف عميل (Delete)

        public async Task<bool> DeleteCustomerAsync(long custId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.CUST_TBL WHERE CUST_ID = @CUST_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_ID", custId);

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

        #region 6. جلب العملاء بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsCust>> GetCustomersPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsCust> customersList = new List<clsCust>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchCustomers(@SearchValue)
                            ORDER BY CUST_ID
                            OFFSET @Offset ROWS 
                            FETCH NEXT @RowsPerPage ROWS ONLY;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", (object?)searchQuery ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Offset", offset);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                customersList.Add(MapDataReaderToCustomer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return customersList;
        }

        #endregion

        #region 7. جلب إجمالي عدد العملاء بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalCustomersCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalCustomersCount(@SearchValue)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", (object?)searchQuery ?? DBNull.Value);

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد العملاء: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 8. جلب أكبر معرف/كود عميل مسجل (GetMaxCustomerId)

        public async Task<long> GetMaxCustomerIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxCustomerId()";

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
