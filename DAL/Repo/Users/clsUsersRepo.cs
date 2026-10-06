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
        #region Helper 

        private clsUser MapReaderToUser(SqlDataReader reader)
        {
            return new clsUser
            {
                USER_ID = reader["USER_ID"] != DBNull.Value ? Convert.ToInt64(reader["USER_ID"]) : 0,
                USER_CODE = reader["USER_CODE"] != DBNull.Value ? Convert.ToInt64(reader["USER_CODE"]) : null,
                USER_NAME = reader["USER_NAME"] != DBNull.Value ? reader["USER_NAME"].ToString()! : string.Empty,
                USER_PASSWORD = reader["USER_PASSWORD"] != DBNull.Value ? reader["USER_PASSWORD"].ToString()! : string.Empty,
                FIRST_NAME = reader["FIRST_NAME"] != DBNull.Value ? reader["FIRST_NAME"].ToString() : null,
                SECOND_NAME = reader["SECOND_NAME"] != DBNull.Value ? reader["SECOND_NAME"].ToString() : null,
                LAST_NAME = reader["LAST_NAME"] != DBNull.Value ? reader["LAST_NAME"].ToString() : null,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : null,
                ROL_ID = reader["ROL_ID"] != DBNull.Value ? Convert.ToInt64(reader["ROL_ID"]) : null,
                STATUS = reader["STATUS"] != DBNull.Value && Convert.ToBoolean(reader["STATUS"])
            };
        }

        #endregion

        #region CRUD Operations

        public List<clsUser> GetAll()
        {
            var usersList = new List<clsUser>();
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, FIRST_NAME, SECOND_NAME, LAST_NAME, CLI_ID, ROL_ID, STATUS 
                            FROM USERS_TBL";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersList.Add(MapReaderToUser(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء جلب قائمة المستخدمين: " + ex.Message, ex);
                }
            }

            return usersList;
        }

        public List<clsUser> GetAllByClincID(long cliId)
        {
            var usersList = new List<clsUser>();
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, FIRST_NAME, SECOND_NAME, LAST_NAME, CLI_ID, ROL_ID, STATUS 
                            FROM USERS_TBL 
                            WHERE CLI_ID = @CLI_ID";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CLI_ID", cliId);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersList.Add(MapReaderToUser(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء جلب قائمة مستخدمي العيادة: " + ex.Message, ex);
                }
            }

            return usersList;
        }

        public clsUser? GetById(long userId)
        {
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, FIRST_NAME, SECOND_NAME, LAST_NAME, CLI_ID, ROL_ID, STATUS 
                            FROM USERS_TBL 
                            WHERE USER_ID = @UserId";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"خطأ أثناء البحث عن المستخدم برقم {userId}: " + ex.Message, ex);
                }
            }
        }

        public clsUser? GetByUsername(string username)
        {
            string query = @"SELECT USER_ID, USER_CODE, USER_NAME, USER_PASSWORD, FIRST_NAME, SECOND_NAME, LAST_NAME, CLI_ID, ROL_ID, STATUS 
                            FROM USERS_TBL 
                            WHERE USER_NAME = @Username";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"خطأ أثناء البحث عن اسم المستخدم {username}: " + ex.Message, ex);
                }
            }
        }

        public bool ValidateLogin(string username, string password)
        {
            string query = @"SELECT USER_ID 
                            FROM USERS_TBL 
                            WHERE USER_NAME = @UserName 
                              AND USER_PASSWORD = @Password 
                              AND STATUS = 1";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = (object)username ?? DBNull.Value;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, -1).Value = (object)password ?? DBNull.Value;

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value;
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء التحقق من بيانات تسجيل الدخول: " + ex.Message, ex);
                }
            }
        }

        public long Add(clsUser user)
        {
            string query = @"INSERT INTO USERS_TBL (USER_CODE, USER_NAME, USER_PASSWORD, FIRST_NAME, SECOND_NAME, LAST_NAME, CLI_ID, ROL_ID, STATUS)
                            VALUES (@UserCode, @UserName, @UserPassword, @FirstName, @SecondName, @LastName, @CliId, @RolId, @Status);
                            SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@UserCode", SqlDbType.BigInt).Value = user.USER_CODE.HasValue ? user.USER_CODE.Value : DBNull.Value;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = (object)user.USER_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@UserPassword", SqlDbType.VarChar, -1).Value = (object)user.USER_PASSWORD ?? DBNull.Value;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = (object?)user.FIRST_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@SecondName", SqlDbType.NVarChar, 50).Value = (object?)user.SECOND_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 50).Value = (object?)user.LAST_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@CliId", SqlDbType.BigInt).Value = user.CLI_ID.HasValue ? user.CLI_ID.Value : DBNull.Value;
                cmd.Parameters.Add("@RolId", SqlDbType.BigInt).Value = user.ROL_ID.HasValue ? user.ROL_ID.Value : DBNull.Value;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = user.STATUS;

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt64(result) : 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء إضافة المستخدم: " + ex.Message, ex);
                }
            }
        }

        public bool Update(clsUser user)
        {
            string query = @"UPDATE USERS_TBL 
                            SET USER_CODE = @UserCode,
                                USER_NAME = @UserName,
                                USER_PASSWORD = @UserPassword,
                                FIRST_NAME = @FirstName,
                                SECOND_NAME = @SecondName,
                                LAST_NAME = @LastName,
                                CLI_ID = @CliId,
                                ROL_ID = @RolId,
                                STATUS = @Status
                            WHERE USER_ID = @UserId";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = user.USER_ID;
                cmd.Parameters.Add("@UserCode", SqlDbType.BigInt).Value = user.USER_CODE.HasValue ? user.USER_CODE.Value : DBNull.Value;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = (object)user.USER_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@UserPassword", SqlDbType.VarChar, -1).Value = (object)user.USER_PASSWORD ?? DBNull.Value;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = (object?)user.FIRST_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@SecondName", SqlDbType.NVarChar, 50).Value = (object?)user.SECOND_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 50).Value = (object?)user.LAST_NAME ?? DBNull.Value;
                cmd.Parameters.Add("@CliId", SqlDbType.BigInt).Value = user.CLI_ID.HasValue ? user.CLI_ID.Value : DBNull.Value;
                cmd.Parameters.Add("@RolId", SqlDbType.BigInt).Value = user.ROL_ID.HasValue ? user.ROL_ID.Value : DBNull.Value;
                cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = user.STATUS;

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception($"خطأ أثناء تحديث بيانات المستخدم رقم {user.USER_ID}: " + ex.Message, ex);
                }
            }
        }

        public bool Delete(long userId)
        {
            string query = @"DELETE FROM USERS_TBL WHERE USER_ID = @UserId";

            using (SqlConnection conn = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = userId;

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception($"خطأ أثناء حذف المستخدم رقم {userId}: " + ex.Message, ex);
                }
            }
        }

        public List<clsUser> GetUsersPaged(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsUser> usersList = new List<clsUser>();

            using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand command = new SqlCommand("SP_GetUsersPaged", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                command.Parameters.AddWithValue("@SearchQuery", (object?)searchQuery ?? DBNull.Value);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersList.Add(MapReaderToUser(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء جلب صفحة المستخدمين: " + ex.Message, ex);
                }
            }

            return usersList;
        }

        public int GetTotalUsersCount(string? searchQuery = null)
        {
            using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            using (SqlCommand command = new SqlCommand("SP_GetTotalUsersCount", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SearchQuery", (object?)searchQuery ?? DBNull.Value);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("خطأ أثناء جلب إجمالي عدد المستخدمين: " + ex.Message, ex);
                }
            }
        }

        #endregion
    }
}
