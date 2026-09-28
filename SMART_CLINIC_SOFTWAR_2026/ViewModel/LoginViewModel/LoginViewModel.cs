using BLL.Mangers.Clinc;
using BLL.Mangers.Users;
using Core.CurrentSession;
using Core.Entites.Clinc;
using Core.Entites.User;
using Microsoft.Data.SqlClient;
using SMART_CLINIC_SOFTWAR_2026.View.MainLayout;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.LoginViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UsersManger _usersManger;
        private readonly clsClincManager _clincManager;

        private ObservableCollection<clsUser> _usersList = new();
        public ObservableCollection<clsUser> UsersList
        {
            get => _usersList;
            set
            {
                _usersList = value;
                OnPropertyChanged();
            }
        }

        #region Properties
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();


                if (!string.IsNullOrWhiteSpace(_username))
                {
                    string UserType = _getUserTypeByUserName(_username);
                    
                    _setUserTypeByUserName(UserType);
                }
            }
        }

        private string _selectedUserType ;
        public string SelectedUserType
        {
            get => _selectedUserType;
            set
            {
                _selectedUserType = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand LoginCommand { get; }
        #endregion

        public LoginViewModel()
        {
            _usersManger = new UsersManger();
            _clincManager = new clsClincManager();
            LoginCommand = new RelayCommand( ExecuteLogin,
                CanExecuteLogin);
            LoadUsers();
        }

        #region Command Methods
        private bool CanExecuteLogin(object? parameter)
        {
            return !IsBusy;
        }

        private async void ExecuteLogin(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "يرجى إدخال اسم المستخدم وكلمة المرور.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                bool isValid = _usersManger.ValidateUserLogin(this.Username, password, SelectedUserType);

                if (isValid)
                {
                    clsCurrentSectioncs.CurrentUser =_usersManger.GetUserByUsername(this.Username);

                    Window? currentWindow = passwordBox != null ? Window.GetWindow(passwordBox) : Application.Current.MainWindow;
                    _navigateToMainWindow(currentWindow);
                }
                else
                {
                    ErrorMessage = "اسم المستخدم أو كلمة المرور أو نوع المستخدم غير صحيح.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        private void LoadUsers()
        {
            var list = _usersManger.GetAllByClincID(clsCurrentSectioncs.CurrentClinc.CLI_ID);
            UsersList = new ObservableCollection<clsUser>(list);
        }
      
        private void _navigateToMainWindow(Window? currentWindow)
        {
            MainLayoutWindo mainLayout = new MainLayoutWindo();
            Application.Current.MainWindow = mainLayout;
            mainLayout.Show();
            currentWindow?.Close();
        }
      
        private string _getUserTypeByUserName(string USER_NAEM)
        {
            clsUser? user = _usersList.FirstOrDefault(u => u.USER_NAME != null && u.USER_NAME.Equals(USER_NAEM, StringComparison.OrdinalIgnoreCase));

            return user != null ? user.USER_TYPE?.Trim().ToUpper() ?? string.Empty : string.Empty;
        }
       
        private void _setUserTypeByUserName(string USER_TYPE)
        {
            if (!string.IsNullOrEmpty(USER_TYPE))
            {
                SelectedUserType = USER_TYPE;
            }
        }
    }
}
