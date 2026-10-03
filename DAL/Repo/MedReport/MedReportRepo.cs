using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Med_Report;
using DAL.ConnectionString;

namespace DAL.Repo.MedReport
{
    public class clsMedReportRepo
    {
        #region Helper Methods 

        private clsMed_Report MapDataReaderToMedReport(SqlDataReader reader)
        {
            return new clsMed_Report
            {
                MREP_ID = Convert.ToInt64(reader["MREP_ID"]),
                MREP_CODE = reader["MREP_CODE"] != DBNull.Value ? Convert.ToInt64(reader["MREP_CODE"]) : 0,
                MREP_DATE = reader["MREP_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["MREP_DATE"]) : (DateTime?)null,
                MREP_NAME = reader["MREP_NAME"] != DBNull.Value ? reader["MREP_NAME"].ToString()! : string.Empty,
                MREP_TIME = reader["MREP_TIME"] != DBNull.Value ? (TimeSpan)reader["MREP_TIME"] : (TimeSpan?)null,
                MREP_TEXT = reader["MREP_TEXT"] != DBNull.Value ? reader["MREP_TEXT"].ToString()! : string.Empty,
                MREP_NOTE = reader["MREP_NOTE"] != DBNull.Value ? reader["MREP_NOTE"].ToString()! : string.Empty,
                CUST_ID = reader["CUST_ID"] != DBNull.Value ? Convert.ToInt64(reader["CUST_ID"]) : 0,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0,
                VIS_ID = reader["VIS_ID"] != DBNull.Value ? Convert.ToInt64(reader["VIS_ID"]) : 0
            };
        }

        #endregion

        #region GetAll

        public async Task<List<clsMed_Report>> GetAllMedReportsAsync()
        {
            List<clsMed_Report> medReportsList = new List<clsMed_Report>();

            string query = @"SELECT * FROM dbo.fn_GetAllMedReports()";

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
                                medReportsList.Add(MapDataReaderToMedReport(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medReportsList;
        }

        #endregion

        #region (GetById)

        public async Task<clsMed_Report?> GetMedReportByIdAsync(long? mrepId)
        {
            clsMed_Report? medReport = null;

            string query = @"SELECT * FROM dbo.fn_GetMedReportById(@MREP_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MREP_ID", mrepId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                medReport = MapDataReaderToMedReport(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medReport;
        }

        #endregion

        #region (GetByClinic)

        public async Task<List<clsMed_Report>> GetMedReportsByClinicAsync(long clinicId)
        {
            List<clsMed_Report> medReportsList = new List<clsMed_Report>();

            string query = @"SELECT * FROM dbo.fn_GetMedReportsByClinic(@CLI_ID)";

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
                                medReportsList.Add(MapDataReaderToMedReport(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medReportsList;
        }

        #endregion

        #region (Insert)

        public async Task<long> AddMedReportAsync(clsMed_Report medReport)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.MEDREP_TBL 
                            (MREP_CODE, MREP_DATE, MREP_NAME, MREP_TIME, MREP_TEXT, MREP_NOTE, CUST_ID, CLI_ID, VIS_ID) 
                            VALUES 
                            (@MREP_CODE, @MREP_DATE, @MREP_NAME, @MREP_TIME, @MREP_TEXT, @MREP_NOTE, @CUST_ID, @CLI_ID, @VIS_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MREP_CODE", (object?)medReport.MREP_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_DATE", (object?)medReport.MREP_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_NAME", (object?)medReport.MREP_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_TIME", (object?)medReport.MREP_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_TEXT", (object?)medReport.MREP_TEXT ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_NOTE", (object?)medReport.MREP_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)medReport.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)medReport.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_ID", (object?)medReport.VIS_ID ?? DBNull.Value);

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

        public async Task<bool> UpdateMedReportAsync(clsMed_Report medReport)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.MEDREP_TBL SET 
                            MREP_CODE = @MREP_CODE, 
                            MREP_DATE = @MREP_DATE, 
                            MREP_NAME = @MREP_NAME, 
                            MREP_TIME = @MREP_TIME, 
                            MREP_TEXT = @MREP_TEXT, 
                            MREP_NOTE = @MREP_NOTE, 
                            CUST_ID = @CUST_ID, 
                            CLI_ID = @CLI_ID,
                            VIS_ID = @VIS_ID
                            WHERE MREP_ID = @MREP_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MREP_ID", medReport.MREP_ID);
                        command.Parameters.AddWithValue("@MREP_CODE", (object?)medReport.MREP_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_DATE", (object?)medReport.MREP_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_NAME", (object?)medReport.MREP_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_TIME", (object?)medReport.MREP_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_TEXT", (object?)medReport.MREP_TEXT ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MREP_NOTE", (object?)medReport.MREP_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)medReport.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)medReport.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_ID", (object?)medReport.VIS_ID ?? DBNull.Value);

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

        public async Task<bool> DeleteMedReportAsync(long mrepId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.MEDREP_TBL WHERE MREP_ID = @MREP_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MREP_ID", mrepId);

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

        public async Task<List<clsMed_Report>> GetMedReportsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsMed_Report> medReportsList = new List<clsMed_Report>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchMedReports(@SearchValue)
                            ORDER BY MREP_ID
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
                                medReportsList.Add(MapDataReaderToMedReport(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medReportsList;
        }

        #endregion

        #region (GetTotalCount)

        public async Task<int> GetTotalMedReportsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalMedReportsCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد التقارير الطبية: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region (GetMaxMedReportId)

        public async Task<long> GetMaxMedReportIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxMedReportId()";

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
