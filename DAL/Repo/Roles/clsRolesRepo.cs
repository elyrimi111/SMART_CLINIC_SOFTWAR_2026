using Core.Entities.Roles;
using DAL.ConnectionString;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repo.Roles
{
    public class clsRolesRepo
    {
        #region Helper Methods 

        private clsRole MapDataReaderToRole(SqlDataReader reader)
        {
            return new clsRole
            {
                ROL_ID = Convert.ToInt64(reader["ROL_ID"]),
                ROL_KEY = reader["ROL_KEY"] != DBNull.Value ? reader["ROL_KEY"].ToString() : null,
                ROL_NAME = reader["ROL_NAME"] != DBNull.Value ? reader["ROL_NAME"].ToString() : null,
                ROL_DESCRIPTION = reader["ROL_DESCRIPTION"] != DBNull.Value ? reader["ROL_DESCRIPTION"].ToString() : null,
                STATUS = reader["STATUS"] != DBNull.Value ? Convert.ToBoolean(reader["STATUS"]) : true
            };
        }

        #endregion

        #region GetAll

        public async Task<List<clsRole>> GetAllRolesAsync()
        {
            List<clsRole> rolesList = new List<clsRole>();

            string query = @"SELECT * FROM dbo.fn_GetAllRoles()";

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
                                rolesList.Add(MapDataReaderToRole(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return rolesList;
        }

        #endregion

        #region (GetById)

        public async Task<clsRole?> GetRoleByIdAsync(long? rolId)
        {
            clsRole? role = null;

            string query = @"SELECT * FROM dbo.fn_GetRoleById(@ROL_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ROL_ID", rolId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                role = MapDataReaderToRole(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return role;
        }

        #endregion

        #region (Insert)

        public async Task<long> AddRoleAsync(clsRole role)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.ROLES_TBL 
                            (ROL_KEY, ROL_NAME, ROL_DESCRIPTION, STATUS) 
                            VALUES 
                            (@ROL_KEY, @ROL_NAME, @ROL_DESCRIPTION, @STATUS);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ROL_KEY", (object?)role.ROL_KEY ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ROL_NAME", (object?)role.ROL_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ROL_DESCRIPTION", (object?)role.ROL_DESCRIPTION ?? DBNull.Value);
                        command.Parameters.AddWithValue("@STATUS", role.STATUS);

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

        public async Task<bool> UpdateRoleAsync(clsRole role)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.ROLES_TBL SET 
                            ROL_KEY = @ROL_KEY, 
                            ROL_NAME = @ROL_NAME, 
                            ROL_DESCRIPTION = @ROL_DESCRIPTION, 
                            STATUS = @STATUS
                            WHERE ROL_ID = @ROL_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ROL_ID", role.ROL_ID);
                        command.Parameters.AddWithValue("@ROL_KEY", (object?)role.ROL_KEY ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ROL_NAME", (object?)role.ROL_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ROL_DESCRIPTION", (object?)role.ROL_DESCRIPTION ?? DBNull.Value);
                        command.Parameters.AddWithValue("@STATUS", role.STATUS);

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

        public async Task<bool> DeleteRoleAsync(long rolId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.ROLES_TBL WHERE ROL_ID = @ROL_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ROL_ID", rolId);

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

        public async Task<List<clsRole>> GetRolesPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsRole> rolesList = new List<clsRole>();

            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchRoles(@SearchValue)
                            ORDER BY ROL_ID
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
                                rolesList.Add(MapDataReaderToRole(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return rolesList;
        }

        #endregion

        #region (GetTotalCount)

        public async Task<int> GetTotalRolesCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;

            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalRolesCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأدوار: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region (GetMaxRoleId)

        public async Task<long> GetMaxRoleIdAsync()
        {
            long maxId = 0;

            string query = @"SELECT dbo.fn_GetMaxRoleId()";

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
