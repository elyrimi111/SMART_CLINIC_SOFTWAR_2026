using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Diagnos;
using DAL.ConnectionString;

namespace DAL.Repo.Diagnos
{
    public class clsDiagnosRepo
    {
        #region Helper Methods (قراءة البيانات وتحويل القيم)

        private clsDiagnos MapDataReaderToDiagnos(SqlDataReader reader)
        {
            return new clsDiagnos
            {
                DIG_ID = Convert.ToInt64(reader["DIG_ID"]),
                DIG_CODE = reader["DIG_CODE"] != DBNull.Value ? Convert.ToInt64(reader["DIG_CODE"]) : 0,
                DIG_NAME = reader["DIG_NAME"] != DBNull.Value ? reader["DIG_NAME"].ToString()! : string.Empty,
                DIG_TYPE = reader["DIG_TYPE"] != DBNull.Value ? reader["DIG_TYPE"].ToString()! : string.Empty,
                DIG_NOTE = reader["DIG_NOTE"] != DBNull.Value ? reader["DIG_NOTE"].ToString()! : string.Empty,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region 1. جلب كافة التشخيصات (GetAll)

        public async Task<List<clsDiagnos>> GetAllDiagnosAsync()
        {
            List<clsDiagnos> diagnosList = new List<clsDiagnos>();

            string query = @"SELECT * FROM dbo.fn_GetAllDiagnosis()";

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
                                diagnosList.Add(MapDataReaderToDiagnos(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return diagnosList;
        }

        #endregion

        #region 2. جلب تشخيص برقم المعرف (GetById)

        public async Task<clsDiagnos?> GetDiagnosByIdAsync(long digId)
        {
            clsDiagnos? diagnos = null;

            string query = @"SELECT * FROM dbo.fn_GetDiagnosisById(@DIG_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DIG_ID", digId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                diagnos = MapDataReaderToDiagnos(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return diagnos;
        }

        #endregion

        #region 3. جلب التشخيصات حسب العيادة (GetByClinic)

        public async Task<List<clsDiagnos>> GetDiagnosByClinicAsync(long clinicId)
        {
            List<clsDiagnos> diagnosList = new List<clsDiagnos>();

            string query = @"SELECT * FROM dbo.fn_GetDiagnosisByClinic(@CLI_ID)";

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
                                diagnosList.Add(MapDataReaderToDiagnos(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return diagnosList;
        }

        #endregion

        #region 4. إضافة تشخيص جديد (Insert)

        public async Task<long> AddDiagnosAsync(clsDiagnos diagnos)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.DIAGNOIS_TABLE 
                            (DIG_CODE, DIG_NAME, DIG_TYPE, DIG_NOTE, CLI_ID) 
                            VALUES 
                            (@DIG_CODE, @DIG_NAME, @DIG_TYPE, @DIG_NOTE, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DIG_CODE", diagnos.DIG_CODE);
                        command.Parameters.AddWithValue("@DIG_NAME", string.IsNullOrEmpty(diagnos.DIG_NAME) ? DBNull.Value : diagnos.DIG_NAME);
                        command.Parameters.AddWithValue("@DIG_TYPE", string.IsNullOrEmpty(diagnos.DIG_TYPE) ? DBNull.Value : diagnos.DIG_TYPE);
                        command.Parameters.AddWithValue("@DIG_NOTE", string.IsNullOrEmpty(diagnos.DIG_NOTE) ? DBNull.Value : diagnos.DIG_NOTE);
                        command.Parameters.AddWithValue("@CLI_ID", diagnos.CLI_ID == 0 ? DBNull.Value : diagnos.CLI_ID);

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

        #region 5. تعديل بيانات تشخيص (Update)

        public async Task<bool> UpdateDiagnosAsync(clsDiagnos diagnos)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.DIAGNOIS_TABLE SET 
                            DIG_CODE = @DIG_CODE, 
                            DIG_NAME = @DIG_NAME, 
                            DIG_TYPE = @DIG_TYPE, 
                            DIG_NOTE = @DIG_NOTE, 
                            CLI_ID = @CLI_ID
                            WHERE DIG_ID = @DIG_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DIG_ID", diagnos.DIG_ID);
                        command.Parameters.AddWithValue("@DIG_CODE", diagnos.DIG_CODE);
                        command.Parameters.AddWithValue("@DIG_NAME", string.IsNullOrEmpty(diagnos.DIG_NAME) ? DBNull.Value : diagnos.DIG_NAME);
                        command.Parameters.AddWithValue("@DIG_TYPE", string.IsNullOrEmpty(diagnos.DIG_TYPE) ? DBNull.Value : diagnos.DIG_TYPE);
                        command.Parameters.AddWithValue("@DIG_NOTE", string.IsNullOrEmpty(diagnos.DIG_NOTE) ? DBNull.Value : diagnos.DIG_NOTE);
                        command.Parameters.AddWithValue("@CLI_ID", diagnos.CLI_ID == 0 ? DBNull.Value : diagnos.CLI_ID);

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

        #region 6. حذف تشخيص (Delete)

        public async Task<bool> DeleteDiagnosAsync(long digId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.DIAGNOIS_TABLE WHERE DIG_ID = @DIG_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DIG_ID", digId);

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

        #region 7. جلب التشخيصات بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsDiagnos>> GetDiagnosPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsDiagnos> diagnosList = new List<clsDiagnos>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchDiagnosis(@SearchValue)
                            ORDER BY DIG_ID
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
                                diagnosList.Add(MapDataReaderToDiagnos(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return diagnosList;
        }

        #endregion

        #region 8. جلب إجمالي عدد التشخيصات بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalDiagnosCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalDiagnosisCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد التشخيصات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 9. جلب أكبر معرف/كود تشخيص مسجل (GetMaxDiagnosId)

        public async Task<long> GetMaxDiagnosIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxDiagnosisId()";

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
