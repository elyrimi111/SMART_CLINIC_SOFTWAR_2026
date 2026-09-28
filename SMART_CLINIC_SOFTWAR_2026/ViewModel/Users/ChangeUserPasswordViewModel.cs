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

        private clsUser _currentUser;
        public clsUser CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand SavePasswordCommand { get; }
        public ICommand CloseWindowCommand { get; }

        #endregion

        #region Constructors

        public ChangeUserPasswordViewModel()
        {
            SavePasswordCommand = new RelayCommand(ExecuteSavePassword, CanExecuteSavePassword);
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);
        }

        public ChangeUserPasswordViewModel(clsUser user) : this()
        {
            CurrentUser = user;
        }

        #endregion

        #region Command Methods

        private bool CanExecuteSavePassword(object parameter)
        {
            return CurrentUser != null;
        }

        private void ExecuteSavePassword(object parameter)
        {
            ErrorMessage = string.Empty;

            if (parameter is object[] passwordBoxes && passwordBoxes.Length == 3)
            {
                var txtCurrent = passwordBoxes[0] as PasswordBox;
                var txtNew = passwordBoxes[1] as PasswordBox;
                var txtConfirm = passwordBoxes[2] as PasswordBox;

                string currentPassword = txtCurrent?.Password;
                string newPassword = txtNew?.Password;
                string confirmPassword = txtConfirm?.Password;

                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    ErrorMessage = "يرجى إدخال كلمة المرور الحالية.";
                    txtCurrent?.Focus();
                    return;
                }

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
                    txtConfirm?.Focus();
                    return;
                }

 
                if (newPassword.Length < 4)
                {
                    ErrorMessage = "يجب أن تحتوي كلمة المرور الجديدة على 4 خانات على الأقل.";
                    txtNew?.Focus();
                    return;
                }

                try
                {
  

                    MessageBox.Show("تم تغيير كلمة المرور بنجاح!", "تأكيد", MessageBoxButton.OK, MessageBoxImage.Information);

                    ExecuteCloseWindow(parameter);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"حدث خطأ أثناء حفظ البيانات: {ex.Message}";
                }
            }
            else
            {
                ErrorMessage = "تعذر قراءة حقول كلمة المرور بشكل صحيح.";
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
