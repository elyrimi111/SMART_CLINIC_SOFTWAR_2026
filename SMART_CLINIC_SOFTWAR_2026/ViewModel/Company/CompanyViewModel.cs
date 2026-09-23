using BLL.Mangers.Companies;
using Core.Entites.Company;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Company
{
    public class CompanyViewModel : INotifyPropertyChanged
    {
        private readonly clsCompanyesManger _companyManager;

        public CompanyViewModel()
        {
            _companyManager = new clsCompanyesManger();
            CompaniesList = new ObservableCollection<clsCompany>();

            _pageSize = 17;
            _currentPage = 1;

            LoadCompaniesCommand = new RelayCommand(async param => await GetCompaniesAsync());
            PageChangedCommand = new RelayCommand(async param => await GetCompaniesAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddCompanyCommand = new RelayCommand(async param => await AddCompanyAsync(), param => CanSaveCompany());
            UpdateCompanyCommand = new RelayCommand(async param => await UpdateCompanyAsync(), param => CanUpdate());
            DeleteCompanyCommand = new RelayCommand(async param => await DeleteCompanyAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            COM_CODE = await GetNew_COM_CODEAsync();
            await GetCompaniesAsync();
        }

        #region Properties (الخصائص)

        private long _com_ID;
        public long COM_ID
        {
            get => _com_ID;
            set
            {
                if (_com_ID != value)
                {
                    _com_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _com_CODE;
        public long? COM_CODE
        {
            get => _com_CODE;
            set
            {
                if (_com_CODE != value)
                {
                    _com_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _com_NAME;
        public string? COM_NAME
        {
            get => _com_NAME;
            set
            {
                if (_com_NAME != value)
                {
                    _com_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _com_ADDRESS;
        public string? COM_ADDRESS
        {
            get => _com_ADDRESS;
            set
            {
                if (_com_ADDRESS != value)
                {
                    _com_ADDRESS = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _com_MOBILE;
        public string? COM_MOBILE
        {
            get => _com_MOBILE;
            set
            {
                if (_com_MOBILE != value)
                {
                    _com_MOBILE = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _com_STATE;
        public bool COM_STATE
        {
            get => _com_STATE;
            set
            {
                if (_com_STATE != value)
                {
                    _com_STATE = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsStateActive));
                    OnPropertyChanged(nameof(IsStateInactive));
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

        private ObservableCollection<clsCompany> _companiesList;
        public ObservableCollection<clsCompany> CompaniesList
        {
            get => _companiesList;
            set
            {
                _companiesList = value;
                OnPropertyChanged();
            }
        }

        private clsCompany? _selectedCompany;
        public clsCompany? SelectedCompany
        {
            get => _selectedCompany;
            set
            {
                if (_selectedCompany != value)
                {
                    _selectedCompany = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedCompany();
                }
            }
        }

        public bool IsStateActive
        {
            get => COM_STATE;
            set
            {
                if (value)
                {
                    COM_STATE = true;
                }
            }
        }

        public bool IsStateInactive
        {
            get => !COM_STATE;
            set
            {
                if (value)
                {
                    COM_STATE = false;
                }
            }
        }

        // --- خصائص التصفح المرقّم والبحث ---

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
                    _ = GetCompaniesAsync();
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
                    _ = GetCompaniesAsync();
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

        public ICommand LoadCompaniesCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddCompanyCommand { get; }
        public ICommand UpdateCompanyCommand { get; }
        public ICommand DeleteCompanyCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(COM_NAME) ||
                   string.IsNullOrWhiteSpace(COM_MOBILE) ||
                   !COM_CODE.HasValue;
        }

        private async Task<long> GetNew_COM_CODEAsync()
        {
            try
            {
                return await _companyManager.GetNewCom_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الشركة", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreCompaniesEqual(clsCompany com1, clsCompany com2)
        {
            if (com1 == null || com2 == null) return com1 == com2;

            string json1 = JsonSerializer.Serialize(com1);
            string json2 = JsonSerializer.Serialize(com2);

            return json1 == json2;
        }

        private async Task GetCompaniesAsync()
        {
            try
            {
                TotalRows = await _companyManager.GetTotalCompaniesCountAsync(SearchQuery);

                var pagedCompanies = await _companyManager.GetCompaniesPagedAsync(CurrentPage, PageSize, SearchQuery)
                                    ?? new List<clsCompany>();

                CompaniesList = new ObservableCollection<clsCompany>(pagedCompanies);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الشركات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveCompany()
        {
            return !IsEmptyInputs() && SelectedCompany == null;
        }

        private bool CanDelete()
        {
            return SelectedCompany != null && SelectedCompany.COM_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedCompany != null && SelectedCompany.COM_ID > 0;
        }

        private async Task AddCompanyAsync()
        {
            try
            {
                var newCompany = BuildCompanyFromProperties();
                long insertedId = await _companyManager.AddCompanyAsync(newCompany);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الشركة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCompaniesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateCompanyAsync()
        {
            if (AreCompaniesEqual(SelectedCompany, BuildCompanyFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var companyToUpdate = BuildCompanyFromProperties();
                bool isSuccess = await _companyManager.UpdateCompanyAsync(companyToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الشركة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCompaniesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteCompanyAsync()
        {
            if (COM_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذه الشركة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _companyManager.DeleteCompanyAsync(COM_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الشركة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetCompaniesAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedCompany()
        {
            if (SelectedCompany == null) return;

            COM_ID = SelectedCompany.COM_ID;
            COM_CODE = SelectedCompany.COM_CODE;
            COM_NAME = SelectedCompany.COM_NAME;
            COM_ADDRESS = SelectedCompany.COM_ADDRESS;
            COM_MOBILE = SelectedCompany.COM_MOBILE;
            COM_STATE = SelectedCompany.COM_STATE;
            CLI_ID = SelectedCompany.CLI_ID;
        }

        private clsCompany BuildCompanyFromProperties()
        {
            return new clsCompany
            {
                COM_ID = this.COM_ID,
                COM_CODE = this.COM_CODE ?? 0,
                COM_NAME = this.COM_NAME ?? string.Empty,
                COM_ADDRESS = this.COM_ADDRESS ?? string.Empty,
                COM_MOBILE = this.COM_MOBILE ?? string.Empty,
                COM_STATE = this.COM_STATE,
                CLI_ID = this.CLI_ID ?? 4
            };
        }

        private async Task ClearFieldsAsync()
        {
            COM_ID = 0;
            COM_CODE = await GetNew_COM_CODEAsync();
            COM_NAME = string.Empty;
            COM_ADDRESS = string.Empty;
            COM_MOBILE = string.Empty;
            COM_STATE = true;
            CLI_ID = null;
            SelectedCompany = null;
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
