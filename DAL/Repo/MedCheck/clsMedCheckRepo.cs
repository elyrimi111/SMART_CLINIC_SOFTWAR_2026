using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Med_Check;
using DAL.ConnectionString;

namespace DAL.Repo.MedCheck
{
    public class clsMedCheckRepo
    {
        #region Helper Methods 

        private clsMed_Check MapDataReaderToMedCheck(SqlDataReader reader)
        {
            return new clsMed_Check
            {
                MEDCHECK_ID = Convert.ToInt64(reader["MEDCHECK_ID"]),
                MEDCHECK_CODE = reader["MEDCHECK_CODE"] != DBNull.Value ? Convert.ToInt64(reader["MEDCHECK_CODE"]) : 0,
                MEDCHECK_NAME = reader["MEDCHECK_NAME"] != DBNull.Value ? reader["MEDCHECK_NAME"].ToString()! : string.Empty,
                MEDCHECK_TYPE = reader["MEDCHECK_TYPE"] != DBNull.Value ? reader["MEDCHECK_TYPE"].ToString()! : string.Empty,
                MEDCHECK_PRICE = reader["MEDCHECK_PRICE"] != DBNull.Value ? Convert.ToDecimal(reader["MEDCHECK_PRICE"]) : null,
                MEDCHECK_NOTE = reader["MEDCHECK_NOTE"] != DBNull.Value ? reader["MEDCHECK_NOTE"].ToString()! : string.Empty,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region GetAll

        public async Task<List<clsMed_Check>> GetAllMedChecksAsync()
        {
            List<clsMed_Check> medCheckList = new List<clsMed_Check>();

            string query = @"SELECT * FROM dbo.fn_GetAllMedChecks()";

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
                                medCheckList.Add(MapDataReaderToMedCheck(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medCheckList;
        }

        #endregion

        #region (GetById)

        public async Task<clsMed_Check?> GetMedCheckByIdAsync(long? medCheckId)
        {
            clsMed_Check? medCheck = null;

            string query = @"SELECT * FROM dbo.fn_GetMedCheckById(@MEDCHECK_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MEDCHECK_ID", (object?)medCheckId ?? DBNull.Value);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                medCheck = MapDataReaderToMedCheck(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medCheck;
        }

        #endregion

        #region (GetByClinic)

        public async Task<List<clsMed_Check>> GetMedChecksByClinicAsync(long clinicId)
        {
            List<clsMed_Check> medCheckList = new List<clsMed_Check>();

            string query = @"SELECT * FROM dbo.fn_GetMedChecksByClinic(@CLI_ID)";

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
                                medCheckList.Add(MapDataReaderToMedCheck(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medCheckList;
        }

        #endregion

        #region (Insert)

        public async Task<long> AddMedCheckAsync(clsMed_Check medCheck)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.MEDCHECK_TBL 
                            (MEDCHECK_CODE, MEDCHECK_NAME, MEDCHECK_TYPE, MEDCHECK_PRICE, MEDCHECK_NOTE, CLI_ID) 
                            VALUES 
                            (@MEDCHECK_CODE, @MEDCHECK_NAME, @MEDCHECK_TYPE, @MEDCHECK_PRICE, @MEDCHECK_NOTE, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MEDCHECK_CODE", medCheck.MEDCHECK_CODE != 0 ? medCheck.MEDCHECK_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@MEDCHECK_NAME", !string.IsNullOrEmpty(medCheck.MEDCHECK_NAME) ? medCheck.MEDCHECK_NAME : DBNull.Value);
                        command.Parameters.AddWithValue("@MEDCHECK_TYPE", !string.IsNullOrEmpty(medCheck.MEDCHECK_TYPE) ? medCheck.MEDCHECK_TYPE : DBNull.Value);

                        // تم التعديل هنا لعدم استخدام decimal.TryParse
                        command.Parameters.AddWithValue("@MEDCHECK_PRICE", (object?)medCheck.MEDCHECK_PRICE ?? DBNull.Value);

                        command.Parameters.AddWithValue("@MEDCHECK_NOTE", !string.IsNullOrEmpty(medCheck.MEDCHECK_NOTE) ? medCheck.MEDCHECK_NOTE : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", medCheck.CLI_ID != 0 ? medCheck.CLI_ID : DBNull.Value);

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

        #region (Update)

        public async Task<bool> UpdateMedCheckAsync(clsMed_Check medCheck)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.MEDCHECK_TBL SET 
                            MEDCHECK_CODE = @MEDCHECK_CODE, 
                            MEDCHECK_NAME = @MEDCHECK_NAME, 
                            MEDCHECK_TYPE = @MEDCHECK_TYPE, 
                            MEDCHECK_PRICE = @MEDCHECK_PRICE, 
                            MEDCHECK_NOTE = @MEDCHECK_NOTE, 
                            CLI_ID = @CLI_ID
                            WHERE MEDCHECK_ID = @MEDCHECK_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MEDCHECK_ID", medCheck.MEDCHECK_ID);
                        command.Parameters.AddWithValue("@MEDCHECK_CODE", medCheck.MEDCHECK_CODE != 0 ? medCheck.MEDCHECK_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@MEDCHECK_NAME", !string.IsNullOrEmpty(medCheck.MEDCHECK_NAME) ? medCheck.MEDCHECK_NAME : DBNull.Value);
                        command.Parameters.AddWithValue("@MEDCHECK_TYPE", !string.IsNullOrEmpty(medCheck.MEDCHECK_TYPE) ? medCheck.MEDCHECK_TYPE : DBNull.Value);

                        // تم التعديل هنا لعدم استخدام decimal.TryParse
                        command.Parameters.AddWithValue("@MEDCHECK_PRICE", (object?)medCheck.MEDCHECK_PRICE ?? DBNull.Value);

                        command.Parameters.AddWithValue("@MEDCHECK_NOTE", !string.IsNullOrEmpty(medCheck.MEDCHECK_NOTE) ? medCheck.MEDCHECK_NOTE : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", medCheck.CLI_ID != 0 ? medCheck.CLI_ID : DBNull.Value);

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

        #region (Delete)

        public async Task<bool> DeleteMedCheckAsync(long medCheckId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.MEDCHECK_TBL WHERE MEDCHECK_ID = @MEDCHECK_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MEDCHECK_ID", medCheckId);

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

        #region (GetPaged)

        public async Task<List<clsMed_Check>> GetMedChecksPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsMed_Check> medCheckList = new List<clsMed_Check>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchMedChecks(@SearchValue)
                            ORDER BY MEDCHECK_ID
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
                                medCheckList.Add(MapDataReaderToMedCheck(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medCheckList;
        }

        #endregion

        #region (GetTotalCount)

        public async Task<int> GetTotalMedChecksCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalMedChecksCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الفحوصات الطبية: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region (GetMaxMedCheckId)

        public async Task<long> GetMaxMedCheckIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxMedCheckId()";

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
