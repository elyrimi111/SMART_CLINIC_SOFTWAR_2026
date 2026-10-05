using BLL.Mangers.Users;
using Core.CurrentSession;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Users
{
    public class ChangeUserPasswordViewModel : BaseViewModel
    {
        #region Fields & Properties

        private readonly UsersManger _usersManager;

        private clsUser _currentUser;
        public clsUser CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private bool _isCurrentPasswordVerified;
        public bool IsCurrentPasswordVerified
        {
            get => _isCurrentPasswordVerified;
            set
            {
                _isCurrentPasswordVerified = value;
                OnPropertyChanged();
                (SavePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand VerifyPasswordCommand { get; }
        public ICommand SavePasswordCommand { get; }
        public ICommand CloseWindowCommand { get; }

        #endregion

        #region Constructors

        public ChangeUserPasswordViewModel()
        {
            _usersManager = new UsersManger();

            VerifyPasswordCommand = new RelayCommand(ExecuteVerifyPassword);
            SavePasswordCommand = new RelayCommand(ExecuteSavePassword, CanExecuteSavePassword);
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);

            CurrentUser = clsCurrentSectioncs.CurrentUser;
        }

        #endregion

        #region Command Methods

        private void ExecuteVerifyPassword(object parameter)
        {
            ErrorMessage = string.Empty;

            if (parameter is PasswordBox txtCurrent)
            {
                string currentPassword = txtCurrent.Password;

                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    ErrorMessage = "يرجى إدخال كلمة المرور الحالية أولاً.";
                    IsCurrentPasswordVerified = false;
                    txtCurrent.Focus();
                    return;
                }

                if (CurrentUser != null && string.Equals(CurrentUser.USER_PASSWORD, currentPassword))
                {
                    IsCurrentPasswordVerified = true;
                    ErrorMessage = string.Empty;
                }
                else
                {
                    IsCurrentPasswordVerified = false;
                    ErrorMessage = "كلمة المرور الحالية غير صحيحة.";
                    txtCurrent.SelectAll();
                    txtCurrent.Focus();
                }
            }
        }

        private bool CanExecuteSavePassword(object parameter)
        {
            return CurrentUser != null && IsCurrentPasswordVerified;
        }

        private void ExecuteSavePassword(object parameter)
        {
            ErrorMessage = string.Empty;

            if (parameter is object[] passwordBoxes && passwordBoxes.Length == 2)
            {
                var txtNew = passwordBoxes[0] as PasswordBox;
                var txtConfirm = passwordBoxes[1] as PasswordBox;

                string newPassword = txtNew?.Password ?? string.Empty;
                string confirmPassword = txtConfirm?.Password ?? string.Empty;

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ErrorMessage = "يرجى إدخال كلمة المرور الجديدة.";
                    txtNew?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ErrorMessage = "يرجى تأكيد كلمة المرور الجديدة.";
                    txtConfirm?.Focus();
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    ErrorMessage = "كلمة المرور الجديدة غير متطابقة مع التأكيد.";
                    txtConfirm?.SelectAll();
                    txtConfirm?.Focus();
                    return;
                }

                if (newPassword.Length < 3)
                {
                    ErrorMessage = "يجب أن تحتوي كلمة المرور الجديدة على 3 خانات على الأقل.";
                    txtNew?.SelectAll();
                    txtNew?.Focus();
                    return;
                }

                try
                {
                    // تحديث كلمة المرور في كائن المستخدم الحالي
                    CurrentUser.USER_PASSWORD = newPassword;

                    // حفظ التعديلات في قاعدة البيانات
                    bool isUpdated = _usersManager.UpdateUser(CurrentUser);

                    if (isUpdated)
                    {
                        // تحديث الجلسة الحالية بكلمة المرور الجديدة
                        clsCurrentSectioncs.CurrentUser.USER_PASSWORD = newPassword;

                        MessageBox.Show("تم تغيير كلمة المرور بنجاح!", "تأكيد", MessageBoxButton.OK, MessageBoxImage.Information);
                        ExecuteCloseWindow(parameter);
                    }
                    else
                    {
                        ErrorMessage = "فشل تحديث كلمة المرور، يرجى المحاولة لاحقاً.";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"حدث خطأ أثناء حفظ البيانات: {ex.Message}";
                }
            }
        }

        private void ExecuteCloseWindow(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
            else
            {
                foreach (Window win in Application.Current.Windows)
                {
                    if (win.DataContext == this)
                    {
                        win.Close();
                        break;
                    }
                }
            }
        }

        #endregion
    }

    #region Converters

    public class MultiPasswordBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
