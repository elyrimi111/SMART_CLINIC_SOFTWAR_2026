using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entites.Card;
using DAL.Repo.Card;

namespace BLL.Mangers.Card
{
    public class clsCardManger
    {
        private readonly clsCardsRepo _cardsRepo;

        public clsCardManger()
        {
            _cardsRepo = new clsCardsRepo();
        }

        #region 1. جلب كافة البطائق

        public async Task<List<clsCard>> GetAllCardsAsync()
        {
            try
            {
                return await _cardsRepo.GetAllCardsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة البطائق: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2.GetCardByID

        public static async Task<clsCard?> GetCardByIdAsync(long cardId)
        {
            if (cardId <= 0)
            {
                throw new ArgumentException("معرف البطاقة غير صالح.");
            }

            try
            {
                return await clsCardsRepo.GetCardByIdAsync(cardId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات البطاقة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. جلب البطائق حسب العيادة

        public async Task<List<clsCard>> GetCardsByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _cardsRepo.GetCardsByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بطائق العيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. إضافة بطاقة جديدة

        public async Task<long> AddCardAsync(clsCard card)
        {
            ValidateCardData(card);

            try
            {
                return await _cardsRepo.AddCardAsync(card);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات البطاقة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. تعديل بيانات بطاقة

        public async Task<bool> UpdateCardAsync(clsCard card)
        {
            if (card.CARD_ID <= 0)
            {
                throw new ArgumentException("معرف البطاقة المطلوب تعديلها غير صحيح.");
            }

            ValidateCardData(card);

            try
            {
                return await _cardsRepo.UpdateCardAsync(card);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات البطاقة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 6. حذف بطاقة

        public async Task<bool> DeleteCardAsync(long cardId)
        {
            if (cardId <= 0)
            {
                throw new ArgumentException("معرف البطاقة المراد حذفها غير صالح.");
            }

            try
            {
                return await _cardsRepo.DeleteCardAsync(cardId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف البطاقة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 7. جلب البطائق بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من البطائق بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsCard>> GetCardsPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("رقم الصفحة يجب أن يكون أكبر من صفر.", nameof(pageNumber));
            }

            if (rowsPerPage <= 0)
            {
                throw new ArgumentException("عدد الصفوف في الصفحة يجب أن يكون أكبر من صفر.", nameof(rowsPerPage));
            }

            try
            {
                return await _cardsRepo.GetCardsPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة البطائق بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. جلب إجمالي عدد البطائق مع البحث

        /// <summary>
        /// جلب إجمالي عدد البطائق المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalCardsCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _cardsRepo.GetTotalCardsCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد البطائق: " + ex.Message, ex);
            }
        }

        #endregion

        #region 9. توليد كود تلقائي للبطاقة الجديدة

        public async Task<long> GetNewCard_CodeAsync()
        {
            try
            {
                long maxId = await _cardsRepo.GetMaxCardIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود البطاقة الجديدة: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateCardData(clsCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "بيانات البطاقة فارغة.");
            }

            if (string.IsNullOrWhiteSpace(card.CARD_NAME))
            {
                throw new Exception("اسم البطاقة مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (card.CARD_NAME.Length > 200)
            {
                throw new Exception("اسم البطاقة يجب ألا يتجاوز 200 حرف.");
            }
        }

        #endregion
    }
}
