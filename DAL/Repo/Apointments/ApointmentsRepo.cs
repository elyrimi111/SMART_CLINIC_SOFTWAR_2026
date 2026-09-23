using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entities.Appointments;
using DAL.ConnectionString;

namespace DAL.Repo.Apointments
{
    public class clsAppointmentsRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsAppointments MapDataReaderToAppointment(SqlDataReader reader)
        {
            return new clsAppointments
            {
                APO_ID = Convert.ToInt64(reader["APO_ID"]),
                APO_CODE = reader["APO_CODE"] != DBNull.Value ? Convert.ToInt64(reader["APO_CODE"]) : (long?)null,
                APO_NAME = reader["APO_NAME"] != DBNull.Value ? reader["APO_NAME"].ToString() : null,
                APO_DATE = reader["APO_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["APO_DATE"]) : (DateTime?)null,
                APO_TIME = reader["APO_TIME"] != DBNull.Value ? (TimeSpan)reader["APO_TIME"] : (TimeSpan?)null,
                APO_NOTE = reader["APO_NOTE"] != DBNull.Value ? reader["APO_NOTE"].ToString() : null,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : (long?)null,
                CUST_ID = reader["CUST_ID"] != DBNull.Value ? Convert.ToInt64(reader["CUST_ID"]) : (long?)null,
                DOC_ID = reader["DOC_ID"] != DBNull.Value ? Convert.ToInt64(reader["DOC_ID"]) : (long?)null,
                CUST_NAME = reader["CUST_NAME"] != DBNull.Value ? reader["CUST_NAME"].ToString() : null,
                DOC_NAME = reader["DOC_NAME"] != DBNull.Value ? reader["DOC_NAME"].ToString() : null
            };
        }


        #endregion

        #region 1. جلب كافة المواعيد (GetAll)

        public async Task<List<clsAppointments>> GetAllAppointmentsAsync()
        {
            List<clsAppointments> appointmentsList = new List<clsAppointments>();

            string query = @"SELECT * FROM dbo.fn_GetAllAppointments()";

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
                                appointmentsList.Add(MapDataReaderToAppointment(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return appointmentsList;
        }

        #endregion

        #region 2. جلب موعد برقم المعرف (GetById)
        public async Task<clsAppointments?> GetAppointmentByIdAsync(long apoId)
        {
            clsAppointments? appointment = null;

            string query = @"SELECT * FROM dbo.fn_GetAppointmentById(@APO_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@APO_ID", apoId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                appointment = MapDataReaderToAppointment(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return appointment;
        }
        #endregion

        #region 3. جلب المواعيد (GetByClinic)
        public async Task<List<clsAppointments>> GetAppointmentsByClinicAsync(long clinicId )
        {
            List<clsAppointments> appointmentsList = new List<clsAppointments>();

            string query = @"SELECT * FROM dbo.fn_GetAppointmentsByClinic(@CLI_ID)";

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
                                appointmentsList.Add(MapDataReaderToAppointment(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return appointmentsList;
        }

        #endregion

        #region 4. إضافة موعد جديد (Insert)

        public async Task<long> AddAppointmentAsync(clsAppointments appointment)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.APO_TBL 
                            (APO_CODE, APO_NAME, APO_DATE, APO_TIME, APO_NOTE, CLI_ID, CUST_ID, DOC_ID) 
                            VALUES 
                            (@APO_CODE, @APO_NAME, @APO_DATE, @APO_TIME, @APO_NOTE, @CLI_ID, @CUST_ID, @DOC_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@APO_CODE", (object?)appointment.APO_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_NAME", (object?)appointment.APO_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_DATE", (object?)appointment.APO_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_TIME", (object?)appointment.APO_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_NOTE", (object?)appointment.APO_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)appointment.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)appointment.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_ID", (object?)appointment.DOC_ID ?? DBNull.Value);

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

        #region 5. تعديل بيانات موعد (Update)

        public async Task<bool> UpdateAppointmentAsync(clsAppointments appointment)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.APO_TBL SET 
                            APO_CODE = @APO_CODE, 
                            APO_NAME = @APO_NAME, 
                            APO_DATE = @APO_DATE, 
                            APO_TIME = @APO_TIME, 
                            APO_NOTE = @APO_NOTE, 
                            CLI_ID = @CLI_ID, 
                            CUST_ID = @CUST_ID, 
                            DOC_ID = @DOC_ID
                            WHERE APO_ID = @APO_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@APO_ID", appointment.APO_ID);
                        command.Parameters.AddWithValue("@APO_CODE", (object?)appointment.APO_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_NAME", (object?)appointment.APO_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_DATE", (object?)appointment.APO_DATE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_TIME", (object?)appointment.APO_TIME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@APO_NOTE", (object?)appointment.APO_NOTE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)appointment.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CUST_ID", (object?)appointment.CUST_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_ID", (object?)appointment.DOC_ID ?? DBNull.Value);

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

        #region 6. حذف موعد (Delete)

        public async Task<bool> DeleteAppointmentAsync(long apoId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.APO_TBL WHERE APO_ID = @APO_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@APO_ID", apoId);

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

        #region 7. جلب المواعيد بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsAppointments>> GetAppointmentsPagedAsync(
            int pageNumber,
            int rowsPerPage,
            string? custName = null,
            string? docName = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            List<clsAppointments> appointmentsList = new List<clsAppointments>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchAppointments(@CUST_NAME, @DOC_NAME, @FROM_DATE, @TO_DATE)
                    ORDER BY APO_ID
                    OFFSET @Offset ROWS 
                    FETCH NEXT @RowsPerPage ROWS ONLY;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_NAME", string.IsNullOrWhiteSpace(custName) ? DBNull.Value : custName);
                        command.Parameters.AddWithValue("@DOC_NAME", string.IsNullOrWhiteSpace(docName) ? DBNull.Value : docName);
                        command.Parameters.AddWithValue("@FROM_DATE", fromDate.HasValue ? fromDate.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                        command.Parameters.AddWithValue("@TO_DATE", toDate.HasValue ? toDate.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value);

                        command.Parameters.AddWithValue("@Offset", offset);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                appointmentsList.Add(MapDataReaderToAppointment(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return appointmentsList;
        }

        #endregion

        #region 8. جلب إجمالي عدد المواعيد بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalAppointmentsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalAppointmentsCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد المواعيد: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 9. جلب أكبر معرف/كود موعد مسجل (GetMaxAppointmentId)

        public async Task<long> GetMaxAppointmentIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxAppointmentId()";

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
