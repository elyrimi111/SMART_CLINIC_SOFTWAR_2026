using Core.Entites.Doctors;
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
      
        public clsUser? GetUserByUsername(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentException("معرف المستخدم غير صالح.");
            }

            try
            {
                return _usersRepo.GetByUsername(UserName);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ أثناء جلب بيانات المستخدم رقم {UserName}: " + ex.Message, ex);
            }
        }

        public bool ValidateUserLogin(string username, string password, string userType)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("اسم المستخدم مطلوب.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("كلمة المرور مطلوبة.");
            }

            if (string.IsNullOrWhiteSpace(userType))
            {
                throw new ArgumentException("نوع المستخدم مطلوب.");
            }

            try
            {
                return _usersRepo.ValidateLogin(username.Trim(), password, userType.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء التحقق من تسجيل الدخول: " + ex.Message, ex);
            }
        }

        public long CreateUser(long userCode, string userName, string userPassword, string userType, long? cliId)
        {
            if (userCode <= 0)
            {
                throw new ArgumentException("كود المستخدم يجب أن يكون رقماً موجباً.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("اسم المستخدم لا يمكن أن يكون فارغاً.");
            }

            if (string.IsNullOrWhiteSpace(userPassword))
            {
                throw new ArgumentException("كلمة المرور لا يمكن أن تكون فارغة.");
            }

            try
            {
                return _usersRepo.Add(userCode, userName.Trim(), userPassword, userType.Trim(), cliId);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ في BLL أثناء إضافة المستخدم: " + ex.Message, ex);
            }
        }

        public bool UpdateUser(long userId, long userCode, string userName, string userPassword, string userType, long? cliId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("معرف المستخدم المراد تعديله غير صحيح.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("اسم المستخدم لا يمكن أن يكون فارغاً.");
            }

            try
            {
                return _usersRepo.Update(userId, userCode, userName.Trim(), userPassword, userType.Trim(), cliId);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ في BLL أثناء تعديل البيانات للمستخدم {userId}: " + ex.Message, ex);
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
                throw new Exception($"حدث خطأ في BLL أثناء حذف المستخدم {userId}: " + ex.Message, ex);
            }
        }

        public List<clsUser> GetUserPaged(int pageNumber, int rowsPerPage, string? userName = null)
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
                return _usersRepo.GetUsersPaged(pageNumber, rowsPerPage, userName);
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
