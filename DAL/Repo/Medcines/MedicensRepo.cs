using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Medcin;
using DAL.ConnectionString;

namespace DAL.Repo.Medcines
{
    public class clsMedicinesRepo
    {
        #region Helper Methods (قراءة البيانات وتنظيف قيم NULL)

        private clsMedcin MapDataReaderToMedicine(SqlDataReader reader)
        {
            return new clsMedcin
            {
                MED_ID = Convert.ToInt64(reader["MED_ID"]),
                MED_CODE = reader["MED_CODE"] != DBNull.Value ? Convert.ToInt64(reader["MED_CODE"]) : 0,
                MED_NAME = reader["MED_NAME"] != DBNull.Value ? reader["MED_NAME"].ToString()! : string.Empty,
                MED_S_NAME = reader["MED_S_NAME"] != DBNull.Value ? reader["MED_S_NAME"].ToString()! : string.Empty,
                MED_SOURES = reader["MED_SOURES"] != DBNull.Value ? reader["MED_SOURES"].ToString()! : string.Empty,
                MED_PRICE = reader["MED_PRICE"] != DBNull.Value ? Convert.ToDecimal(reader["MED_PRICE"]) : 0m,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region 1. جلب كافة الأدوية (GetAll)

        public async Task<List<clsMedcin>> GetAllMedicinesAsync()
        {
            List<clsMedcin> medicinesList = new List<clsMedcin>();
            string query = @"SELECT * FROM dbo.fn_GetAllMedicines()";

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
                                medicinesList.Add(MapDataReaderToMedicine(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medicinesList;
        }

        #endregion

        #region 2. جلب دواء برقم المعرف (GetById)

        public async Task<clsMedcin?> GetMedicineByIdAsync(long medId)
        {
            clsMedcin? medicine = null;
            string query = @"SELECT * FROM dbo.fn_GetMedicineById(@MED_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MED_ID", medId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                medicine = MapDataReaderToMedicine(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medicine;
        }

        #endregion

        #region 3. جلب الأدوية حسب العيادة (GetByClinic)

        public async Task<List<clsMedcin>> GetMedicinesByClinicAsync(long clinicId)
        {
            List<clsMedcin> medicinesList = new List<clsMedcin>();
            string query = @"SELECT * FROM dbo.fn_GetMedicinesByClinic(@CLI_ID)";

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
                                medicinesList.Add(MapDataReaderToMedicine(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medicinesList;
        }

        #endregion

        #region 4. إضافة دواء جديد (Insert)

        public async Task<long> AddMedicineAsync(clsMedcin medicine)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.MEDCIN_TBL 
                            (MED_CODE, MED_NAME, MED_S_NAME, MED_SOURES, MED_PRICE, CLI_ID) 
                            VALUES 
                            (@MED_CODE, @MED_NAME, @MED_S_NAME, @MED_SOURES, @MED_PRICE, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MED_CODE", (object?)medicine.MED_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_NAME", (object?)medicine.MED_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_S_NAME", (object?)medicine.MED_S_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_SOURES", (object?)medicine.MED_SOURES ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_PRICE", medicine.MED_PRICE);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)medicine.CLI_ID ?? DBNull.Value);

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

        #region 5. تعديل بيانات دواء (Update)

        public async Task<bool> UpdateMedicineAsync(clsMedcin medicine)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.MEDCIN_TBL SET 
                            MED_CODE = @MED_CODE, 
                            MED_NAME = @MED_NAME, 
                            MED_S_NAME = @MED_S_NAME, 
                            MED_SOURES = @MED_SOURES, 
                            MED_PRICE = @MED_PRICE, 
                            CLI_ID = @CLI_ID
                            WHERE MED_ID = @MED_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MED_ID", medicine.MED_ID);
                        command.Parameters.AddWithValue("@MED_CODE", (object?)medicine.MED_CODE ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_NAME", (object?)medicine.MED_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_S_NAME", (object?)medicine.MED_S_NAME ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_SOURES", (object?)medicine.MED_SOURES ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MED_PRICE", medicine.MED_PRICE);
                        command.Parameters.AddWithValue("@CLI_ID", (object?)medicine.CLI_ID ?? DBNull.Value);

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

        #region 6. حذف دواء (Delete)

        public async Task<bool> DeleteMedicineAsync(long medId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.MEDCIN_TBL WHERE MED_ID = @MED_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MED_ID", medId);

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

        #region 7. جلب الأدوية بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsMedcin>> GetMedicinesPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsMedcin> medicinesList = new List<clsMedcin>();
            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchMedicines(@SearchValue)
                            ORDER BY MED_ID
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
                                medicinesList.Add(MapDataReaderToMedicine(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return medicinesList;
        }

        #endregion

        #region 8. جلب إجمالي عدد الأدوية بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalMedicinesCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;
            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalMedicinesCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأدوية: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 9. جلب أكبر معرف/كود دواء مسجل (GetMaxMedicineId)

        public async Task<long> GetMaxMedicineIdAsync()
        {
            long maxId = 0;
            string query = @"SELECT dbo.fn_GetMaxMedicineId()";

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
