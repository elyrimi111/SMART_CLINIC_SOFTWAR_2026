using Core.Entities.Roles;
using DAL.Repo.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Mangers.Role
{
    public class clsRoleManger
    {
        private readonly clsRolesRepo _rolesRepo;

        public clsRoleManger()
        {
            _rolesRepo = new clsRolesRepo();
        }

        #region GetAll

        public static  async Task<List<clsRole>> GetAllRolesAsync()
        {
            try
            {
                return await clsRolesRepo.GetAllRolesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأدوار: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get By Id

        public static async Task<clsRole?> GetRoleByIdAsync(long? rolId)
        {
            if (rolId <= 0)
            {
                throw new ArgumentException("معرف الدور غير صالح.");
            }

            try
            {
                return await clsRolesRepo.GetRoleByIdAsync(rolId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب بيانات الدور: " + ex.Message, ex);
            }
        }

        #endregion

        #region Add New Role

        public async Task<long> AddRoleAsync(clsRole role)
        {
            ValidateRoleData(role);

            try
            {
                return await _rolesRepo.AddRoleAsync(role);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حفظ بيانات الدور: " + ex.Message, ex);
            }
        }

        #endregion

        #region Update Role

        public async Task<bool> UpdateRoleAsync(clsRole role)
        {
            if (role.ROL_ID <= 0)
            {
                throw new ArgumentException("معرف الدور المطلوب تعديله غير صحيح.");
            }

            ValidateRoleData(role);

            try
            {
                return await _rolesRepo.UpdateRoleAsync(role);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء تعديل بيانات الدور: " + ex.Message, ex);
            }
        }

        #endregion

        #region Delete Role

        public async Task<bool> DeleteRoleAsync(long rolId)
        {
            if (rolId <= 0)
            {
                throw new ArgumentException("معرف الدور المراد حذفه غير صالح.");
            }

            try
            {
                return await _rolesRepo.DeleteRoleAsync(rolId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حذف الدور: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Roles Paged Async
        public async Task<List<clsRole>> GetRolesPagedAsync(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return await _rolesRepo.GetRolesPagedAsync(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة الأدوار بنظام الصفحات: " + ex.Message, ex);
            }
        }

        #endregion

        #region Get Total Roles Count

        public async Task<int> GetTotalRolesCountAsync(string? searchQuery = null)
        {
            try
            {
                return await _rolesRepo.GetTotalRolesCountAsync(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب إجمالي عدد الأدوار: " + ex.Message, ex);
            }
        }

        #endregion

        #region Generate New Role Code / ID

        public async Task<long> GetNewRole_CodeAsync()
        {
            try
            {
                long maxId = await _rolesRepo.GetMaxRoleIdAsync();
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء توليد كود الدور الجديد: " + ex.Message, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void ValidateRoleData(clsRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "بيانات الدور فارغة.");
            }

            if (string.IsNullOrWhiteSpace(role.ROL_NAME))
            {
                throw new Exception("اسم الدور مطلوب ولا يمكن أن يكون فارغاً.");
            }

            if (role.ROL_NAME.Length > 100)
            {
                throw new Exception("اسم الدور يجب ألا يتجاوز 100 حرف.");
            }

            if (!string.IsNullOrEmpty(role.ROL_KEY) && role.ROL_KEY.Length > 50)
            {
                throw new Exception("مفتاح الدور (ROL_KEY) يجب ألا يتجاوز 50 حرف.");
            }

            if (!string.IsNullOrEmpty(role.ROL_DESCRIPTION) && role.ROL_DESCRIPTION.Length > 250)
            {
                throw new Exception("وصف الدور يجب ألا يتجاوز 250 حرف.");
            }
        }

        #endregion
    }
}
