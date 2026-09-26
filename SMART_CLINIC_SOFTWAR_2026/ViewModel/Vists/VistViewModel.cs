using BLL.Mangers.Clinc;
using BLL.Mangers.Customers;
using BLL.Mangers.Orders;
using BLL.Mangers.Vists;
using Core.Entites.Clinc;
using Core.Entites.Customer;
using Core.Entites.Orders;
using Core.Entites.Vist;
using SMART_CLINIC_SOFTWAR_2026.View.Clinc;
using SMART_CLINIC_SOFTWAR_2026.View.Customers;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Clinc;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Customers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Vists
{
    public class VistViewModel : BaseViewModel
    {
        private clsVistManger _vistManger;
        private OrdersManger _ordersMnager;
        private clsCustomersManager _custManger;

        public VistViewModel()
        {
            _vistManger = new clsVistManger();
            _ordersMnager = new OrdersManger();
            _custManger = new clsCustomersManager();

            VisitsList = new ObservableCollection<clsVist>();
            PaymentTypesList = new ObservableCollection<string>();
            OrdersList = new ObservableCollection<clsOrders>();
            OrderItemsList = new ObservableCollection<clsOrders>();
            ClinicsList = new ObservableCollection<clsClinc>();

            _pageSize = 17;
            _currentPage = 1;

            LoadOrdersCommand = new RelayCommand(async param => await GetOrdersAsync());
            PageChangedCommand = new RelayCommand(async param => await GetOrdersAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddVisitCommand = new RelayCommand(async param => await AddVisitAsync(), param => CanSaveVisit());
            DeleteVisitCommand = new RelayCommand(async param => await DeleteVisitAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());
            ClearHeaderFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            RemoveOrderItemCommand = new RelayCommand(param => RemoveOrderItem());

            OpenClinicsLookupCommand = new RelayCommand(param => OpenClinicsLookup());
            OpenPatientsLookupCommand = new RelayCommand(param => OpenPatientsLookup());

            OpenDiagnosisCommand = new RelayCommand(async param => await OpenDiagnosis(), param => IsVisitActive);
            OpenLabTestsCommand = new RelayCommand(async param => await OpenLabTest(), param => IsVisitActive);
            OpenProceduresCommand = new RelayCommand(async param => await OpenProcedures(), param => IsVisitActive);
            OpenMedicationCommand = new RelayCommand(async param => await OpenMedication(), param => IsVisitActive);
            OpenSickLeavesCommand = new RelayCommand(async param => await OpenSickLeave(), param => IsVisitActive);
            OpenMedicalReportsCommand = new RelayCommand(async param => await OpenMedicalReports(), param => IsVisitActive);
            OpenAppointmentsCommand = new RelayCommand(async param => await OpenAppointments(), param => IsVisitActive);
            OpenPatientCardCommand = new RelayCommand(async param => await OpenPatientCard(), param => IsPatientSelected);
            OpenChronicDiseasesCommand = new RelayCommand(async param => await OpenChronicDiseases(), param => IsPatientSelected);

            SelectOrderCommand = new RelayCommand(async param => await FillFieldsFromSelectedOrder());
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            VIS_CODE = await GetNew_VIS_CODEAsync();
            FillThePaymentTypes();
            await GetVisitsAsync();
            await GetOrdersAsync();
        }

        #region Properties

        private long _vis_id;
        public long VIS_ID
        {
            get => _vis_id;
            set
            {
                if (_vis_id != value)
                {
                    _vis_id = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsVisitActive));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private long _vis_code;
        public long VIS_CODE
        {
            get => _vis_code;
            set { if (_vis_code != value) { _vis_code = value; OnPropertyChanged(); } }
        }

        private string _vis_name = string.Empty;
        public string VIS_NAME
        {
            get => _vis_name;
            set { if (_vis_name != value) { _vis_name = value; OnPropertyChanged(); } }
        }

        private DateTime? _vis_date = DateTime.Now;
        public DateTime? VIS_DATE
        {
            get => _vis_date;
            set { if (_vis_date != value) { _vis_date = value; OnPropertyChanged(); } }
        }

        private TimeSpan _vis_time = DateTime.Now.TimeOfDay;
        public TimeSpan VIS_TIME
        {
            get => _vis_time;
            set { if (_vis_time != value) { _vis_time = value; OnPropertyChanged(); } }
        }

        private string _vis_type = string.Empty;
        public string VIS_TYPE
        {
            get => _vis_type;
            set { if (_vis_type != value) { _vis_type = value; OnPropertyChanged(); } }
        }

        private decimal _vis_price;
        public decimal VIS_PRICE
        {
            get => _vis_price;
            set { if (_vis_price != value) { _vis_price = value; OnPropertyChanged(); CalculateTotal(); } }
        }

        private decimal _vis_discount;
        public decimal VIS_DISCOUNT
        {
            get => _vis_discount;
            set { if (_vis_discount != value) { _vis_discount = value; OnPropertyChanged(); CalculateTotal(); } }
        }

        private decimal _vis_total;
        public decimal VIS_TOTAL
        {
            get => _vis_total;
            set { if (_vis_total != value) { _vis_total = value; OnPropertyChanged(); } }
        }

        private string _vis_pay_type = string.Empty;
        public string VIS_PAY_TYPE
        {
            get => _vis_pay_type;
            set { if (_vis_pay_type != value) { _vis_pay_type = value; OnPropertyChanged(); } }
        }

        private long? _cli_id;
        public long? CLI_ID
        {
            get => _cli_id;
            set
            {
                if (_cli_id != value)
                {
                    _cli_id = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPatientAndClinicSelected));

                    var matchedClinic = ClinicsList.FirstOrDefault(c => c.CLI_ID == _cli_id);
                    if (matchedClinic != null)
                    {
                        CLI_NAME = matchedClinic.CLI_NAME;
                    }
                }
            }
        }

        private string? _cli_Name;
        public string? CLI_NAME
        {
            get => _cli_Name;
            set
            {
                if (_cli_Name != value)
                {
                    _cli_Name = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPatientAndClinicSelected));
                }
            }
        }

        private long _doc_id;
        public long DOC_ID
        {
            get => _doc_id;
            set { if (_doc_id != value) { _doc_id = value; OnPropertyChanged(); } }
        }

        private long _apo_id;
        public long APO_ID
        {
            get => _apo_id;
            set { if (_apo_id != value) { _apo_id = value; OnPropertyChanged(); } }
        }

        private long _serlist_id;
        public long SERLIST_ID
        {
            get => _serlist_id;
            set { if (_serlist_id != value) { _serlist_id = value; OnPropertyChanged(); } }
        }

        private long? _cust_id;
        public long? CUST_ID
        {
            get => _cust_id;
            set
            {
                if (_cust_id != value)
                {
                    _cust_id = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPatientSelected));
                    OnPropertyChanged(nameof(IsVisitActive));
                    OnPropertyChanged(nameof(IsPatientAndClinicSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
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
        public string? FOURTH_NAME
        {
            get => _lastName;
            set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); CombineFullName(); } }
        }

        private void CombineFullName()
        {
            VIS_NAME = $"{FIRST_NAME} {SECOND_NAME} {THIRD_NAME} {FOURTH_NAME}".Replace("  ", " ").Trim();
        }

        private int? _custAge;
        public int? CUST_AGE
        {
            get => _custAge;
            set
            {
                if (_custAge != value)
                {
                    _custAge = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _custBd;
        public DateTime? CUST_BD
        {
            get => _custBd;
            set
            {
                if (_custBd != value)
                {
                    _custBd = value;
                    OnPropertyChanged();
                    CalculateAgeFromBD();
                }
            }
        }

        /// <summary>
        /// دالة استنتاج وحساب العمر بناءً على تاريخ الميلاد (CUST_BD)
        /// </summary>
        private void CalculateAgeFromBD()
        {
            if (CUST_BD.HasValue && CUST_BD.Value <= DateTime.Today)
            {
                var today = DateTime.Today;
                int age = today.Year - CUST_BD.Value.Year;

                if (CUST_BD.Value.Date > today.AddYears(-age))
                {
                    age--;
                }

                CUST_AGE = age >= 0 ? age : 0;
            }
            else
            {
                CUST_AGE = null;
            }
        }

        private ObservableCollection<clsVist> _visitsList = new();
        public ObservableCollection<clsVist> VisitsList
        {
            get => _visitsList;
            set { _visitsList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsOrders> _ordersList = new();
        public ObservableCollection<clsOrders> OrdersList
        {
            get => _ordersList;
            set { _ordersList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsOrders> _orderItemsList = new();
        public ObservableCollection<clsOrders> OrderItemsList
        {
            get => _orderItemsList;
            set { _orderItemsList = value; OnPropertyChanged(); CalculateItemsTotals(); }
        }

        private ObservableCollection<string> _paymentTypesList = new();
        public ObservableCollection<string> PaymentTypesList
        {
            get => _paymentTypesList;
            set { _paymentTypesList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsClinc> _clinicsList = new();
        public ObservableCollection<clsClinc> ClinicsList
        {
            get => _clinicsList;
            set { _clinicsList = value; OnPropertyChanged(); }
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
                    _ = FillFieldsFromSelectedOrder();
                }
            }
        }

        private clsOrders? _selectedOrderItem;
        public clsOrders? SelectedOrderItem
        {
            get => _selectedOrderItem;
            set { if (_selectedOrderItem != value) { _selectedOrderItem = value; OnPropertyChanged(); } }
        }

        private string? _searchQuery;
        public string? SearchQuery
        {
            get => _searchQuery;
            set { if (_searchQuery != value) { _searchQuery = value; OnPropertyChanged(); CurrentPage = 1; } }
        }

        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set { if (_pageSize != value && value > 0) { _pageSize = value; OnPropertyChanged(); CurrentPage = 1; _ = GetOrdersAsync(); } }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(); }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set { if (_currentPage != value) { _currentPage = value; OnPropertyChanged(); } }
        }

        public bool IsPatientSelected => CUST_ID.HasValue && CUST_ID.Value > 0;
        public bool IsVisitActive => VIS_ID > 0 || IsPatientSelected;

        public bool IsPatientAndClinicSelected => CUST_ID.HasValue && CUST_ID > 0 && CLI_ID.HasValue && CLI_ID > 0;

        #endregion

        #region Commands

        public ICommand LoadOrdersCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddVisitCommand { get; }
        public ICommand UpdateVisitCommand { get; }
        public ICommand DeleteVisitCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand ClearHeaderFieldsCommand { get; }
        public ICommand RemoveOrderItemCommand { get; }

        public ICommand OpenClinicsLookupCommand { get; }
        public ICommand OpenPatientsLookupCommand { get; }

        public ICommand OpenDiagnosisCommand { get; }
        public ICommand OpenLabTestsCommand { get; }
        public ICommand OpenProceduresCommand { get; }
        public ICommand OpenMedicationCommand { get; }
        public ICommand OpenSickLeavesCommand { get; }
        public ICommand OpenMedicalReportsCommand { get; }
        public ICommand OpenAppointmentsCommand { get; }
        public ICommand OpenPatientCardCommand { get; }
        public ICommand OpenChronicDiseasesCommand { get; }

        public ICommand SelectOrderCommand { get; }

        #endregion

        #region Methods

        private void CalculateTotal()
        {
            VIS_TOTAL = VIS_PRICE - VIS_DISCOUNT;
        }

        private void CalculateItemsTotals()
        {
            if (OrderItemsList == null || OrderItemsList.Count == 0)
            {
                VIS_PRICE = 0m;
                VIS_DISCOUNT = 0m;
                VIS_TOTAL = 0m;
                return;
            }

            CalculateTotal();
        }

        private async Task FillFieldsFromSelectedOrder()
        {
            if (SelectedOrder == null) return;

            CUST_ID = SelectedOrder.CUST_ID;
            CLI_ID = SelectedOrder.CLI_ID;

            if (CUST_ID.HasValue)
            {
                clsCust custmer = await _custManger.GetCustomerByIdAsync(CUST_ID);

                if (custmer != null)
                {
                    if (custmer != null)
                    {
                        FIRST_NAME = custmer.CUST_F_NAME;
                        SECOND_NAME = custmer.CUST_S_NAME;
                        THIRD_NAME = custmer.CUST_T_NAME;
                        FOURTH_NAME = custmer.CUST_L_NAME;

                        // تحويل DateOnly المباشر إلى DateTime
                        CUST_BD = custmer.CUST_BD.ToDateTime(TimeOnly.MinValue);
                    }

                }

                if (SelectedOrder.ORDER_DATE.HasValue)
                {
                    CUST_BD = SelectedOrder.ORDER_DATE.Value;
                }



                VIS_TIME = SelectedOrder.ORDER_TIME ?? TimeSpan.Zero;
            }
        }

        private void RemoveOrderItem()
        {
            if (SelectedOrderItem == null) return;

            OrderItemsList.Remove(SelectedOrderItem);
            CalculateItemsTotals();
        }

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(VIS_NAME) ||
                   VIS_CODE <= 0 ||
                   !CUST_ID.HasValue || CUST_ID <= 0;
        }

        private async Task<long> GetNew_VIS_CODEAsync()
        {
            try
            {
                return await _vistManger.GetNewVis_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الزيارة", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreVisitsEqual(clsVist? v1, clsVist? v2)
        {
            if (v1 == null || v2 == null) return v1 == v2;
            string json1 = JsonSerializer.Serialize(v1);
            string json2 = JsonSerializer.Serialize(v2);
            return json1 == json2;
        }

        private async Task GetVisitsAsync()
        {
            try
            {
                TotalRows = await _vistManger.GetTotalVisitsCountAsync(SearchQuery);
                var pagedVisits = await _vistManger.GetVisitsPagedAsync(CurrentPage, PageSize, SearchQuery) ?? new List<clsVist>();
                VisitsList = new ObservableCollection<clsVist>(pagedVisits);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الزيارات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveVisit()
        {
            return !IsEmptyInputs();
        }

        private bool CanDelete() => false;
        private bool CanUpdate() => false;

        private async Task AddVisitAsync()
        {
            try
            {
                var newVisit = BuildVisitFromProperties();
                long insertedId = await _vistManger.AddVisitAsync(newVisit);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة وتسجيل الزيارة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetVisitsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteVisitAsync()
        {
            if (VIS_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذه الزيارة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _vistManger.DeleteVisitAsync(VIS_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الزيارة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetVisitsAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private clsVist BuildVisitFromProperties()
        {
            return new clsVist
            {
                VIS_ID = this.VIS_ID,
                VIS_CODE = this.VIS_CODE,
                VIS_NAME = this.VIS_NAME ?? string.Empty,
                VIS_DATE = this.VIS_DATE.HasValue ? DateOnly.FromDateTime(this.VIS_DATE.Value) : default,
                VIS_TYPE = this.VIS_TYPE ?? string.Empty,
                VIS_TIME = TimeOnly.FromTimeSpan(this.VIS_TIME),
                CUST_ID = this.CUST_ID,
                CLI_ID = this.CLI_ID,
                APO_ID = this.APO_ID,
                DOC_ID = this.DOC_ID,
                SERLIST_ID = this.SERLIST_ID,
                VIS_PRICE = this.VIS_PRICE,
                VIS_DISCOUNT = this.VIS_DISCOUNT,
                VIS_TOTAL = this.VIS_TOTAL,
                VIS_PAY_TYPE = this.VIS_PAY_TYPE ?? string.Empty
            };
        }

        private async Task ClearFieldsAsync()
        {
            VIS_ID = 0;
            VIS_CODE = await GetNew_VIS_CODEAsync();
            VIS_NAME = string.Empty;
            VIS_DATE = DateTime.Now;
            VIS_TYPE = string.Empty;
            VIS_TIME = DateTime.Now.TimeOfDay;
            CUST_ID = 0;
            CLI_ID = 0;
            CLI_NAME = string.Empty;
            APO_ID = 0;
            DOC_ID = 0;
            SERLIST_ID = 0;
            VIS_PRICE = 0m;
            VIS_DISCOUNT = 0m;
            VIS_TOTAL = 0m;
            VIS_PAY_TYPE = string.Empty;
            FIRST_NAME = string.Empty;
            SECOND_NAME = string.Empty;
            THIRD_NAME = string.Empty;
            FOURTH_NAME = string.Empty;
            CUST_BD = null;
            CUST_AGE = null;
            SelectedOrder = null;
            SelectedOrderItem = null;
            OrderItemsList.Clear();

            CommandManager.InvalidateRequerySuggested();
        }

        private void FillThePaymentTypes()
        {
            PaymentTypesList.Clear();
            PaymentTypesList.Add("كاش");
            PaymentTypesList.Add("فيزا");
            PaymentTypesList.Add("تحويل");
        }

        private async Task GetOrdersAsync()
        {
            try
            {
                TotalRows = await OrdersManger.GetTotalOrdersCountAsync(SearchQuery);
                var pagedOrders = await _ordersMnager.GetOrdersPagedAsync(CurrentPage, PageSize, SearchQuery) ?? new List<clsOrders>();
                OrdersList = new ObservableCollection<clsOrders>(pagedOrders);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الطلبات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Lookups Methods

        private void OpenClinicsLookup()
        {
            var clinicsWin = new ClincListWindow();

            var activeWindow = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.IsActive && w != clinicsWin);

            if (activeWindow != null)
            {
                clinicsWin.Owner = activeWindow;
            }

            if (clinicsWin.ShowDialog() == true)
            {
                if (clinicsWin.DataContext is ClincListViewModel vm && vm.SelectedClinic != null)
                {
                    var selected = vm.SelectedClinic;

                    if (!ClinicsList.Any(c => c.CLI_ID == selected.CLI_ID))
                    {
                        ClinicsList.Add(selected);
                    }

                    CLI_ID = selected.CLI_ID;
                    CLI_NAME = selected.CLI_NAME;

                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private void OpenPatientsLookup()
        {
            var customersWin = new CustomersListWindow();

            var activeWindow = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.IsActive && w != customersWin);

            if (activeWindow != null)
            {
                customersWin.Owner = activeWindow;
            }

            if (customersWin.ShowDialog() == true)
            {
                if (customersWin.DataContext is CustomersListViewModel vm && vm.SelectedCustomer != null)
                {
                    var selectedCust = vm.SelectedCustomer;

                    CUST_ID = selectedCust.CUST_ID;
                    FIRST_NAME = selectedCust.CUST_F_NAME;
                    SECOND_NAME = selectedCust.CUST_S_NAME;
                    THIRD_NAME = selectedCust.CUST_T_NAME;
                    FOURTH_NAME = selectedCust.CUST_L_NAME;

                  //   CUST_BD = SelectedOrder.ORDER_DATE;


                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        #endregion

        #region OpenWindowsMethods

        private async Task OpenDiagnosis()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة التشخيص والفحوصات الطبية.", "التشخيص", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenLabTest()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة فحوصات المختبر.", "المختبر", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenProcedures()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة الإجراءات والعمليات.", "الإجراءات الطبية", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenMedication()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة وصف وصرف العلاج.", "الوصفات الطبية", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenSickLeave()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة الإجازات المرضية.", "الإجازات المرضية", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenMedicalReports()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة التقارير الطبية.", "التقارير الطبية", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenAppointments()
        {
            MessageBox.Show("تم التحقق من تفعيل الزيارة: سيتم فتح شاشة مواعيد المريض.", "المواعيد", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenPatientCard()
        {
            MessageBox.Show("تم التحقق من تحديد المريض: سيتم فتح شاشة بطاقة المريض والسجل الطبي.", "بطاقة المريض", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        private async Task OpenChronicDiseases()
        {
            MessageBox.Show("تم التحقق من تحديد المريض: سيتم فتح شاشة الأمراض المزمنة للمريض.", "الأمراض المزمنة", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        #endregion

        #endregion
    }
}
