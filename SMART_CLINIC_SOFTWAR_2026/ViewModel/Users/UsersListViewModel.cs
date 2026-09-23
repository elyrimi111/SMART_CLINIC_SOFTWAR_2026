using BLL.Mangers.Users;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Users
{
    public class UsersListViewModel : INotifyPropertyChanged
    {
        private readonly UsersManger _usersManger;

        public UsersListViewModel()
        {
            _usersManger = new UsersManger();
            UsersList = new ObservableCollection<clsUser>();

            _pageSize = 15;
            _currentPage = 1;

            LoadUsersCommand = new RelayCommand(param => GetPagedUsers());
            PageChangedCommand = new RelayCommand(param => GetPagedUsers());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            GetPagedUsers();
        }

        #region Properties (الخصائص)

        private ObservableCollection<clsUser> _usersList;
        public ObservableCollection<clsUser> UsersList
        {
            get => _usersList;
            set
            {
                _usersList = value;
                OnPropertyChanged();
            }
        }

        private clsUser? _selectedUser;
        public clsUser? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _searchQuery;
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    CurrentPage = 1; // إعادة الضبط للصفحة الأولى عند كتابة نص بحث جديد
                    GetPagedUsers();
                }
            }
        }

        // --- خصائص التصفح المرقّم (Pagination) ---

        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value && value > 0)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    GetPagedUsers();
                }
            }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set
            {
                _totalRows = value;
                OnPropertyChanged();
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands (الأوامر)

        public ICommand LoadUsersCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }

        #endregion

        #region Methods (العمليات)

        public void GetPagedUsers()
        {
            try
            {
                // جلب إجمالي عدد السجلات المفلترة لتحديث شريط الصفحات
                TotalRows = _usersManger.GetTotalUsersCount(SearchQuery);

                // جلب الصفحة الحالية فقط بناءً على نص البحث
                var pagedList = _usersManger.GetUserPaged(CurrentPage, PageSize, SearchQuery)
                                ?? new List<clsUser>();

                UsersList = new ObservableCollection<clsUser>(pagedList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات المستخدمين", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
