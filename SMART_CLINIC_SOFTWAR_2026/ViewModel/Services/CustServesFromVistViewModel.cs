using BLL.Mangers.Customers;
using BLL.Mangers.Services;
using Core.Entites.Customer;
using Core.Entites.Services;
using SMART_CLINIC_SOFTWAR_2026.View.Services;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Services
{
    public class CustServesFromVistViewModel : BaseViewModel
    {
        #region Fields & Managers

        private readonly clsServicesManger _servicesManager;
        private readonly clsCustomersManager _customerManager;
        private readonly long? _custID;

        #endregion

        #region Properties & Data Binding

        private clsCust? _customer;
        public clsCust? Customer
        {
            get => _customer;
            set { _customer = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsService> _allServicesList;
        public ObservableCollection<clsService> AllServicesList
        {
            get => _allServicesList;
            set { _allServicesList = value; OnPropertyChanged(); }
        }

        private clsService? _selectedAllService;
        public clsService? SelectedAllService
        {
            get => _selectedAllService;
            set
            {
                _selectedAllService = value;
                OnPropertyChanged();
                if (value != null)
                {
                    AddServiceToVisit(value);
                }
            }
        }

        private int _allServicesCurrentPage = 1;
        public int AllServicesCurrentPage
        {
            get => _allServicesCurrentPage;
            set { _allServicesCurrentPage = value; OnPropertyChanged(); }
        }

        private int _allServicesTotalRows;
        public int AllServicesTotalRows
        {
            get => _allServicesTotalRows;
            set { _allServicesTotalRows = value; OnPropertyChanged(); }
        }

        private int _allServicesPageSize = 10;
        public int AllServicesPageSize
        {
            get => _allServicesPageSize;
            set { _allServicesPageSize = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsService> _visitServicesList;
        public ObservableCollection<clsService> VisitServicesList
        {
            get => _visitServicesList;
            set { _visitServicesList = value; OnPropertyChanged(); }
        }

        private clsService? _selectedVisitService;
        public clsService? SelectedVisitService
        {
            get => _selectedVisitService;
            set { _selectedVisitService = value; OnPropertyChanged(); }
        }

        private int _visitPage = 1;
        public int VisitPage
        {
            get => _visitPage;
            set { _visitPage = value; OnPropertyChanged(); }
        }

        private int _visitTotalRows;
        public int VisitTotalRows
        {
            get => _visitTotalRows;
            set { _visitTotalRows = value; OnPropertyChanged(); }
        }

        private int _visitPageSize = 10;
        public int VisitPageSize
        {
            get => _visitPageSize;
            set { _visitPageSize = value; OnPropertyChanged(); }
        }

        private string? _searchServiceQuery;
        public string? SearchServiceQuery
        {
            get => _searchServiceQuery;
            set
            {
                _searchServiceQuery = value;
                OnPropertyChanged();
                _ = FilterAndLoadServicesAsync();
            }
        }

        #endregion

        #region Commands

        public ICommand NewServiceCommand { get; }
        public ICommand PreviousServiceCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteAllCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AllServicesPageChangedCommand { get; }
        public ICommand VisitPageChangedCommand { get; }

        #endregion

        #region Constructor

        public CustServesFromVistViewModel(long? CustID)
        {
            _custID = CustID;
            _servicesManager = new clsServicesManger();
            _customerManager = new clsCustomersManager();

            AllServicesList = new ObservableCollection<clsService>();
            VisitServicesList = new ObservableCollection<clsService>();

            NewServiceCommand = new RelayCommand(ExecuteNewService);
            PreviousServiceCommand = new RelayCommand(ExecutePreviousService);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            DeleteAllCommand = new RelayCommand(ExecuteDeleteAll, CanExecuteDeleteAll);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);

            AllServicesPageChangedCommand = new RelayCommand(async (p) => await OnAllServicesPageChangedAsync(p));
            VisitPageChangedCommand = new RelayCommand(OnVisitPageChanged);

            _ = InitializeViewModelAsync();
        }

        #endregion

        #region Data Loading Methods

        private async Task InitializeViewModelAsync()
        {
            await LoadCustomerDataAsync();
            await LoadAllServicesPagedAsync();
        }

        private async Task LoadCustomerDataAsync()
        {
            if (_custID.HasValue && _custID.Value > 0)
            {
                try
                {
                    Customer = await clsCustomersManager.GetCustomerByIdAsync(_custID.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ أثناء جلب بيانات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task LoadAllServicesPagedAsync()
        {
            try
            {
                var services = await _servicesManager.GetServicesPagedAsync(AllServicesCurrentPage, AllServicesPageSize, SearchServiceQuery);
                AllServicesTotalRows = await _servicesManager.GetTotalServicesCountAsync(SearchServiceQuery);

                AllServicesList = new ObservableCollection<clsService>(services);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الإجراءات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task FilterAndLoadServicesAsync()
        {
            AllServicesCurrentPage = 1;
            await LoadAllServicesPagedAsync();
        }

        private void AddServiceToVisit(clsService service)
        {
            if (service == null) return;

            if (!VisitServicesList.Any(s => s.SER_ID == service.SER_ID && s.SER_ID > 0))
            {
                VisitServicesList.Add(service);
                VisitTotalRows = VisitServicesList.Count;
            }
        }

        #endregion

        #region Command Implementations

        private void ExecuteNewService(object? obj)
        {
            // فتح نافذة إضافة خدمة جديدة بالنظام
        }

        private void ExecutePreviousService(object? obj)
        {
            Cust_ServicesHestoryWindow window = new Cust_ServicesHestoryWindow(Customer.CUST_ID);
            window.ShowDialog();
        }


        private void ExecuteDelete(object? obj)
        {
            if (SelectedVisitService != null)
            {
                VisitServicesList.Remove(SelectedVisitService);
                VisitTotalRows = VisitServicesList.Count;
                SelectedVisitService = null;
            }
        }

        private bool CanExecuteDelete(object? obj)
        {
            return SelectedVisitService != null;
        }

        private void ExecuteDeleteAll(object? obj)
        {
            if (VisitServicesList.Count > 0)
            {
                var result = MessageBox.Show("هل أنت تأكد من حذف جميع الإجراءات المضافة للزيارة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    VisitServicesList.Clear();
                    VisitTotalRows = 0;
                    SelectedVisitService = null;
                }
            }
        }

        private bool CanExecuteDeleteAll(object? obj)
        {
            return VisitServicesList != null && VisitServicesList.Count > 0;
        }

        private void ExecuteClearSearch(object? obj)
        {
            SearchServiceQuery = string.Empty;
        }

        private async Task OnAllServicesPageChangedAsync(object? obj)
        {
            await LoadAllServicesPagedAsync();
        }

        private void OnVisitPageChanged(object? obj)
        {
            // كود معالجة التنقل الداخلي في شبكة خدمات الزيارة الحالية
        }

        #endregion
    }
}
