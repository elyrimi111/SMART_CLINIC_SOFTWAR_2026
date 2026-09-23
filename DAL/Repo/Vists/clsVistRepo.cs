using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Vist;
using DAL.ConnectionString;

namespace DAL.Repo.Vists
{
    public class clsVistRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsVist MapDataReaderToVisit(SqlDataReader reader)
        {
            return new clsVist
            {
                VIS_ID = Convert.ToInt64(reader["VIS_ID"]),
                VIS_CODE = reader["VIS_CODE"] != DBNull.Value ? Convert.ToInt64(reader["VIS_CODE"]) : 0,
                VIS_NAME = reader["VIS_NAME"] != DBNull.Value ? reader["VIS_NAME"].ToString()! : string.Empty,
                VIS_DATE = reader["VIS_DATE"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["VIS_DATE"])) : default,
                VIS_TYPE = reader["VIS_TYPE"] != DBNull.Value ? reader["VIS_TYPE"].ToString()! : string.Empty,
                VIS_TIME = reader["VIS_TIME"] != DBNull.Value ? TimeOnly.FromTimeSpan((TimeSpan)reader["VIS_TIME"]) : default,
                CUST_ID = reader["CUST_ID"] != DBNull.Value ? Convert.ToInt64(reader["CUST_ID"]) : 0,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0,
                APO_ID = reader["APO_ID"] != DBNull.Value ? Convert.ToInt64(reader["APO_ID"]) : 0,
                DOC_ID = reader["DOC_ID"] != DBNull.Value ? Convert.ToInt64(reader["DOC_ID"]) : 0,
                SERLIST_ID = reader["SERLIST_ID"] != DBNull.Value ? Convert.ToInt64(reader["SERLIST_ID"]) : 0,

