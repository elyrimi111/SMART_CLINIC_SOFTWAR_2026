using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Doctors;
using DAL.ConnectionString;

namespace DAL.Repo.Dcotors
{
    public class clsDoctorsRepo
    {
        #region Helper Methods 

        private clsDoctors MapDataReaderToDoctor(SqlDataReader reader)
        {
            return new clsDoctors
            {
                DOC_ID = Convert.ToInt64(reader["DOC_ID"]),
                DOC_CODE = reader["DOC_CODE"] != DBNull.Value ? Convert.ToInt64(reader["DOC_CODE"]) : (long?)null,
                DOC_NAME = reader["DOC_NAME"] != DBNull.Value ? reader["DOC_NAME"].ToString() : null,
                DOC_MAJOR = reader["DOC_MAJOR"] != DBNull.Value ? reader["DOC_MAJOR"].ToString() : null,
                DOC_EXP = reader["DOC_EXP"] != DBNull.Value ? reader["DOC_EXP"].ToString() : null,
                DOC_BD = reader["DOC_BD"] != DBNull.Value ? Convert.ToDateTime(reader["DOC_BD"]) : (DateTime?)null,
                DOC_MOBILE = reader["DOC_MOBILE"] != DBNull.Value ? reader["DOC_MOBILE"].ToString() : null,
                DOC_ADDRESS = reader["DOC_ADDRESS"] != DBNull.Value ? reader["DOC_ADDRESS"].ToString() : null,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : (long?)null,
                USER_ID = reader["USER_ID"] != DBNull.Value ? Convert.ToInt64(reader["USER_ID"]) : (long?)null
            };
        }

        #endregion

        #region GetAll

        public async Task<List<clsDoctors>> GetAllDoctorsAsync()
        {
            List<clsDoctors> doctorsList = new List<clsDoctors>();

            string query = @"SELECT * FROM dbo.fn_GetAllDoctors()";

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
                                doctorsList.Add(MapDataReaderToDoctor(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return doctorsList;
        }

        #endregion

        #region (GetById)

        public async Task<clsDoctors?> GetDoctorByIdAsync(long? docId)
        {
            clsDoctors? doctor = null;

            string query = @"SELECT * FROM dbo.fn_GetDoctorById(@DOC_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DOC_ID", docId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                doctor = MapDataReaderToDoctor(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return doctor;
        }

        #endregion

        #region (GetByClinic)

        public async Task<List<clsDoctors>> GetDoctorsByClinicAsync(long clinicId)
        {
            List<clsDoctors> doctorsList = new List<clsDoctors>();

            string query = @"SELECT * FROM dbo.fn_GetDoctorsByClinic(@CLI_ID)";

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
                                doctorsList.Add(MapDataReaderToDoctor(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return doctorsList;
        }

        #endregion

        #region (Insert)

        public async Task<long> AddDoctorAsync(clsDoctors doctor)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.DOCTORS_TBL 
                            (DOC_CODE, DOC_NAME, DOC_MAJOR, DOC_EXP, DOC_BD, DOC_MOBILE, DOC_ADDRESS, CLI_ID, USER_ID) 
                            VALUES 
                            (@DOC_CODE, @DOC_NAME, @DOC_MAJOR, @DOC_EXP, @DOC_BD, @DOC_MOBILE, @DOC_ADDRESS, @CLI_ID, @USER_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DOC_CODE", (object?)doctor.DOC_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_NAME", (object?)doctor.DOC_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_MAJOR", (object?)doctor.DOC_MAJOR ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_EXP", (object?)doctor.DOC_EXP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_BD", (object?)doctor.DOC_BD ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_MOBILE", (object?)doctor.DOC_MOBILE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_ADDRESS", (object?)doctor.DOC_ADDRESS ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)doctor.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@USER_ID", (object?)doctor.USER_ID ?? DBNull.Value);

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

        #region  (Update)

        public async Task<bool> UpdateDoctorAsync(clsDoctors doctor)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.DOCTORS_TBL SET 
                            DOC_CODE = @DOC_CODE, 
                            DOC_NAME = @DOC_NAME, 
                            DOC_MAJOR = @DOC_MAJOR, 
                            DOC_EXP = @DOC_EXP, 
                            DOC_BD = @DOC_BD, 
                            DOC_MOBILE = @DOC_MOBILE, 
                            DOC_ADDRESS = @DOC_ADDRESS, 
                            CLI_ID = @CLI_ID,
                            USER_ID = @USER_ID
                            WHERE DOC_ID = @DOC_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DOC_ID", doctor.DOC_ID);
                        command.Parameters.AddWithValue("@DOC_CODE", (object?)doctor.DOC_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_NAME", (object?)doctor.DOC_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_MAJOR", (object?)doctor.DOC_MAJOR ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_EXP", (object?)doctor.DOC_EXP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_BD", (object?)doctor.DOC_BD ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_MOBILE", (object?)doctor.DOC_MOBILE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DOC_ADDRESS", (object?)doctor.DOC_ADDRESS ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)doctor.CLI_ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@USER_ID", (object?)doctor.USER_ID ?? DBNull.Value);

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

        public async Task<bool> DeleteDoctorAsync(long docId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.DOCTORS_TBL WHERE DOC_ID = @DOC_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DOC_ID", docId);

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

        #region  (GetPaged)

        public async Task<List<clsDoctors>> GetDoctorsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsDoctors> doctorsList = new List<clsDoctors>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchDoctors(@SearchValue)
                            ORDER BY DOC_ID
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
                                doctorsList.Add(MapDataReaderToDoctor(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return doctorsList;
        }

        #endregion

        #region (GetTotalCount)


        public async Task<int> GetTotalDoctorsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalDoctorsCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأطباء: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region (GetMaxDoctorId)

        public async Task<long> GetMaxDoctorIdAsync()
        {
            long maxId = 0;


            string query = @"SELECT dbo.fn_GetMaxDoctorId()";

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
