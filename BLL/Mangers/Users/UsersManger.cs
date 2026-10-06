using Core.Entites.User;
using DAL.Repo.Users;
using System;
using System.Collections.Generic;

namespace BLL.Mangers.Users
{
    public class UsersManger
    {
        private readonly clsUsersRepo _usersRepo;

        public UsersManger()
        {
            _usersRepo = new clsUsersRepo();
        }

        #region Business Logic Methods

        public List<clsUser> GetAllUsers()
        {
            try
            {
                return _usersRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء جلب قائمة المستخدمين: " + ex.Message, ex);
            }
        }

        public List<clsUser> GetAllByClincID(long cliId)
        {
            if (cliId <= 0)
            {
                throw new ArgumentException("معرف العيادة غير صالح.");
            }

            try
            {
                return _usersRepo.GetAllByClincID(cliId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء جلب قائمة المستخدمين للعيادة: " + ex.Message, ex);
            }
        }

        public clsUser? GetUserById(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("معرف المستخدم غير صالح.");
            }

            try
            {
                return _usersRepo.GetById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ أثناء جلب بيانات المستخدم رقم {userId}: " + ex.Message, ex);
            }
        }

        public clsUser? GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("اسم المستخدم لا يمكن أن يكون فارغاً.");
            }

            try
            {
                return _usersRepo.GetByUsername(username.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ أثناء جلب بيانات المستخدم {username}: " + ex.Message, ex);
            }
        }

        public bool ValidateUserLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("اسم المستخدم مطلوب.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("كلمة المرور مطلوبة.");
            }

            try
            {
                return _usersRepo.ValidateLogin(username.Trim(), password);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء التحقق من تسجيل الدخول: " + ex.Message, ex);
            }
        }

        public long CreateUser(clsUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "بيانات المستخدم غير موجودة.");
            }

            if (string.IsNullOrWhiteSpace(user.USER_NAME))
            {
                throw new ArgumentException("اسم المستخدم لا يمكن أن يكون فارغاً.");
            }

            if (string.IsNullOrWhiteSpace(user.USER_PASSWORD))
            {
                throw new ArgumentException("كلمة المرور لا يمكن أن تكون فارغة.");
            }

            try
            {
                user.USER_NAME = user.USER_NAME.Trim();
                return _usersRepo.Add(user);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء إضافة المستخدم: " + ex.Message, ex);
            }
        }

        public bool UpdateUser(clsUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "بيانات المستخدم غير موجودة.");
            }

            if (user.USER_ID <= 0)
            {
                throw new ArgumentException("معرف المستخدم المراد تعديله غير صحيح.");
            }

            if (string.IsNullOrWhiteSpace(user.USER_NAME))
            {
                throw new ArgumentException("اسم المستخدم لا يمكن أن يكون فارغاً.");
            }

            try
            {
                user.USER_NAME = user.USER_NAME.Trim();
                return _usersRepo.Update(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ في BLL أثناء تعديل البيانات للمستخدم رقم {user.USER_ID}: " + ex.Message, ex);
            }
        }

        public bool RemoveUser(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("معرف المستخدم غير صحيح.");
            }

            try
            {
                return _usersRepo.Delete(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ في BLL أثناء حذف المستخدم رقم {userId}: " + ex.Message, ex);
            }
        }

        public List<clsUser> GetUserPaged(int pageNumber, int rowsPerPage, string? searchQuery = null)
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
                return _usersRepo.GetUsersPaged(pageNumber, rowsPerPage, searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء جلب قائمة المستخدمين بنظام الصفحات: " + ex.Message, ex);
            }
        }

        public int GetTotalUsersCount(string? searchQuery = null)
        {
            try
            {
                return _usersRepo.GetTotalUsersCount(searchQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ أثناء حساب إجمالي عدد المستخدمين: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
