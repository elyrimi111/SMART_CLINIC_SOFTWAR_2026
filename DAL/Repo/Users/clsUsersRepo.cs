using Core.Entites.Doctors;
using Core.Entites.User;
using DAL.ConnectionString;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL.Repo.Users
{
    public class clsUsersRepo
    {
        private readonly string _connectionString = clsConnectionStringcs.ConnectionString;

        #region Helper Mapping Method
        /// <summary>
        /// قراءة البيانات يدوياً بأسلوب الكاست المباشر مع معالجة الـ DBNull
        /// </summary>
        private clsUser MapReaderToUser(SqlDataReader reader)
        {
            var user = new clsUser
            {
                USER_ID = reader["USER_ID"] != DBNull.Value ? Convert.ToInt64(reader["USER_ID"]) : 0,
                USER_CODE = reader["USER_CODE"] != DBNull.Value ? Convert.ToInt64(reader["USER_CODE"]) : 0,
                USER_NAME = reader["USER_NAME"] != DBNull.Value ? reader["USER_NAME"].ToString()! : string.Empty,
                USER_PASSWORD = reader["USER_PASSWORD"] != DBNull.Value ? reader["USER_PASSWORD"].ToString()! : string.Empty,
                USER_TYPE = reader["USER_TYPE"] != DBNull.Value ? reader["USER_TYPE"].ToString()! : string.Empty,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };

            return user;
        }
        #endregion

        #region Manual ADO.NET CRUD Operations (SqlDataReader Only)

        public List<clsUser> GetAll()
        {
            var usersList = new List<clsUser>();
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, USER_TYPE, CLI_ID 
                            FROM USERS_TBL";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader? reader = null;

            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    usersList.Add(MapReaderToUser(reader));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("خطأ أثناء جلب قائمة المستخدمين: " + ex.Message, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }

            return usersList;
        }

        public clsUser? GetById(long userId)
        {
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, USER_TYPE, CLI_ID 
                            FROM USERS_TBL 
                            WHERE USER_ID = @UserId";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader? reader = null;

            try
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    return MapReaderToUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء البحث عن المستخدم برقم {userId}: " + ex.Message, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }


        public clsUser? GetByUsername(string Username)
        {
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, USER_TYPE, CLI_ID 
                            FROM USERS_TBL 
                            WHERE USER_NAME =@Username";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader? reader = null;

            try
            {
                cmd.Parameters.AddWithValue("@Username", Username);

                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    return MapReaderToUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء البحث عن المستخدم برقم {Username}: " + ex.Message, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public bool ValidateLogin(string username, string password, string userType)
        {
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, USER_TYPE, CLI_ID 
                            FROM USERS_TBL 
                            WHERE USER_NAME = @UserName 
                              AND USER_PASSWORD = @Password 
                              AND USER_TYPE = @UserType";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader? reader = null;

            try
            {
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 200).Value = (object)username ?? DBNull.Value;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, -1).Value = (object)password ?? DBNull.Value;
                cmd.Parameters.Add("@UserType", SqlDbType.VarChar, 50).Value = (object)userType ?? DBNull.Value;

                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("خطأ أثناء التحقق من بيانات تسجيل الدخول: " + ex.Message, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public long Add(long userCode, string userName, string userPassword, string userType, long? cliId)
        {
            string query = @"INSERT INTO USERS_TBL (USER_CODE, USER_NAME, USER_PASSWORD, USER_TYPE, CLI_ID)
                            VALUES (@UserCode, @UserName, @UserPassword, @UserType, @CliId);
                            SELECT SCOPE_IDENTITY() AS NewID;";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader? reader = null;

            try
            {
                cmd.Parameters.Add("@UserCode", SqlDbType.BigInt).Value = userCode;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 200).Value = (object)userName ?? DBNull.Value;
                cmd.Parameters.Add("@UserPassword", SqlDbType.VarChar, -1).Value = (object)userPassword ?? DBNull.Value;
                cmd.Parameters.Add("@UserType", SqlDbType.VarChar, 50).Value = (object)userType ?? DBNull.Value;
                cmd.Parameters.Add("@CliId", SqlDbType.BigInt).Value = cliId.HasValue ? cliId.Value : DBNull.Value;

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Convert.ToInt64(reader["NewID"]);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("خطأ أثناء إضافة المستخدم: " + ex.Message, ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public bool Update(long userId, long userCode, string userName, string userPassword, string userType, long? cliId)
        {
            string query = @"UPDATE USERS_TBL 
                            SET USER_CODE = @UserCode,
                                USER_NAME = @UserName,
                                USER_PASSWORD = @UserPassword,
                                USER_TYPE = @UserType,
                                CLI_ID = @CliId
                            WHERE USER_ID = @UserId";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;
                cmd.Parameters.Add("@UserCode", SqlDbType.BigInt).Value = userCode;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 200).Value = (object)userName ?? DBNull.Value;
                cmd.Parameters.Add("@UserPassword", SqlDbType.VarChar, -1).Value = (object)userPassword ?? DBNull.Value;
                cmd.Parameters.Add("@UserType", SqlDbType.VarChar, 50).Value = (object)userType ?? DBNull.Value;
                cmd.Parameters.Add("@CliId", SqlDbType.BigInt).Value = cliId.HasValue ? cliId.Value : DBNull.Value;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء تحديث بيانات المستخدم {userId}: " + ex.Message, ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public bool Delete(long userId)
        {
            string query = @"DELETE FROM USERS_TBL WHERE USER_ID = @UserId";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ أثناء حذف المستخدم {userId}: " + ex.Message, ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public List<clsUser> GetUsersPaged(int pageNumber, int rowsPerPage, string? userName = null)
        {
            List<clsUser> usersList = new List<clsUser>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetUsersPaged", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                        command.Parameters.AddWithValue("@UserName", (object?)userName ?? DBNull.Value);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usersList.Add(MapReaderToUser(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return usersList;
        }

        public int GetTotalUsersCount(string? searchQuery = null)
        {
            int totalCount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    string query = @"SELECT COUNT(*) FROM USERS_TBL 
                             WHERE (@SearchValue IS NULL 
                                 OR @SearchValue = '' 
                                 OR USER_NAME LIKE '%' + @SearchValue + '%' 
                                 OR USER_TYPE LIKE '%' + @SearchValue + '%' 
                                 OR CAST(USER_ID AS VARCHAR) LIKE '%' + @SearchValue + '%')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", (object?)searchQuery ?? DBNull.Value);

                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            totalCount = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return totalCount;
        }

        #endregion
    }
}
