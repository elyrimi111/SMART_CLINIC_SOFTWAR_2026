using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Services;
using DAL.ConnectionString;

namespace DAL.Repo.Services
{
    public class clsServicesRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsService MapDataReaderToService(SqlDataReader reader)
        {
            return new clsService
            {
                SER_ID = Convert.ToInt64(reader["SER_ID"]),
                SER_CODE = reader["SER_CODE"] != DBNull.Value ? reader["SER_CODE"].ToString() : null,
                SER_NAME = reader["SER_NAME"] != DBNull.Value ? reader["SER_NAME"].ToString() : null,
                SER_TYPE = reader["SER_TYPE"] != DBNull.Value ? reader["SER_TYPE"].ToString() : null,
                SER_PRICE = reader["SER_PRICE"] != DBNull.Value ? reader["SER_PRICE"].ToString() : null,
                SER_NOTE = reader["SER_NOTE"] != DBNull.Value ? reader["SER_NOTE"].ToString() : null,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region 1. جلب كافة الخدمات (GetAll)

        public async Task<List<clsService>> GetAllServicesAsync()
        {
            List<clsService> servicesList = new List<clsService>();

            string query = @"SELECT * FROM dbo.SERVICE_TBL";

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
                                servicesList.Add(MapDataReaderToService(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return servicesList;
        }

        #endregion

        #region 2. جلب خدمة برقم المعرف (GetById)

        public async Task<clsService?> GetServiceByIdAsync(long serId)
        {
            clsService? service = null;
            string query = @"SELECT SER_ID, SER_CODE, SER_NAME, SER_TYPE, SER_PRICE, SER_NOTE, CLI_ID 
                            FROM dbo.SERVICE_TBL 
                            WHERE SER_ID = @SER_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SER_ID", serId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                service = MapDataReaderToService(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return service;
        }

        #endregion

        #region 3. إضافة خدمة جديدة (Insert)

        public async Task<long> AddServiceAsync(clsService service)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.SERVICE_TBL 
                            (SER_CODE, SER_NAME, SER_TYPE, SER_PRICE, SER_NOTE, CLI_ID) 
                            VALUES 
                            (@SER_CODE, @SER_NAME, @SER_TYPE, @SER_PRICE, @SER_NOTE, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SER_CODE", (object?)service.SER_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_NAME", (object?)service.SER_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_TYPE", (object?)service.SER_TYPE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_PRICE", (object?)service.SER_PRICE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_NOTE", (object?)service.SER_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", service.CLI_ID != 0 ? service.CLI_ID : DBNull.Value);

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

        #region 4. تعديل بيانات خدمة (Update)

        public async Task<bool> UpdateServiceAsync(clsService service)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.SERVICE_TBL SET 
                            SER_CODE = @SER_CODE, 
                            SER_NAME = @SER_NAME, 
                            SER_TYPE = @SER_TYPE, 
                            SER_PRICE = @SER_PRICE, 
                            SER_NOTE = @SER_NOTE, 
                            CLI_ID = @CLI_ID
                            WHERE SER_ID = @SER_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SER_ID", service.SER_ID);
                        command.Parameters.AddWithValue("@SER_CODE", (object?)service.SER_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_NAME", (object?)service.SER_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_TYPE", (object?)service.SER_TYPE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_PRICE", (object?)service.SER_PRICE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SER_NOTE", (object?)service.SER_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", service.CLI_ID != 0 ? service.CLI_ID : DBNull.Value);

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

        #region 5. حذف خدمة (Delete)

        public async Task<bool> DeleteServiceAsync(long serId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.SERVICE_TBL WHERE SER_ID = @SER_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SER_ID", serId);

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

        #region 6. جلب الخدمات بنظام الصفحات (GetPaged)

        public async Task<List<clsService>> GetServicesPagedAsync(int pageNumber, int rowsPerPage)
        {
            List<clsService> servicesList = new List<clsService>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetPageByTable", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@RowPageNumber", rowsPerPage);
                        command.Parameters.AddWithValue("@TableName", "SERVICE_TBL");
                        command.Parameters.AddWithValue("@OrderFieldName", "SER_ID");

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                servicesList.Add(MapDataReaderToService(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return servicesList;
        }

        public async Task<List<clsService>> GetServicesPagedAsync(int pageNumber, int rowsPerPage, string? serviceName = null)
        {
            List<clsService> servicesList = new List<clsService>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetServicesPaged", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                        command.Parameters.AddWithValue("@ServiceName", (object?)serviceName ?? DBNull.Value);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                servicesList.Add(MapDataReaderToService(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return servicesList;
        }

        #endregion

        #region 7. جلب عدد الخدمات بنظام البحث

        public async Task<int> GetTotalServicesCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    string query = @"SELECT COUNT(*) FROM SERVICE_TBL 
                             WHERE (@SearchValue IS NULL 
                                 OR @SearchValue = '' 
                                 OR SER_NAME LIKE '%' + @SearchValue + '%' 
                                 OR CAST(SER_ID AS VARCHAR) LIKE '%' + @SearchValue + '%')";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الخدمات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 8. جلب أكبر معرف/كود خدمة مسجل (GetMaxServiceId)

        public async Task<long> GetMaxServiceIdAsync()
        {
            long maxId = 0;
            string query = @"SELECT ISNULL(MAX(SER_ID), 0) FROM dbo.SERVICE_TBL";

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