                // الحقول الجديدة
                VIS_PRICE = reader["VIS_PRICE"] != DBNull.Value ? Convert.ToDecimal(reader["VIS_PRICE"]) : 0m,
                VIS_DISCOUNT = reader["VIS_DISCOUNT"] != DBNull.Value ? Convert.ToDecimal(reader["VIS_DISCOUNT"]) : 0m,
                VIS_TOTAL = reader["VIS_TOTAL"] != DBNull.Value ? Convert.ToDecimal(reader["VIS_TOTAL"]) : 0m,
                VIS_PAY_TYPE = reader["VIS_PAY_TYPE"] != DBNull.Value ? reader["VIS_PAY_TYPE"].ToString()! : string.Empty
            };
        }

        #endregion

        #region 1. جلب كافة الزيارات (GetAll)

        public async Task<List<clsVist>> GetAllVisitsAsync()
        {
            List<clsVist> visitsList = new List<clsVist>();
            string query = @"SELECT * FROM dbo.fn_GetAllVisits()";

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
                                visitsList.Add(MapDataReaderToVisit(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return visitsList;
        }

        #endregion

        #region 2. جلب زيارة برقم المعرف (GetById)

        public async Task<clsVist?> GetVisitByIdAsync(long? visitId)
        {
            clsVist? visit = null;
            string query = @"SELECT * FROM dbo.fn_GetVisitById(@VIS_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VIS_ID", (object?)visitId ?? DBNull.Value);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                visit = MapDataReaderToVisit(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return visit;
        }

        #endregion

        #region 3. جلب الزيارات حسب العيادة (GetByClinic)

        public async Task<List<clsVist>> GetVisitsByClinicAsync(long clinicId)
        {
            List<clsVist> visitsList = new List<clsVist>();
            string query = @"SELECT * FROM dbo.fn_GetVisitsByClinic(@CLI_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CLI_ID", clinicId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                visitsList.Add(MapDataReaderToVisit(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return visitsList;
        }

        #endregion

        #region 4. إضافة زيارة جديدة (Insert)

        public async Task<long> AddVisitAsync(clsVist visit)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.VIST_TBL 
                            (VIS_CODE, VIS_NAME, VIS_DATE, VIS_TYPE, VIS_TIME, CUST_ID, CLI_ID, APO_ID, DOC_ID, SERLIST_ID, VIS_PRICE, VIS_DISCOUNT, VIS_TOTAL, VIS_PAY_TYPE) 
                            VALUES 
                            (@VIS_CODE, @VIS_NAME, @VIS_DATE, @VIS_TYPE, @VIS_TIME, @CUST_ID, @CLI_ID, @APO_ID, @DOC_ID, @SERLIST_ID, @VIS_PRICE, @VIS_DISCOUNT, @VIS_TOTAL, @VIS_PAY_TYPE);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VIS_CODE", visit.VIS_CODE);
                        command.Parameters.AddWithValue("@VIS_NAME", (object?)visit.VIS_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_DATE", visit.VIS_DATE != default ? visit.VIS_DATE.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_TYPE", (object?)visit.VIS_TYPE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_TIME", visit.VIS_TIME != default ? visit.VIS_TIME.ToTimeSpan() : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", visit.CUST_ID);
                        command.Parameters.AddWithValue("@CLI_ID", visit.CLI_ID);
                        command.Parameters.AddWithValue("@APO_ID", visit.APO_ID);
                        command.Parameters.AddWithValue("@DOC_ID", visit.DOC_ID);
                        command.Parameters.AddWithValue("@SERLIST_ID", visit.SERLIST_ID);

                        // إضافة البرامترات الجديدة
                        command.Parameters.AddWithValue("@VIS_PRICE", visit.VIS_PRICE);
                        command.Parameters.AddWithValue("@VIS_DISCOUNT", visit.VIS_DISCOUNT);
                        command.Parameters.AddWithValue("@VIS_TOTAL", visit.VIS_TOTAL);
                        command.Parameters.AddWithValue("@VIS_PAY_TYPE", (object?)visit.VIS_PAY_TYPE ?? DBNull.Value);

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

        #region 5. تعديل بيانات زيارة (Update)

        public async Task<bool> UpdateVisitAsync(clsVist visit)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.VIST_TBL SET 
                            VIS_CODE = @VIS_CODE, 
                            VIS_NAME = @VIS_NAME, 
                            VIS_DATE = @VIS_DATE, 
                            VIS_TYPE = @VIS_TYPE, 
                            VIS_TIME = @VIS_TIME, 
                            CUST_ID = @CUST_ID, 
                            CLI_ID = @CLI_ID, 
                            APO_ID = @APO_ID, 
                            DOC_ID = @DOC_ID, 
                            SERLIST_ID = @SERLIST_ID,
                            VIS_PRICE = @VIS_PRICE,
                            VIS_DISCOUNT = @VIS_DISCOUNT,
                            VIS_TOTAL = @VIS_TOTAL,
                            VIS_PAY_TYPE = @VIS_PAY_TYPE
                            WHERE VIS_ID = @VIS_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VIS_ID", visit.VIS_ID);
                        command.Parameters.AddWithValue("@VIS_CODE", visit.VIS_CODE);
                        command.Parameters.AddWithValue("@VIS_NAME", (object?)visit.VIS_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_DATE", visit.VIS_DATE != default ? visit.VIS_DATE.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_TYPE", (object?)visit.VIS_TYPE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_TIME", visit.VIS_TIME != default ? visit.VIS_TIME.ToTimeSpan() : DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", visit.CUST_ID);
                        command.Parameters.AddWithValue("@CLI_ID", visit.CLI_ID);
                        command.Parameters.AddWithValue("@APO_ID", visit.APO_ID);
                        command.Parameters.AddWithValue("@DOC_ID", visit.DOC_ID);
                        command.Parameters.AddWithValue("@SERLIST_ID", visit.SERLIST_ID);

                        // إضافة البرامترات الجديدة
                        command.Parameters.AddWithValue("@VIS_PRICE", visit.VIS_PRICE);
                        command.Parameters.AddWithValue("@VIS_DISCOUNT", visit.VIS_DISCOUNT);
                        command.Parameters.AddWithValue("@VIS_TOTAL", visit.VIS_TOTAL);
                        command.Parameters.AddWithValue("@VIS_PAY_TYPE", (object?)visit.VIS_PAY_TYPE ?? DBNull.Value);

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

        #region 6. حذف زيارة (Delete)

        public async Task<bool> DeleteVisitAsync(long visitId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.VIST_TBL WHERE VIS_ID = @VIS_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VIS_ID", visitId);

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

        #region 7. جلب الزيارات بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsVist>> GetVisitsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsVist> visitsList = new List<clsVist>();
            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchVisits(@SearchValue)
                            ORDER BY VIS_ID
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
                                visitsList.Add(MapDataReaderToVisit(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return visitsList;
        }

        #endregion

        #region 8. جلب إجمالي عدد الزيارات بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalVisitsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;
            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalVisitsCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الزيارات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 9. جلب أكبر معرف/كود زيارة مسجلة (GetMaxVisitId)

        public async Task<long> GetMaxVisitIdAsync()
        {
            long maxId = 0;
            string query = @"SELECT dbo.fn_GetMaxVisitId()";

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
