using BLL.Mangers.Customers;
using Core.Entites.Customer;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Cards;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Customers
{
    public class CustomersViewModel : INotifyPropertyChanged
    {
        private readonly clsCustomersManager _customersManager;

        public CustomersViewModel()
        {
            _customersManager = new clsCustomersManager();
            CustomersList = new ObservableCollection<clsCust>();

            _pageSize = 17;
            _currentPage = 1;

            LoadCustomersCommand = new RelayCommand(async param => await GetCustomersAsync());
            PageChangedCommand = new RelayCommand(async param => await GetCustomersAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddCustomerCommand = new RelayCommand(async param => await AddCustomerAsync(), param => CanSaveCustomer());
            UpdateCustomerCommand = new RelayCommand(async param => await UpdateCustomerAsync(), param => CanUpdate());
            DeleteCustomerCommand = new RelayCommand(async param => await DeleteCustomerAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());
            OpenCardsWindowCommand = new RelayCommand(Param => OpenCardsWindow());
            // تهيئة البيانات الأولية بشكل غير متزامن
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            CUST_CODE = await GetNew_CUST_CODEAsync();
            await GetCustomersAsync();
        }

        #region Properties (الخصائص)

        private long _cust_ID;
        public long CUST_ID
        {
            get => _cust_ID;
            set
            {
                if (_cust_ID != value)
                {
                    _cust_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _cust_CODE;
        public long? CUST_CODE
        {
            get => _cust_CODE;
            set
            {
                if (_cust_CODE != value)
                {
                    _cust_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_F_NAME;
        public string? CUST_F_NAME
        {
            get => _cust_F_NAME;
            set
            {
                if (_cust_F_NAME != value)
                {
                    _cust_F_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_S_NAME;
        public string? CUST_S_NAME
        {
            get => _cust_S_NAME;
            set
            {
                if (_cust_S_NAME != value)
                {
                    _cust_S_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_T_NAME;
        public string? CUST_T_NAME
        {
            get => _cust_T_NAME;
            set
            {
                if (_cust_T_NAME != value)
                {
                    _cust_T_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_L_NAME;
        public string? CUST_L_NAME
        {
            get => _cust_L_NAME;
            set
            {
                if (_cust_L_NAME != value)
                {
                    _cust_L_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_AGE;
        public string? CUST_AGE
        {
            get => _cust_AGE;
            set
            {
                if (_cust_AGE != value)
                {
                    _cust_AGE = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _cust_BD = DateTime.Now;
        public DateTime? CUST_BD
        {
            get => _cust_BD;
            set
            {
                if (_cust_BD != value)
                {
                    _cust_BD = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_MOBILE1;
        public string? CUST_MOBILE1
        {
            get => _cust_MOBILE1;
            set
            {
                if (_cust_MOBILE1 != value)
                {
                    _cust_MOBILE1 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_MOBILE2;
        public string? CUST_MOBILE2
        {
            get => _cust_MOBILE2;
            set
            {
                if (_cust_MOBILE2 != value)
                {
                    _cust_MOBILE2 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_ADDRESS;
        public string? CUST_ADDRESS
        {
            get => _cust_ADDRESS;
            set
            {
                if (_cust_ADDRESS != value)
                {
                    _cust_ADDRESS = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cust_SAVE_STATE;
        public string? CUST_SAVE_STATE
        {
            get => _cust_SAVE_STATE;
            set
            {
                if (_cust_SAVE_STATE != value)
                {
                    _cust_SAVE_STATE = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _card_ID;
        public long? CARD_ID
        {
            get => _card_ID;
            set
            {
                if (_card_ID != value)
                {
                    _card_ID = value;
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

        private ObservableCollection<clsCust> _customersList;
        public ObservableCollection<clsCust> CustomersList
        {
            get => _customersList;
            set
            {
                _customersList = value;
                OnPropertyChanged();
            }
        }

        private clsCust? _selectedCustomer;
        public clsCust? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer != value)
                {
                    _selectedCustomer = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedCustomer();
                }
            }
        }

        // --- خصائص التصفح المرقّم والبحث المرتبطة بالـ PaginatedGridControl ---

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
                    _ = GetCustomersAsync();
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
                    _ = GetCustomersAsync();
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
        public ICommand LoadCustomersCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand UpdateCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand OpenCardsWindowCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(CUST_F_NAME) ||
                   string.IsNullOrWhiteSpace(CUST_MOBILE1) ||
                   !CUST_CODE.HasValue;
        }

        private async Task<long> GetNew_CUST_CODEAsync()
        {
            try
            {
                return await _customersManager.GetNewCust_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود العميل", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreCustomersEqual(clsCust cust1, clsCust cust2)
        {
            if (cust1 == null || cust2 == null) return cust1 == cust2;

            string json1 = JsonSerializer.Serialize(cust1);
            string json2 = JsonSerializer.Serialize(cust2);

            return json1 == json2;
        }

        private async Task GetCustomersAsync()
        {
            try
            {
                TotalRows = await _customersManager.GetTotalCustomersCountAsync(SearchQuery);

                var pagedCustomers = await _customersManager.GetCustomersPagedAsync(CurrentPage, PageSize, SearchQuery)
                                    ?? new List<clsCust>();

                CustomersList = new ObservableCollection<clsCust>(pagedCustomers);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات العملاء", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveCustomer()
        {
            return !IsEmptyInputs() && SelectedCustomer == null;
        }

        private bool CanDelete()
        {
            return SelectedCustomer != null && SelectedCustomer.CUST_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedCustomer != null && SelectedCustomer.CUST_ID > 0;
        }

        private async Task AddCustomerAsync()
        {
            try
            {
                var newCustomer = BuildCustomerFromProperties();
                long insertedId = await _customersManager.AddCustomerAsync(newCustomer);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة العميل بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCustomersAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateCustomerAsync()
        {
            if (AreCustomersEqual(SelectedCustomer, BuildCustomerFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var customerToUpdate = BuildCustomerFromProperties();
                bool isSuccess = await _customersManager.UpdateCustomerAsync(customerToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات العميل بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCustomersAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteCustomerAsync()
        {
            if (CUST_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا العميل؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _customersManager.DeleteCustomerAsync(CUST_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف العميل بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetCustomersAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedCustomer()
        {
            if (SelectedCustomer == null) return;

            CUST_ID = SelectedCustomer.CUST_ID;
            CUST_CODE = SelectedCustomer.CUST_CODE;
            CUST_F_NAME = SelectedCustomer.CUST_F_NAME;
            CUST_S_NAME = SelectedCustomer.CUST_S_NAME;
            CUST_T_NAME = SelectedCustomer.CUST_T_NAME;
            CUST_L_NAME = SelectedCustomer.CUST_L_NAME;
            CUST_AGE = SelectedCustomer.CUST_AGE;
            CUST_BD = SelectedCustomer.CUST_BD != default ? SelectedCustomer.CUST_BD.ToDateTime(TimeOnly.MinValue) : DateTime.Now;
            CUST_MOBILE1 = SelectedCustomer.CUST_MOBILE1;
            CUST_MOBILE2 = SelectedCustomer.CUST_MOBILE2;
            CUST_ADDRESS = SelectedCustomer.CUST_ADDRESS;
            CUST_SAVE_STATE = SelectedCustomer.CUST_SAVE_STATE;
            CARD_ID = SelectedCustomer.CARD_ID;
            CLI_ID = SelectedCustomer.CLI_ID;
        }

        private clsCust BuildCustomerFromProperties()
        {
            return new clsCust
            {
                CUST_ID = this.CUST_ID,
                CUST_CODE = this.CUST_CODE ?? 0,
                CUST_F_NAME = this.CUST_F_NAME ?? string.Empty,
                CUST_S_NAME = this.CUST_S_NAME ?? string.Empty,
                CUST_T_NAME = this.CUST_T_NAME ?? string.Empty,
                CUST_L_NAME = this.CUST_L_NAME ?? string.Empty,
                CUST_AGE = this.CUST_AGE ?? string.Empty,
                CUST_BD = this.CUST_BD.HasValue ? DateOnly.FromDateTime(this.CUST_BD.Value) : default,
                CUST_MOBILE1 = this.CUST_MOBILE1 ?? string.Empty,
                CUST_MOBILE2 = this.CUST_MOBILE2 ?? string.Empty,
                CUST_ADDRESS = this.CUST_ADDRESS ?? string.Empty,
                CUST_SAVE_STATE = this.CUST_SAVE_STATE ?? string.Empty,
                CARD_ID = this.CARD_ID ?? 0,
                CLI_ID = this.CLI_ID ?? 4
            };
        }

        private async Task ClearFieldsAsync()
        {
            CUST_ID = 0;
            CUST_CODE = await GetNew_CUST_CODEAsync();
            CUST_F_NAME = string.Empty;
            CUST_S_NAME = string.Empty;
            CUST_T_NAME = string.Empty;
            CUST_L_NAME = string.Empty;
            CUST_AGE = string.Empty;
            CUST_BD = DateTime.Now;
            CUST_MOBILE1 = string.Empty;
            CUST_MOBILE2 = string.Empty;
            CUST_ADDRESS = string.Empty;
            CUST_SAVE_STATE = string.Empty;
            CARD_ID = null;
            CLI_ID = null;
            SelectedCustomer = null;
        }


        private void OpenCardsWindow()
        {
            var cardsWindow = new View.Card.CardsListWindow();

            bool? result = cardsWindow.ShowDialog();

            if (result == true && cardsWindow.DataContext is CardListViewModel cardsVm && cardsVm.SelectedCard != null)
            {
                CARD_ID = cardsVm.SelectedCard.CARD_ID;
            }
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
