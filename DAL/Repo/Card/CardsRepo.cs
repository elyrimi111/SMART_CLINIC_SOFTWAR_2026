using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Core.Entites.Card;
using DAL.ConnectionString;

namespace DAL.Repo.Card
{
    public class clsCardsRepo
    {
        #region Helper Methods (قراءة البيانات وتحويل الأنواع)

        private static clsCard MapDataReaderToCard(SqlDataReader reader)
        {
            return new clsCard
            {
                CARD_ID = Convert.ToInt64(reader["CARD_ID"]),
                CARD_CODE = reader["CARD_CODE"] != DBNull.Value ? Convert.ToInt64(reader["CARD_CODE"]) : 0,
                CARD_NAME = reader["CARD_NAME"] != DBNull.Value ? reader["CARD_NAME"].ToString()! : string.Empty,
                CARD_DATE = reader["CARD_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["CARD_DATE"]).ToString("yyyy-MM-dd") : string.Empty,
                CARD_STATE = reader["CARD_STATE"] != DBNull.Value ? reader["CARD_STATE"].ToString()! : string.Empty,
                CARD_PER = reader["CARD_PER"] != DBNull.Value ? reader["CARD_PER"].ToString()! : string.Empty,
                CARD_NOTE = reader["CARD_NOTE"] != DBNull.Value ? reader["CARD_NOTE"].ToString()! : string.Empty,
                COM_ID = reader["COM_ID"] != DBNull.Value ? Convert.ToInt64(reader["COM_ID"]) : 0,
                CLI_ID = reader["CLI_ID"] != DBNull.Value ? Convert.ToInt64(reader["CLI_ID"]) : 0
            };
        }

        #endregion

        #region 1. جلب كافة البطائق (GetAll)

        public async Task<List<clsCard>> GetAllCardsAsync()
        {
            List<clsCard> cardsList = new List<clsCard>();
            string query = @"SELECT * FROM dbo.fn_GetAllCards()";

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
                                cardsList.Add(MapDataReaderToCard(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return cardsList;
        }

        #endregion

        #region 2. جلب بطاقة برقم المعرف (GetById)

        public static async Task<clsCard?> GetCardByIdAsync(long cardId)
        {
            clsCard? card = null;
            string query = @"SELECT * FROM dbo.fn_GetCardById(@CARD_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CARD_ID", cardId);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                card = MapDataReaderToCard(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return card;
        }

        #endregion

        #region 2. جلب بطاقة برقم المعرف (GetById)

        public static async Task<clsCard?> GetCardByCustNamAsync(long CUST_ID)
        {
            clsCard? card = null;
            string query = @"SELECT * FROM dbo.fn_GetCardById(@CUST_ID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CUST_ID", CUST_ID);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                card = MapDataReaderToCard(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return card;
        }

        #endregion

        #region 3. جلب البطائق حسب العيادة (GetByClinic)

        public async Task<List<clsCard>> GetCardsByClinicAsync(long clinicId)
        {
            List<clsCard> cardsList = new List<clsCard>();
            string query = @"SELECT * FROM dbo.fn_GetCardsByClinic(@CLI_ID)";

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
                                cardsList.Add(MapDataReaderToCard(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return cardsList;
        }

        #endregion

        #region 4. إضافة بطاقة جديدة (Insert)

        public async Task<long> AddCardAsync(clsCard card)
        {
            long newInsertedId = 0;
            string query = @"INSERT INTO dbo.CARD_TBL 
                            (CARD_CODE, CARD_NAME, CARD_DATE, CARD_STATE, CARD_PER, CARD_NOTE, COM_ID, CLI_ID) 
                            VALUES 
                            (@CARD_CODE, @CARD_NAME, @CARD_DATE, @CARD_STATE, @CARD_PER, @CARD_NOTE, @COM_ID, @CLI_ID);
                            SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CARD_CODE", card.CARD_CODE != 0 ? card.CARD_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_NAME", !string.IsNullOrWhiteSpace(card.CARD_NAME) ? card.CARD_NAME : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_DATE", DateTime.TryParse(card.CARD_DATE, out DateTime dtDate) ? dtDate : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_STATE", bool.TryParse(card.CARD_STATE, out bool bState) ? bState : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_PER", int.TryParse(card.CARD_PER, out int iPer) ? iPer : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_NOTE", !string.IsNullOrWhiteSpace(card.CARD_NOTE) ? card.CARD_NOTE : DBNull.Value);
                        command.Parameters.AddWithValue("@COM_ID", card.COM_ID != 0 ? card.COM_ID : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", card.CLI_ID != 0 ? card.CLI_ID : DBNull.Value);

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

        #region 5. تعديل بيانات بطاقة (Update)

        public async Task<bool> UpdateCardAsync(clsCard card)
        {
            int rowsAffected = 0;
            string query = @"UPDATE dbo.CARD_TBL SET 
                            CARD_CODE = @CARD_CODE, 
                            CARD_NAME = @CARD_NAME, 
                            CARD_DATE = @CARD_DATE, 
                            CARD_STATE = @CARD_STATE, 
                            CARD_PER = @CARD_PER, 
                            CARD_NOTE = @CARD_NOTE, 
                            COM_ID = @COM_ID, 
                            CLI_ID = @CLI_ID
                            WHERE CARD_ID = @CARD_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CARD_ID", card.CARD_ID);
                        command.Parameters.AddWithValue("@CARD_CODE", card.CARD_CODE != 0 ? card.CARD_CODE : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_NAME", !string.IsNullOrWhiteSpace(card.CARD_NAME) ? card.CARD_NAME : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_DATE", DateTime.TryParse(card.CARD_DATE, out DateTime dtDate) ? dtDate : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_STATE", bool.TryParse(card.CARD_STATE, out bool bState) ? bState : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_PER", int.TryParse(card.CARD_PER, out int iPer) ? iPer : DBNull.Value);
                        command.Parameters.AddWithValue("@CARD_NOTE", !string.IsNullOrWhiteSpace(card.CARD_NOTE) ? card.CARD_NOTE : DBNull.Value);
                        command.Parameters.AddWithValue("@COM_ID", card.COM_ID != 0 ? card.COM_ID : DBNull.Value);
                        command.Parameters.AddWithValue("@CLI_ID", card.CLI_ID != 0 ? card.CLI_ID : DBNull.Value);

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

        #region 6. حذف بطاقة (Delete)

        public async Task<bool> DeleteCardAsync(long cardId)
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM dbo.CARD_TBL WHERE CARD_ID = @CARD_ID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsConnectionStringcs.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CARD_ID", cardId);

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

        #region 7. جلب البطائق بنظام الصفحات والبحث (GetPaged)

        public async Task<List<clsCard>> GetCardsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            List<clsCard> cardsList = new List<clsCard>();
            int offset = (pageNumber - 1) * rowsPerPage;

            string query = @"SELECT * FROM dbo.fn_SearchCards(@SearchValue)
                            ORDER BY CARD_ID
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
                                cardsList.Add(MapDataReaderToCard(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return cardsList;
        }

        #endregion

        #region 8. جلب إجمالي عدد البطائق بنظام البحث (GetTotalCount)

        public async Task<int> GetTotalCardsCountAsync(string? searchQuery = null)
        {
            int totalCount = 0;
            string query = @"SELECT TotalCount FROM dbo.fn_GetTotalCardsCount(@SearchValue)";

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
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد البطائق: " + ex.Message, ex);
            }

            return totalCount;
        }

        #endregion

        #region 9. جلب أكبر معرف بطاقة مسجل (GetMaxCardId)

        public async Task<long> GetMaxCardIdAsync()
        {
            long maxId = 0;
            string query = @"SELECT dbo.fn_GetMaxCardId()";

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
