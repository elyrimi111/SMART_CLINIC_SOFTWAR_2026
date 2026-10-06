using System;
using System.Windows.Input;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Users
{
    public class UsersChoseOptionViewModel : BaseViewModel
    {
        #region  Properties 

        private int _totalUsers;
        private int _activeAccounts;
        private int _onlineUsers;
        private int _definedRolesCount;

        /// <summary>
        /// إجمالي عدد المستخدمين في النظام
        /// </summary>
        public int TotalUsers
        {
            get => _totalUsers;
            set { _totalUsers = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// عدد الحسابات النشطة
        /// </summary>
        public int ActiveAccounts
        {
            get => _activeAccounts;
            set { _activeAccounts = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// عدد المستخدمين المتصلين حالياً
        /// </summary>
        public int OnlineUsers
        {
            get => _onlineUsers;
            set { _onlineUsers = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// عدد الأدوار والصلاحيات المعرفة
        /// </summary>
        public int DefinedRolesCount
        {
            get => _definedRolesCount;
            set { _definedRolesCount = value; OnPropertyChanged(); }
        }

        #endregion

        #region Actions 

        public Action? OnNavigateToUsersList { get; set; }

        public Action? OnNavigateToManageAccount { get; set; }

        public Action? OnNavigateToRolesPermissions { get; set; }

        #endregion

        #region Commands

        public ICommand OpenUsersListCommand { get; }

        public ICommand OpenManageAccountCommand { get; }

        public ICommand OpenRolesPermissionsCommand { get; }

        #endregion

        #region Constructor

        public UsersChoseOptionViewModel()
        {
            OpenUsersListCommand = new RelayCommand(ExecuteOpenUsersList);
            OpenManageAccountCommand = new RelayCommand(ExecuteOpenManageAccount);
            OpenRolesPermissionsCommand = new RelayCommand(ExecuteOpenRolesPermissions);

            LoadUserMetrics();
        }

        #endregion

        #region Private Execution Methods

        private void ExecuteOpenUsersList(object? parameter)
        {
            OnNavigateToUsersList?.Invoke();
        }

        private void ExecuteOpenManageAccount(object? parameter)
        {
            OnNavigateToManageAccount?.Invoke();
        }

        private void ExecuteOpenRolesPermissions(object? parameter)
        {
            OnNavigateToRolesPermissions?.Invoke();
        }

        #endregion

        #region Helper Methods

        private void LoadUserMetrics()
        {
            TotalUsers = 0;
            ActiveAccounts = 0;
            OnlineUsers = 0;
            DefinedRolesCount = 0;
        }

        #endregion
    }
}
