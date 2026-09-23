using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Clinc;
using DAL.ConnectionString;

namespace DAL.Repo.Clinc
{
    public class clsClincRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL والتحويل الآمن)

        private long ParseLongSafe(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value)
                return 0;

            string valStr = dbValue.ToString()?.Trim() ?? string.Empty;

            // التجاهل في حال كانت القيمة النصية NULL أو فارغة
            if (string.Equals(valStr, "NULL", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(valStr))
                return 0;

            return long.TryParse(valStr, out long result) ? result : 0;
        }

        private string ParseStringSafe(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value)
                return string.Empty;

            string valStr = dbValue.ToString()?.Trim() ?? string.Empty;

            if (string.Equals(valStr, "NULL", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            return valStr;
        }

        private clsClinc MapDataReaderToClinc(SqlDataReader reader)
        {
            return new clsClinc
            {
                CLI_ID = ParseLongSafe(reader["CLI_ID"]),
                CLI_CODE = ParseLongSafe(reader["CLI_CODE"]),
                CLI_NAME = ParseStringSafe(reader["CLI_NAME"]),
                CLI_LOC = ParseStringSafe(reader["CLI_LOC"]),
                CLI_NOTE = ParseStringSafe(reader["CLI_NOTE"])
            };
        }

        #endregion

        #region 1. جلب كافة العيادات (GetAll)

        public async Task<List<clsClinc>> GetAllClinicsAsync()
        {
            List<clsClinc> clinicsList = new List<clsClinc>();
            string query = @"SELECT * FROM dbo.CLINC_TBL";

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
                                clinicsList.Add(MapDataReaderToClinc(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return clinicsList;
        }

        #endregion

        #region 2. جلب عيادة برقم المعرف (GetById)

        public async Task<clsClinc?> GetClinicByIdAsync(long? cliId)
        {
            clsClinc? clinic = null;
            string query = @"SELECT CLI_ID, CLI_CODE, CLI_NAME, CLI_LOC, CLI_NOTE 
                            FROM dbo.CLINC_TBL 
                            WHERE CLI_ID = @CLI_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CLI_ID", cliId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                clinic = MapDataReaderToClinc(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return clinic;
        }

        #endregion

        #region 3. إضافة عيادة جديدة (Insert)

        public async Task<long> AddClinicAsync(clsClinc clinic)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.CLINC_TBL 
                            (CLI_CODE, CLI_NAME, CLI_LOC, CLI_NOTE) 
                            VALUES 
                            (@CLI_CODE, @CLI_NAME, @CLI_LOC, @CLI_NOTE);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CLI_CODE", (object?)clinic.CLI_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_NAME", (object?)clinic.CLI_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_LOC", (object?)clinic.CLI_LOC ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_NOTE", (object?)clinic.CLI_NOTE ?? DBNull.Value);

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

        #region 4. تعديل بيانات عيادة (Update)

        public async Task<bool> UpdateClinicAsync(clsClinc clinic)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.CLINC_TBL SET 
                            CLI_CODE = @CLI_CODE, 
                            CLI_NAME = @CLI_NAME, 
                            CLI_LOC = @CLI_LOC, 
                            CLI_NOTE = @CLI_NOTE
                            WHERE CLI_ID = @CLI_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CLI_ID", clinic.CLI_ID);
                        command.Parameters.AddWithValue("@CLI_CODE", (object?)clinic.CLI_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_NAME", (object?)clinic.CLI_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_LOC", (object?)clinic.CLI_LOC ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_NOTE", (object?)clinic.CLI_NOTE ?? DBNull.Value);

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

        #region 5. حذف عيادة (Delete)

        public async Task<bool> DeleteClinicAsync(long cliId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.CLINC_TBL WHERE CLI_ID = @CLI_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CLI_ID", cliId);

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

        #region 6. جلب العيادات بنظام الصفحات (GetPaged)

        public async Task<List<clsClinc>> GetClinicsPagedAsync(int pageNumber, int rowsPerPage, string? clinicName = null)
        {
            List<clsClinc> clinicsList = new List<clsClinc>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetClinicsPaged", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                        command.Parameters.AddWithValue("@ClinicName", (object?)clinicName ?? DBNull.Value);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                clinicsList.Add(MapDataReaderToClinc(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return clinicsList;
        }

        #endregion

        #region 7. جلب إجمالي عدد العيادات بنظام البحث

        public async Task<int> GetTotalClinicsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    string query = @"SELECT COUNT(*) FROM CLINC_TBL 
                                     WHERE (@SearchValue IS NULL 
                                         OR @SearchValue = '' 
                                         OR CLI_NAME LIKE '%' + @SearchValue + '%' 
                                         OR CAST(CLI_ID AS VARCHAR) LIKE '%' + @SearchValue + '%')";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد العيادات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region جلب كود الخدمة التالي (GetNextServiceCode)

        public async Task<long> GetNextServiceCodeAsync()
        {
            long nextCode = 1001;
            string query = @"SELECT dbo.fn_GetNextServiceCode()";

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
                            nextCode = Convert.ToInt64(result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return nextCode;
        }

        #endregion

    }
}
