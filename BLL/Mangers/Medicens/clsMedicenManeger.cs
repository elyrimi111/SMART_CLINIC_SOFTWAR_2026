using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entites.Medcin;
using DAL.Repo.Medcines;

namespace BLL.Mangers.Medicens
{
    public class clsMedicenManeger
    {
        private readonly clsMedicinesRepo _medicinesRepo;

        public clsMedicenManeger()
        {
            _medicinesRepo = new clsMedicinesRepo();
        }

        #region 1. جلب كافة الأدوية

        public async Task<List<clsMedcin>> GetAllMedicinesAsync()
        {
            try
            {
                return await _medicinesRepo.GetAllMedicinesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأدوية: " + ex.Message, ex);
            }
        }

        #endregion

        #region 2. جلب دواء برقم المعرف

        public async Task<clsMedcin?> GetMedicineByIdAsync(long medId)
        {
            if (medId <= 0)
            {
                throw new ArgumentException("معرف الدواء غير صالح.");
            }

            try
            {
                return await _medicinesRepo.GetMedicineByIdAsync(medId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الدواء: " + ex.Message, ex);
            }
        }

        #endregion

        #region 3. جلب الأدوية حسب العيادة

        public async Task<List<clsMedcin>> GetMedicinesByClinicAsync(long clinicId)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return await _medicinesRepo.GetMedicinesByClinicAsync(clinicId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأدوية الخاصة بالعيادة: " + ex.Message, ex);
            }
        }

        #endregion

        #region 4. إضافة دواء جديد

        public async Task<long> AddMedicineAsync(clsMedcin medicine)
        {
            ValidateMedicineData(medicine);

            try
            {
                return await _medicinesRepo.AddMedicineAsync(medicine);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الدواء: " + ex.Message, ex);
            }
        }

        #endregion

        #region 5. تعديل بيانات دواء

        public async Task<bool> UpdateMedicineAsync(clsMedcin medicine)
        {
            if (medicine.MED_ID <= 0)
            {
                throw new ArgumentException("معرف الدواء المطلوب تعديله غير صحيح.");
            }

            ValidateMedicineData(medicine);

            try
            {
                return await _medicinesRepo.UpdateMedicineAsync(medicine);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الدواء: " + ex.Message, ex);
            }
        }

        #endregion

        #region 6. حذف دواء

        public async Task<bool> DeleteMedicineAsync(long medId)
        {
            if (medId <= 0)
            {
                throw new ArgumentException("معرف الدواء المراد حذفه غير صالح.");
            }

            try
            {
                return await _medicinesRepo.DeleteMedicineAsync(medId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الدواء: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods - التحقق من صحة البيانات

        private void ValidateMedicineData(clsMedcin medicine)
        {
            if (medicine == null)
            {
                throw new ArgumentNullException(nameof(medicine), "بيانات الدواء فارغة.");
            }

            if (string.IsNullOrWhiteSpace(medicine.MED_NAME))
            {
                throw new Exception("اسم الدواء مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (medicine.MED_NAME.Length > 200)
            {
                throw new Exception("اسم الدواء يجب ألا يتجاوز 200 حرف.");
            }

            if (medicine.MED_PRICE < 0)
            {
                throw new Exception("سعر الدواء لا يمكن أن يكون أقل من صفر.");
            }
        }

        #endregion

        #region 7. جلب الأدوية بنظام الصفحات مع البحث

        /// <summary>
        /// جلب صفحة محددة من الأدوية بناءً على رقم الصفحة وعدد العناصر ونص البحث
        /// </summary>
        public async Task<List<clsMedcin>> GetMedicinesPagedAsync(int pageNumber, int rowsPerPage, string? medicineName = null)
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
                return await _medicinesRepo.GetMedicinesPagedAsync(pageNumber, rowsPerPage, medicineName);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأدوية بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region 8. جلب إجمالي عدد الأدوية مع البحث

        /// <summary>
        /// جلب إجمالي عدد الأدوية المطابقة لنص البحث لحساب عدد الصفحات في الواجهة
        /// </summary>
        public async Task<int> GetTotalMedicinesCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _medicinesRepo.GetTotalMedicinesCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأدوية: " + ex.Message, ex);
            }
        }

        #endregion

        #region Genrate

        public async Task<long> GetNewMed_CodeAsync()
        {
            try
            {
                long maxId = await _medicinesRepo.GetMaxMedicineIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الدواء الجديد: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
