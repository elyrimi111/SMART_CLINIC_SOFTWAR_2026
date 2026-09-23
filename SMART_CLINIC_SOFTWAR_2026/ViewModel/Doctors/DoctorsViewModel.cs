using BLL.Mangers.Dcotros;
using Core.Entites.Doctors;
using SMART_CLINIC_SOFTWAR_2026.View.Users;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors
{
    public class DoctorsViewModel : BaseViewModel
    {
        private readonly clsDoctorsManger _doctorsManger;

        public DoctorsViewModel()
        {
            _doctorsManger = new clsDoctorsManger();
            DoctorsList = new ObservableCollection<clsDoctors>();

            _pageSize = 17;
            _currentPage = 1;

            LoadDoctorsCommand = new RelayCommand(async param => await GetDoctorsAsync());
            PageChangedCommand = new RelayCommand(async param => await GetDoctorsAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddDoctorCommand = new RelayCommand(async param => await AddDoctorAsync(), param => CanSaveDoctor());
            UpdateDoctorCommand = new RelayCommand(async param => await UpdateDoctorAsync(), param => CanUpdate());
            DeleteDoctorCommand = new RelayCommand(async param => await DeleteDoctorAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());
            OpenUsersListWindoCommand = new RelayCommand(param => _openUsersListWindo(param));

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            DOC_CODE = await GetNew_DOC_CODEAsync();
            await GetDoctorsAsync();
        }

        #region Properties 

        private long _doc_ID;
        public long DOC_ID
        {
            get => _doc_ID;
            set
            {
                if (_doc_ID != value)
                {
                    _doc_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _doc_CODE;
        public long? DOC_CODE
        {
            get => _doc_CODE;
            set
            {
                if (_doc_CODE != value)
                {
                    _doc_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _doc_NAME;
        public string? DOC_NAME
        {
            get => _doc_NAME;
            set
            {
                if (_doc_NAME != value)
                {
                    _doc_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _doc_MAJOR;
        public string? DOC_MAJOR
        {
            get => _doc_MAJOR;
            set
            {
                if (_doc_MAJOR != value)
                {
                    _doc_MAJOR = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _doc_EXP;
        public string? DOC_EXP
        {
            get => _doc_EXP;
            set
            {
                if (_doc_EXP != value)
                {
                    _doc_EXP = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _doc_BD = DateTime.Now;
        public DateTime? DOC_BD
        {
            get => _doc_BD;
            set
            {
                if (_doc_BD != value)
                {
                    _doc_BD = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _doc_MOBILE;
        public string? DOC_MOBILE
        {
            get => _doc_MOBILE;
            set
            {
                if (_doc_MOBILE != value)
                {
                    _doc_MOBILE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _doc_ADDRESS;
        public string? DOC_ADDRESS
        {
            get => _doc_ADDRESS;
            set
            {
                if (_doc_ADDRESS != value)
                {
                    _doc_ADDRESS = value;
                    OnPropertyChanged();
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

        private string? _user_name;
        public string? USER_NAME
        {
            get => _user_name;
            set
            {
                if (_user_name != value)
                {
                    _user_name = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _user_id;
        public long? USER_ID
        {
            get => _user_id;
            set
            {
                if (_user_id != value)
                {
                    _user_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<clsDoctors> _doctorsList;
        public ObservableCollection<clsDoctors> DoctorsList
        {
            get => _doctorsList;
            set
            {
                _doctorsList = value;
                OnPropertyChanged();
            }
        }

        private clsDoctors? _selectedDoctor;
        public clsDoctors? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (_selectedDoctor != value)
                {
                    _selectedDoctor = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedDoctor();
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
                    _ = GetDoctorsAsync();
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
                    _ = GetDoctorsAsync();
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

        public ICommand LoadDoctorsCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddDoctorCommand { get; }
        public ICommand UpdateDoctorCommand { get; }
        public ICommand DeleteDoctorCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand OpenUsersListWindoCommand { get; }

        #endregion

        #region Methods 

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(DOC_NAME) ||
                   string.IsNullOrWhiteSpace(DOC_MAJOR) ||
                   string.IsNullOrWhiteSpace(DOC_MOBILE) ||
                   string.IsNullOrWhiteSpace(DOC_ADDRESS) ||
                   !DOC_CODE.HasValue ||
                   !USER_ID.HasValue;
        }

        private async Task<long> GetNew_DOC_CODEAsync()
        {
            try
            {
                return await _doctorsManger.GetNewDoc_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الطبيب", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreDoctorsEqual(clsDoctors doc1, clsDoctors doc2)
        {
            if (doc1 == null || doc2 == null) return doc1 == doc2;

            string json1 = JsonSerializer.Serialize(doc1);
            string json2 = JsonSerializer.Serialize(doc2);

            return json1 == json2;
        }

        private async Task GetDoctorsAsync()
        {
            try
            {
                TotalRows = await _doctorsManger.GetTotalDoctorsCountAsync(SearchQuery);

                var pagedDoctors = await _doctorsManger.GetDoctorsPagedAsync(CurrentPage, PageSize, SearchQuery)
                                  ?? new List<clsDoctors>();

                DoctorsList = new ObservableCollection<clsDoctors>(pagedDoctors);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الأطباء", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveDoctor()
        {
            return !IsEmptyInputs() && SelectedDoctor == null;
        }

        private bool CanDelete()
        {
            return SelectedDoctor != null && SelectedDoctor.DOC_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedDoctor != null && SelectedDoctor.DOC_ID > 0;
        }

        private async Task AddDoctorAsync()
        {
            try
            {
                var newDoctor = BuildDoctorFromProperties();
                long insertedId = await _doctorsManger.AddDoctorAsync(newDoctor);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الطبيب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetDoctorsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateDoctorAsync()
        {
            if (AreDoctorsEqual(SelectedDoctor, BuildDoctorFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var doctorToUpdate = BuildDoctorFromProperties();
                bool isSuccess = await _doctorsManger.UpdateDoctorAsync(doctorToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الطبيب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetDoctorsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteDoctorAsync()
        {
            if (DOC_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا الطبيب؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _doctorsManger.DeleteDoctorAsync(DOC_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الطبيب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetDoctorsAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedDoctor()
        {
            if (SelectedDoctor == null) return;

            DOC_ID = SelectedDoctor.DOC_ID;
            DOC_CODE = SelectedDoctor.DOC_CODE;
            DOC_NAME = SelectedDoctor.DOC_NAME;
            DOC_MAJOR = SelectedDoctor.DOC_MAJOR;
            DOC_EXP = SelectedDoctor.DOC_EXP;
            DOC_BD = SelectedDoctor.DOC_BD;
            DOC_MOBILE = SelectedDoctor.DOC_MOBILE;
            DOC_ADDRESS = SelectedDoctor.DOC_ADDRESS;
            USER_ID = SelectedDoctor.USER_ID;
            CLI_ID = SelectedDoctor.CLI_ID;
        }

        private clsDoctors BuildDoctorFromProperties()
        {
            return new clsDoctors
            {
                DOC_ID = this.DOC_ID,
                DOC_CODE = this.DOC_CODE,
                DOC_NAME = this.DOC_NAME ?? string.Empty,
                DOC_MAJOR = this.DOC_MAJOR ?? string.Empty,
                DOC_EXP = this.DOC_EXP ?? string.Empty,
                DOC_BD = this.DOC_BD,
                DOC_MOBILE = this.DOC_MOBILE ?? string.Empty,
                DOC_ADDRESS = this.DOC_ADDRESS ?? string.Empty,
                CLI_ID = 4,
                USER_ID = this.USER_ID
            };
        }

        private async Task ClearFieldsAsync()
        {
            DOC_ID = 0;
            DOC_CODE = await GetNew_DOC_CODEAsync();
            DOC_NAME = string.Empty;
            DOC_MAJOR = string.Empty;
            DOC_EXP = string.Empty;
            DOC_BD = DateTime.Now;
            DOC_MOBILE = string.Empty;
            DOC_ADDRESS = string.Empty;
            CLI_ID = null;
            USER_ID = null;
            USER_NAME = string.Empty;
            SelectedDoctor = null;
        }

        private void _openUsersListWindo(object? parameter)
        {
            UsersListWindow usersWindow = new UsersListWindow();

            if (Application.Current.MainWindow != null)
            {
                usersWindow.Owner = Application.Current.MainWindow;
            }

            bool? result = usersWindow.ShowDialog();

            if (result == true && usersWindow.SelectedUser != null)
            {
                this.USER_ID = usersWindow.SelectedUser.USER_ID;
                this.USER_NAME = usersWindow.SelectedUser.USER_NAME;
            }
        }

        #endregion

    }
}
