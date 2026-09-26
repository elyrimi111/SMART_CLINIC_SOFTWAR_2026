using BLL.Mangers.Customers;
using BLL.Mangers.Orders;
using Core.Entites.Customer;
using Core.Entites.Orders;
using SMART_CLINIC_SOFTWAR_2026.View.Customers;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Orders
{
    public class OrdersViewModel : BaseViewModel
    {
        private readonly OrdersManger _ordersManger;
        private readonly clsCustomersManager _custManger;
        private CancellationTokenSource? _searchCts;

        public OrdersViewModel()
        {
            _ordersManger = new OrdersManger();
            _custManger = new clsCustomersManager();

            OrdersList = new ObservableCollection<clsOrders>();
            CustList = new ObservableCollection<clsCust>();

            _pageSize = 6;
            _currentPage = 1;
            _custCurrentPage = 1;

            LoadOrdersCommand = new RelayCommand(async _ => await GetOrdersAsync());
            OrdersPageChangedCommand = new RelayCommand(async _ => await GetOrdersAsync());
            CustPageChangedCommand = new RelayCommand(async _ => await GetCustAsync());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch());

            AddOrderCommand = new RelayCommand(async _ => await AddOrderAsync(), _ => CanSaveOrder());
            UpdateOrderCommand = new RelayCommand(async _ => await UpdateOrderAsync(), _ => CanUpdateOrder());
            DeleteOrderCommand = new RelayCommand(async _ => await DeleteOrderAsync(), _ => CanDeleteOrder());
            ClearFieldsCommand = new RelayCommand(async _ => await ClearFieldsAsync());

            OpenPatientManagementCommand = new RelayCommand(_ => OpenPatientManagement());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            ORDER_CODE = await GetNew_ORDER_CODEAsync();
            await Task.WhenAll(GetOrdersAsync(), GetCustAsync());
        }

        #region Properties 

        private long _order_id;
        public long ORDER_ID
        {
            get => _order_id;
            set { if (_order_id != value) { _order_id = value; OnPropertyChanged(); } }
        }

        private long? _order_code;
        public long? ORDER_CODE
        {
            get => _order_code;
            set { if (_order_code != value) { _order_code = value; OnPropertyChanged(); } }
        }

        private DateTime? _order_date = DateTime.Now;
        public DateTime? ORDER_DATE
        {
            get => _order_date;
            set { if (_order_date != value) { _order_date = value; OnPropertyChanged(); } }
        }

        private TimeSpan? _order_time = DateTime.Now.TimeOfDay;
        public TimeSpan? ORDER_TIME
        {
            get => _order_time;
            set { if (_order_time != value) { _order_time = value; OnPropertyChanged(); } }
        }

        private string? _order_note;
        public string? ORDER_NOTE
        {
            get => _order_note;
            set { if (_order_note != value) { _order_note = value; OnPropertyChanged(); } }
        }

        private long? _cust_id;
        public long? CUST_ID
        {
            get => _cust_id;
            set { if (_cust_id != value) { _cust_id = value; OnPropertyChanged(); } }
        }

        private string? _cust_Mobile;
        public string? CUST_MOBILE
        {
            get => _cust_Mobile;
            set { if (_cust_Mobile != value) { _cust_Mobile = value; OnPropertyChanged(); } }
        }

        private string? _custName;
        public string? CUST_NAME
        {
            get => _custName;
            set { if (_custName != value) { _custName = value; OnPropertyChanged(); } }
        }

        private string? _firstName;
        public string? FIRST_NAME
        {
            get => _firstName;
            set { if (_firstName != value) { _firstName = value; OnPropertyChanged(); CombineFullName(); } }
        }

        private string? _secondName;
        public string? SECOND_NAME
        {
            get => _secondName;
            set { if (_secondName != value) { _secondName = value; OnPropertyChanged(); CombineFullName(); } }
        }

        private string? _thirdName;
        public string? THIRD_NAME
        {
            get => _thirdName;
            set { if (_thirdName != value) { _thirdName = value; OnPropertyChanged(); CombineFullName(); } }
        }

        private string? _lastName;
        public string? LAST_NAME
        {
            get => _lastName;
            set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); CombineFullName(); } }
        }

        private int? _custAge;
        public int? CUST_AGE
        {
            get => _custAge;
            set { if (_custAge != value) { _custAge = value; OnPropertyChanged(); } }
        }

        private DateTime? _custBd;
        public DateTime? CUST_BD
        {
            get => _custBd;
            set { if (_custBd != value) { _custBd = value; OnPropertyChanged(); } }
        }

        private string? _custAddress;
        public string? CUST_ADDRESS
        {
            get => _custAddress;
            set { if (_custAddress != value) { _custAddress = value; OnPropertyChanged(); } }
        }

        private string? _insStatus;
        public string? INS_STATUS
        {
            get => _insStatus;
            set { if (_insStatus != value) { _insStatus = value; OnPropertyChanged(); } }
        }

        private string? _custNotes;
        public string? CUST_NOTES
        {
            get => _custNotes;
            set { if (_custNotes != value) { _custNotes = value; OnPropertyChanged(); } }
        }

        private int? _insCardNo;
        public int? INS_CARD_NO
        {
            get => _insCardNo;
            set { if (_insCardNo != value) { _insCardNo = value; OnPropertyChanged(); } }
        }

        private long? _cli = 1;
        public long? CLI
        {
            get => _cli;
            set { if (_cli != value) { _cli = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<clsOrders> _ordersList = new();
        public ObservableCollection<clsOrders> OrdersList
        {
            get => _ordersList;
            set { _ordersList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsCust> _custList = new();
        public ObservableCollection<clsCust> CustList
        {
            get => _custList;
            set { _custList = value; OnPropertyChanged(); }
        }

        private clsOrders? _selectedOrder;
        public clsOrders? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedOrder();
                }
            }
        }

        private clsCust? _selectedPatient;
        public clsCust? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if (_selectedPatient != value)
                {
                    _selectedPatient = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedPatient();
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
                    CustCurrentPage = 1;
                    _ = PerformSearchWithDebounceAsync();
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
                    _ = GetOrdersAsync();
                }
            }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(); }
        }

        private int _totalCustRows;
        public int TotalCustRows
        {
            get => _totalCustRows;
            set { _totalCustRows = value; OnPropertyChanged(); }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set { if (_currentPage != value) { _currentPage = value; OnPropertyChanged(); } }
        }

        private int _custCurrentPage;
        public int CustCurrentPage
        {
            get => _custCurrentPage;
            set { if (_custCurrentPage != value) { _custCurrentPage = value; OnPropertyChanged(); } }
        }

        #endregion

        #region Commands 

        public ICommand LoadOrdersCommand { get; }
        public ICommand OrdersPageChangedCommand { get; }
        public ICommand CustPageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddOrderCommand { get; }
        public ICommand UpdateOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand OpenPatientManagementCommand { get; }

        #endregion

        #region Methods 

        private async Task PerformSearchWithDebounceAsync()
        {
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                {
                    await Task.WhenAll(GetOrdersAsync(token), GetCustAsync(token));
                }
            }
            catch (OperationCanceledException)
            {
                // تم إلغاء البحث لبدء استعلام جديد أثناء الكتابة
            }
        }

        private bool IsEmptyInputs() => !CUST_ID.HasValue || CUST_ID <= 0 || !ORDER_CODE.HasValue;

        private async Task<long> GetNew_ORDER_CODEAsync()
        {
            try
            {
                return await _ordersManger.GetNewOrder_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الطلب", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreOrdersEqual(clsOrders o1, clsOrders o2)
        {
            if (o1 == null || o2 == null) return o1 == o2;

            return o1.ORDER_ID == o2.ORDER_ID &&
                   o1.ORDER_CODE == o2.ORDER_CODE &&
                   o1.ORDER_DATE?.Date == o2.ORDER_DATE?.Date &&
                   o1.ORDER_TIME == o2.ORDER_TIME &&
                   o1.ORDER_NOTE == o2.ORDER_NOTE &&
                   o1.CUST_ID == o2.CUST_ID &&
                   o1.CLI_ID == o2.CLI_ID;
        }

        private async Task GetOrdersAsync(CancellationToken token = default)
        {
            try
            {
                TotalRows = await _ordersManger.GetTotalOrdersCountAsync(SearchQuery);
                if (token.IsCancellationRequested) return;

                var pagedOrders = await _ordersManger.GetOrdersPagedAsync(CurrentPage, PageSize, SearchQuery)
                                  ?? new List<clsOrders>();
                if (token.IsCancellationRequested) return;

                OrdersList = new ObservableCollection<clsOrders>(pagedOrders);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الطلبات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetCustAsync(CancellationToken token = default)
        {
            try
            {
                TotalCustRows = await _custManger.GetTotalCustomersCountAsync(SearchQuery);
                if (token.IsCancellationRequested) return;

                var pageCust = await _custManger.GetCustomersPagedAsync(CustCurrentPage, PageSize, SearchQuery)
                                  ?? new List<clsCust>();
                if (token.IsCancellationRequested) return;

                CustList = new ObservableCollection<clsCust>(pageCust);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات المرضى", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch() => SearchQuery = string.Empty;

        private bool CanSaveOrder() => !IsEmptyInputs();
        private bool CanUpdateOrder() => SelectedOrder != null && SelectedOrder.ORDER_ID > 0;
        private bool CanDeleteOrder() => SelectedOrder != null && SelectedOrder.ORDER_ID > 0;

        private async Task AddOrderAsync()
        {
            try
            {
                var newOrder = BuildOrderFromProperties();
                long insertedId = await _ordersManger.AddOrderAsync(newOrder);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الطلب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetOrdersAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task UpdateOrderAsync()
        {
            if (SelectedOrder == null) return;

            if (AreOrdersEqual(SelectedOrder, BuildOrderFromProperties()))
            {
                MessageBox.Show("لا يوجد فرق بين البيانات السابقة والحالية", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var orderToUpdate = BuildOrderFromProperties();
                bool isSuccess = await _ordersManger.UpdateOrderAsync(orderToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الطلب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetOrdersAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteOrderAsync()
        {
            if (SelectedOrder == null) return;

            var result = MessageBox.Show("هل أنت تأكد من حذف هذا الطلب؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                bool isSuccess = await _ordersManger.DeleteOrderAsync(SelectedOrder.ORDER_ID);

                if (isSuccess)
                {
                    MessageBox.Show("تم حذف الطلب بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetOrdersAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillFieldsFromSelectedPatient()
        {
            if (SelectedPatient == null) return;

            _selectedOrder = null;
            OnPropertyChanged(nameof(SelectedOrder));

            CUST_ID = SelectedPatient.CUST_ID;
            FIRST_NAME = SelectedPatient.CUST_F_NAME;
            SECOND_NAME = SelectedPatient.CUST_S_NAME;
            THIRD_NAME = SelectedPatient.CUST_T_NAME;
            LAST_NAME = SelectedPatient.CUST_L_NAME;

            CombineFullName();

            CUST_MOBILE = SelectedPatient.CUST_MOBILE1;
            CUST_ADDRESS = SelectedPatient.CUST_ADDRESS;

            if (int.TryParse(SelectedPatient.CARD_ID.ToString(), out int cardId))
            {
                INS_CARD_NO = cardId;
            }
            else
            {
                INS_CARD_NO = null;
            }
        }

        private void FillFieldsFromSelectedOrder()
        {
            if (SelectedOrder == null) return;

            ORDER_ID = SelectedOrder.ORDER_ID;
            ORDER_CODE = SelectedOrder.ORDER_CODE;
            ORDER_DATE = SelectedOrder.ORDER_DATE;
            ORDER_TIME = SelectedOrder.ORDER_TIME;
            ORDER_NOTE = SelectedOrder.ORDER_NOTE;
            CUST_ID = SelectedOrder.CUST_ID;
            CUST_NAME = SelectedOrder.CUST_NAME;
            CLI = SelectedOrder.CLI_ID;
        }

        private void CombineFullName()
        {
            CUST_NAME = ConcatFullName(FIRST_NAME, SECOND_NAME, THIRD_NAME, LAST_NAME);
        }

        private clsOrders BuildOrderFromProperties()
        {
            return new clsOrders
            {
                ORDER_ID = this.ORDER_ID,
                ORDER_CODE = this.ORDER_CODE,
                ORDER_DATE = this.ORDER_DATE,
                ORDER_TIME = this.ORDER_TIME,
                ORDER_NOTE = this.ORDER_NOTE ?? string.Empty,
                CUST_ID = this.CUST_ID,
                CUST_NAME = this.CUST_NAME,
                CLI_ID = this.CLI
            };
        }

        private async Task ClearFieldsAsync()
        {
            ORDER_ID = 0;
            ORDER_CODE = await GetNew_ORDER_CODEAsync();
            ORDER_DATE = DateTime.Now;
            ORDER_TIME = DateTime.Now.TimeOfDay;
            ORDER_NOTE = string.Empty;
            CUST_ID = null;
            CLI = 1;
            SelectedOrder = null;

            FIRST_NAME = string.Empty;
            SECOND_NAME = string.Empty;
            THIRD_NAME = string.Empty;
            LAST_NAME = string.Empty;
            CUST_NAME = string.Empty;
            CUST_MOBILE = string.Empty;
            CUST_AGE = null;
            CUST_BD = null;
            CUST_ADDRESS = string.Empty;
            CUST_NOTES = string.Empty;
            INS_STATUS = string.Empty;
            INS_CARD_NO = null;
            SelectedPatient = null;
        }

        private void OpenPatientManagement()
        {
            var customersWin = new CustomersWindow();
            if (Application.Current.MainWindow != null)
                customersWin.Owner = Application.Current.MainWindow;

            customersWin.ShowDialog();

            _ = RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            await Task.WhenAll(GetOrdersAsync(), GetCustAsync());
        }

        private string ConcatFullName(params string?[] names)
        {
            return string.Join(" ", names.Where(n => !string.IsNullOrWhiteSpace(n))).Trim();
        }

        #endregion

    }
}
