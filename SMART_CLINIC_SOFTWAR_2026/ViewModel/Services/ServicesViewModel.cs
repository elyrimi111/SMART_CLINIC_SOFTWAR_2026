using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BLL.Mangers.Services;
using Core.Entites.Services;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Services
{
    public class ServicesViewModel : INotifyPropertyChanged
    {
        private readonly clsServicesManger _servicesManger;

        public ServicesViewModel()
        {
            _servicesManger = new clsServicesManger();
            ServicesList = new ObservableCollection<clsService>();

            _pageSize = 17;
            _currentPage = 1;

            LoadServicesCommand = new RelayCommand(async param => await GetServicesAsync());
            PageChangedCommand = new RelayCommand(async param => await GetServicesAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddServiceCommand = new RelayCommand(async param => await AddServiceAsync(), param => CanSaveService());
            UpdateServiceCommand = new RelayCommand(async param => await UpdateServiceAsync(), param => CanUpdate());
            DeleteServiceCommand = new RelayCommand(async param => await DeleteServiceAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            // تهيئة البيانات الأولية بشكل غير متزامن
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            SER_CODE = await GetNew_SER_CODEAsync();
            await GetServicesAsync();
        }

        #region Properties (الخصائص)

        private long _ser_ID;
        public long SER_ID
        {
            get => _ser_ID;
            set
            {
                if (_ser_ID != value)
                {
                    _ser_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _ser_CODE;
        public string? SER_CODE
        {
            get => _ser_CODE;
            set
            {
                if (_ser_CODE != value)
                {
                    _ser_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _ser_NAME;
        public string? SER_NAME
        {
            get => _ser_NAME;
            set
            {
                if (_ser_NAME != value)
                {
                    _ser_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _ser_TYPE;
        public string? SER_TYPE
        {
            get => _ser_TYPE;
            set
            {
                if (_ser_TYPE != value)
                {
                    _ser_TYPE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _ser_PRICE;
        public string? SER_PRICE
        {
            get => _ser_PRICE;
            set
            {
                if (_ser_PRICE != value)
                {
                    _ser_PRICE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _ser_NOTE;
        public string? SER_NOTE
        {
            get => _ser_NOTE;
            set
            {
                if (_ser_NOTE != value)
                {
                    _ser_NOTE = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _cli_ID;
        public long CLI_ID
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

        private ObservableCollection<clsService> _servicesList;
        public ObservableCollection<clsService> ServicesList
        {
            get => _servicesList;
            set
            {
                _servicesList = value;
                OnPropertyChanged();
            }
        }

        private clsService? _selectedService;
        public clsService? SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService != value)
                {
                    _selectedService = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedService();
                }
            }
        }

        // --- خصائص التصفح المرقّم والبحث المفتوحة للـ PaginatedGridControl ---

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
                    _ = GetServicesAsync();
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
                    _ = GetServicesAsync();
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

        public ICommand LoadServicesCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddServiceCommand { get; }
        public ICommand UpdateServiceCommand { get; }
        public ICommand DeleteServiceCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(SER_NAME) ||
                   string.IsNullOrWhiteSpace(SER_CODE) ||
                   string.IsNullOrWhiteSpace(SER_PRICE);
        }

        private async Task<string> GetNew_SER_CODEAsync()
        {
            try
            {
                long newCode = await _servicesManger.GetNewSer_CodeAsync();
                return newCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الخدمة", MessageBoxButton.OK, MessageBoxImage.Error);
                return "0";
            }
        }

        public bool AreServicesEqual(clsService s1, clsService s2)
        {
            if (s1 == null || s2 == null) return s1 == s2;

            string json1 = JsonSerializer.Serialize(s1);
            string json2 = JsonSerializer.Serialize(s2);

            return json1 == json2;
        }

        private async Task GetServicesAsync()
        {
            try
            {
                TotalRows = await _servicesManger.GetTotalServicesCountAsync(SearchQuery);

                var pagedServices = await _servicesManger.GetServicesPagedAsync(CurrentPage, PageSize, SearchQuery)
                                   ?? new List<clsService>();

                ServicesList = new ObservableCollection<clsService>(pagedServices);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الخدمات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveService()
        {
            return !IsEmptyInputs() && SelectedService == null;
        }

        private bool CanDelete()
        {
            return SelectedService != null && SelectedService.SER_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedService != null && SelectedService.SER_ID > 0;
        }

        private async Task AddServiceAsync()
        {
            try
            {
                var newService = BuildServiceFromProperties();
                long insertedId = await _servicesManger.AddServiceAsync(newService);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الخدمة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetServicesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateServiceAsync()
        {
            if (AreServicesEqual(SelectedService, BuildServiceFromProperties()))
            {
                MessageBox.Show("لا يوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var serviceToUpdate = BuildServiceFromProperties();
                bool isSuccess = await _servicesManger.UpdateServiceAsync(serviceToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الخدمة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetServicesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteServiceAsync()
        {
            if (SER_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذه الخدمة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _servicesManger.DeleteServiceAsync(SER_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الخدمة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetServicesAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedService()
        {
            if (SelectedService == null) return;

            SER_ID = SelectedService.SER_ID;
            SER_CODE = SelectedService.SER_CODE;
            SER_NAME = SelectedService.SER_NAME;
            SER_TYPE = SelectedService.SER_TYPE;
            SER_PRICE = SelectedService.SER_PRICE;
            SER_NOTE = SelectedService.SER_NOTE;
            CLI_ID = SelectedService.CLI_ID;
        }

        private clsService BuildServiceFromProperties()
        {
            return new clsService
            {
                SER_ID = this.SER_ID,
                SER_CODE = this.SER_CODE ?? string.Empty,
                SER_NAME = this.SER_NAME ?? string.Empty,
                SER_TYPE = this.SER_TYPE ?? string.Empty,
                SER_PRICE = this.SER_PRICE ?? string.Empty,
                SER_NOTE = this.SER_NOTE ?? string.Empty,
                CLI_ID = 4
            };
        }

        private async Task ClearFieldsAsync()
        {
            SER_ID = 0;
            SER_CODE = await GetNew_SER_CODEAsync();
            SER_NAME = string.Empty;
            SER_TYPE = string.Empty;
            SER_PRICE = string.Empty;
            SER_NOTE = string.Empty;
            CLI_ID = 0;
            SelectedService = null;
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
