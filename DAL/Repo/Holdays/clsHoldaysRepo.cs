using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Holday;
using DAL.ConnectionString;

namespace DAL.Repo.Holdays
{
    public class clsHoldaysRepo
    {
        #region Helper Methods

        private clsHolday MapDataReaderToHoliday(SqlDataReader reader)
        {
            return new clsHolday
            {
                HOL_ID = Convert.ToInt64(reader["HOL_ID"]),
                HOL_CODE = reader["HOL_CODE"] != DBNull.Value ? Convert.ToInt64(reader["HOL_CODE"]) : (long?)null,
                HOL_DATE = reader["HOL_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["HOL_DATE"]) : (DateTime?)null,
                HOL_TIME = reader["HOL_TIME"] != DBNull.Value ? (TimeSpan)reader["HOL_TIME"] : (TimeSpan?)null,
                HOL_NAME = reader["HOL_NAME"] != DBNull.Value ? reader["HOL_NAME"].ToString() : null,
                HOL_TEXT = reader["HOL_TEXT"] != DBNull.Value ? reader["HOL_TEXT"].ToString() : null,
                HOL_NOTE = reader["HOL_NOTE"] != DBNull.Value ? reader["HOL_NOTE"].ToString() : null,
                CUST_ID = reader["CUST_ID"] != DBNull.Value ? Convert.ToInt64(reader["CUST_ID"]) : (long?)null,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : (long?)null,
                VIS_ID = reader["VIS_ID"] != DBNull.Value ? Convert.ToInt64(reader["VIS_ID"]) : (long?)null
            };
        }

        #endregion

        #region GetAll

        public async Task<List<clsHolday>> GetAllHolidaysAsync()
        {
            List<clsHolday> holidaysList = new List<clsHolday>();

            string query = @"SELECT * FROM dbo.fn_GetAllHolidays()";

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
                                holidaysList.Add(MapDataReaderToHoliday(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return holidaysList;
        }

        #endregion

        #region (GetById)

        public async Task<clsHolday?> GetHolidayByIdAsync(long? holId)
        {
            clsHolday? holiday = null;

            string query = @"SELECT * FROM dbo.fn_GetHolidayById(@HOL_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HOL_ID", (object?)holId ?? DBNull.Value);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                holiday = MapDataReaderToHoliday(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return holiday;
        }

        #endregion

        #region (GetByClinic)

        public async Task<List<clsHolday>> GetHolidaysByClinicAsync(long clinicId)
        {
            List<clsHolday> holidaysList = new List<clsHolday>();

            string query = @"SELECT * FROM dbo.fn_GetHolidaysByClinic(@CLI_ID)";

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
                                holidaysList.Add(MapDataReaderToHoliday(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return holidaysList;
        }

        #endregion

        #region (Insert)

        public async Task<long> AddHolidayAsync(clsHolday holiday)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.HOLDAY_TBL 
                            (HOL_CODE, HOL_DATE, HOL_TIME, HOL_NAME, HOL_TEXT, HOL_NOTE, CUST_ID, CLI_ID, VIS_ID) 
                            VALUES 
                            (@HOL_CODE, @HOL_DATE, @HOL_TIME, @HOL_NAME, @HOL_TEXT, @HOL_NOTE, @CUST_ID, @CLI_ID, @VIS_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HOL_CODE", (object?)holiday.HOL_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_DATE", (object?)holiday.HOL_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_TIME", (object?)holiday.HOL_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_NAME", (object?)holiday.HOL_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_TEXT", (object?)holiday.HOL_TEXT ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_NOTE", (object?)holiday.HOL_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)holiday.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)holiday.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_ID", (object?)holiday.VIS_ID ?? DBNull.Value);

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

        public async Task<bool> UpdateHolidayAsync(clsHolday holiday)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.HOLDAY_TBL SET 
                            HOL_CODE = @HOL_CODE, 
                            HOL_DATE = @HOL_DATE, 
                            HOL_TIME = @HOL_TIME, 
                            HOL_NAME = @HOL_NAME, 
                            HOL_TEXT = @HOL_TEXT, 
                            HOL_NOTE = @HOL_NOTE, 
                            CUST_ID = @CUST_ID,
                            CLI_ID = @CLI_ID,
                            VIS_ID = @VIS_ID
                            WHERE HOL_ID = @HOL_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HOL_ID", holiday.HOL_ID);
                        command.Parameters.AddWithValue("@HOL_CODE", (object?)holiday.HOL_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_DATE", (object?)holiday.HOL_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_TIME", (object?)holiday.HOL_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_NAME", (object?)holiday.HOL_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_TEXT", (object?)holiday.HOL_TEXT ?? DBNull.Value);
                        command.Parameters.AddWithValue("@HOL_NOTE", (object?)holiday.HOL_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)holiday.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)holiday.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@VIS_ID", (object?)holiday.VIS_ID ?? DBNull.Value);

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

        public async Task<bool> DeleteHolidayAsync(long holId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.HOLDAY_TBL WHERE HOL_ID = @HOL_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HOL_ID", holId);

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

        public async Task<List<clsHolday>> GetHolidaysPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsHolday> holidaysList = new List<clsHolday>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchHolidays(@SearchValue)
                            ORDER BY HOL_ID
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
                                holidaysList.Add(MapDataReaderToHoliday(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return holidaysList;
        }

        #endregion

        #region (GetTotalCount)

        public async Task<int> GetTotalHolidaysCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalHolidaysCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الإجازات: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region (GetMaxHolidayId)

        public async Task<long> GetMaxHolidayIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxHolidayId()";

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
