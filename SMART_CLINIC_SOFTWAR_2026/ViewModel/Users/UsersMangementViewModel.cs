using BLL.Mangers.Users;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Users
{
    public class UsersMangementViewModel : BaseViewModel
    {
        private readonly UsersManger _usersManger;

        public UsersMangementViewModel()
        {
            _usersManger = new UsersManger();
            UsersList = new ObservableCollection<clsUser>();

            _pageSize = 15;
            _currentPage = 1;

            LoadUsersCommand = new RelayCommand(async param => await GetUsersAsync());
            PageChangedCommand = new RelayCommand(async param => await GetUsersAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddUserCommand = new RelayCommand(async param => await AddUserAsync(), param => CanAddUser());
            DeleteUserCommand = new RelayCommand(async param => await DeleteUserAsync(), param => CanDeleteUser());
            ClearFieldsCommand = new RelayCommand(param => ClearFields());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            await GetUsersAsync();
        }

        #region Properties

        private long _user_ID;
        public long USER_ID
        {
            get => _user_ID;
            set
            {
                if (_user_ID != value)
                {
                    _user_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _user_CODE;
        public long? USER_CODE
        {
            get => _user_CODE;
            set
            {
                if (_user_CODE != value)
                {
                    _user_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _user_NAME = string.Empty;
        public string USER_NAME
        {
            get => _user_NAME;
            set
            {
                if (_user_NAME != value)
                {
                    _user_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _user_PASSWORD = string.Empty;
        public string USER_PASSWORD
        {
            get => _user_PASSWORD;
            set
            {
                if (_user_PASSWORD != value)
                {
                    _user_PASSWORD = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _first_NAME;
        public string? FIRST_NAME
        {
            get => _first_NAME;
            set
            {
                if (_first_NAME != value)
                {
                    _first_NAME = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FULL_NAME));
                }
            }
        }

        private string? _second_NAME;
        public string? SECOND_NAME
        {
            get => _second_NAME;
            set
            {
                if (_second_NAME != value)
                {
                    _second_NAME = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FULL_NAME));
                }
            }
        }

        private string? _last_NAME;
        public string? LAST_NAME
        {
            get => _last_NAME;
            set
            {
                if (_last_NAME != value)
                {
                    _last_NAME = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FULL_NAME));
                }
            }
        }

        private string? _fullName;
        public string FULL_NAME
        {
            get
            {
                var name = $"{FIRST_NAME} {SECOND_NAME} {LAST_NAME}".Trim();
                return string.IsNullOrWhiteSpace(name) ? _fullName ?? string.Empty : name;
            }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged();

                    // تقسيم الاسم الكامل المدخل إلى الأجزاء عند كتابته مباشرة في الواجهة
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var parts = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        FIRST_NAME = parts.Length > 0 ? parts[0] : string.Empty;
                        SECOND_NAME = parts.Length > 1 ? parts[1] : string.Empty;
                        LAST_NAME = parts.Length > 2 ? string.Join(" ", parts.Skip(2)) : string.Empty;
                    }
                    else
                    {
                        FIRST_NAME = string.Empty;
                        SECOND_NAME = string.Empty;
                        LAST_NAME = string.Empty;
                    }
                }
            }
        }


        private long? _cli_ID;
        public long? CLI_ID
        {
            get => _cli_ID;
            set
            {
                if (_cli_ID != value)
                {
                    _cli_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _rol_ID;
        public long? ROL_ID
        {
            get => _rol_ID;
            set
            {
                if (_rol_ID != value)
                {
                    _rol_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _status = true;
        public bool STATUS
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

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
                    FillFieldsFromSelectedUser();
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
                    CurrentPage = 1;
                    _ = GetUsersAsync();
                }
            }
        }

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
                    _ = GetUsersAsync();
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

        #region Commands

        public ICommand LoadUsersCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(USER_NAME) ||
                   string.IsNullOrWhiteSpace(USER_PASSWORD);
        }

        private async Task GetUsersAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    TotalRows = _usersManger.GetTotalUsersCount(SearchQuery);
                    var pagedUsers = _usersManger.GetUserPaged(CurrentPage, PageSize, SearchQuery) ?? new List<clsUser>();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UsersList = new ObservableCollection<clsUser>(pagedUsers);
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات المستخدمين", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanAddUser()
        {
            return !IsEmptyInputs() && SelectedUser == null;
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null && SelectedUser.USER_ID > 0;
        }

        private async Task AddUserAsync()
        {
            try
            {
                var newUser = BuildUserFromProperties();
                long insertedId = 0;

                await Task.Run(() =>
                {
                    insertedId = _usersManger.CreateUser(newUser);
                });

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة المستخدم بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetUsersAsync();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteUserAsync()
        {
            if (USER_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا المستخدم؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = false;

                    await Task.Run(() =>
                    {
                        isSuccess = _usersManger.RemoveUser(USER_ID);
                    });

                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف المستخدم بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetUsersAsync();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedUser()
        {
            if (SelectedUser == null) return;

            USER_ID = SelectedUser.USER_ID;
            USER_CODE = SelectedUser.USER_CODE;
            USER_NAME = SelectedUser.USER_NAME;
            USER_PASSWORD = SelectedUser.USER_PASSWORD;
            FIRST_NAME = SelectedUser.FIRST_NAME;
            SECOND_NAME = SelectedUser.SECOND_NAME;
            LAST_NAME = SelectedUser.LAST_NAME;
            CLI_ID = SelectedUser.CLI_ID;
            ROL_ID = SelectedUser.ROL_ID;
            STATUS = SelectedUser.STATUS;
        }

        private clsUser BuildUserFromProperties()
        {
            return new clsUser
            {
                USER_ID = this.USER_ID,
                USER_CODE = this.USER_CODE,
                USER_NAME = this.USER_NAME ?? string.Empty,
                USER_PASSWORD = this.USER_PASSWORD ?? string.Empty,
                FIRST_NAME = this.FIRST_NAME,
                SECOND_NAME = this.SECOND_NAME,
                LAST_NAME = this.LAST_NAME,
                CLI_ID = this.CLI_ID,
                ROL_ID = this.ROL_ID,
                STATUS = this.STATUS
            };
        }

        private void ClearFields()
        {
            USER_ID = 0;
            USER_CODE = null;
            USER_NAME = string.Empty;
            USER_PASSWORD = string.Empty;
            FIRST_NAME = string.Empty;
            SECOND_NAME = string.Empty;
            LAST_NAME = string.Empty;
            CLI_ID = null;
            ROL_ID = null;
            STATUS = true;
            SelectedUser = null;
        }

        #endregion
    }
}
