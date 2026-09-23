using Core.Entites.Company;
using DAL.ConnectionString;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repo.Companyes
{
    public class CompanesRepo
    {
        public CompanesRepo()
        {

        }

        public async Task<long> GetNewComCodeAsync()
        {
            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT ISNULL(MAX(COM_CODE), 0) + 1 FROM COMPANY_TBL";

                using (var command = new SqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return result != null && result != DBNull.Value ? Convert.ToInt64(result) : 1;
                }
            }
        }

        public async Task<int> GetTotalCompaniesCountAsync(string? searchQuery = null)
        {
            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT COUNT(1) FROM COMPANY_TBL 
                                WHERE (@Search IS NULL OR COM_NAME LIKE '%' + @Search + '%' OR COM_MOBILE LIKE '%' + @Search + '%')";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Search", (object?)searchQuery ?? DBNull.Value);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public async Task<List<clsCompany>> GetCompaniesPagedAsync(int pageNumber, int pageSize, string? searchQuery = null)
        {
            var list = new List<clsCompany>();

            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_GetCompaniesPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@RowsPerPage", pageSize);
                    command.Parameters.AddWithValue("@CompanyName", (object?)searchQuery ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new clsCompany
                            {
                                COM_ID = Convert.ToInt64(reader["COM_ID"]),
                                COM_CODE = Convert.ToInt64(reader["COM_CODE"]),
                                COM_NAME = reader["COM_NAME"] != DBNull.Value ? reader["COM_NAME"].ToString() : null,
                                COM_ADDRESS = reader["COM_ADDRESS"] != DBNull.Value ? reader["COM_ADDRESS"].ToString() : null,
                                COM_MOBILE = reader["COM_MOBILE"] != DBNull.Value ? reader["COM_MOBILE"].ToString() : null,
                                COM_STATE = reader["COM_STATE"] != DBNull.Value && Convert.ToBoolean(reader["COM_STATE"]),
                                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
                            });
                        }
                    }
                }
            }

            return list;
        }

        public async Task<long> AddCompanyAsync(clsCompany company)
        {
            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO COMPANY_TBL (COM_CODE, COM_NAME, COM_ADDRESS, COM_MOBILE, COM_STATE, CLI_ID)
                                VALUES (@COM_CODE, @COM_NAME, @COM_ADDRESS, @COM_MOBILE, @COM_STATE, @CLI_ID);
                                SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@COM_CODE", company.COM_CODE);
                    command.Parameters.AddWithValue("@COM_NAME", (object?)company.COM_NAME ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_ADDRESS", (object?)company.COM_ADDRESS ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_MOBILE", (object?)company.COM_MOBILE ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_STATE", company.COM_STATE);
                    command.Parameters.AddWithValue("@CLI_ID", company.CLI_ID);

                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt64(result) : 0;
                }
            }
        }

        public async Task<bool> UpdateCompanyAsync(clsCompany company)
        {
            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE COMPANY_TBL 
                                SET COM_CODE = @COM_CODE,
                                    COM_NAME = @COM_NAME,
                                    COM_ADDRESS = @COM_ADDRESS,
                                    COM_MOBILE = @COM_MOBILE,
                                    COM_STATE = @COM_STATE,
                                    CLI_ID = @CLI_ID
                                WHERE COM_ID = @COM_ID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@COM_ID", company.COM_ID);
                    command.Parameters.AddWithValue("@COM_CODE", company.COM_CODE);
                    command.Parameters.AddWithValue("@COM_NAME", (object?)company.COM_NAME ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_ADDRESS", (object?)company.COM_ADDRESS ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_MOBILE", (object?)company.COM_MOBILE ?? DBNull.Value);
                    command.Parameters.AddWithValue("@COM_STATE", company.COM_STATE);
                    command.Parameters.AddWithValue("@CLI_ID", company.CLI_ID);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteCompanyAsync(long comId)
        {
            using (var connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM COMPANY_TBL WHERE COM_ID = @COM_ID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@COM_ID", comId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
