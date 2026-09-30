using BLL.Mangers.Customers;
using BLL.Mangers.Services;
using Core.Entites.Customer;
using Core.Entites.Services;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Services
{
    public class Cust_ServicesHestoryViewModel : BaseViewModel
    {
        #region Fields & Private Variables

        private clsCust? _currentCustomer;
        private string _searchHistoryQuery = string.Empty;
        private clsService? _selectedHistoryService;
        private ObservableCollection<clsService> _servicesHistoryList;
        private List<clsService> _masterHistoryRecords;
        private readonly clsServicesManger _servicesManager;

        private int _currentPage = 1;
        private int _totalRows = 0;
        private int _pageSize = 10;

        #endregion

        #region Constructor

        public Cust_ServicesHestoryViewModel(long CustomerId)
        {
            _servicesManager = new clsServicesManger();
            ServicesHistoryList = new ObservableCollection<clsService>();
            _masterHistoryRecords = new List<clsService>();

            SelectServiceCommand = new RelayCommand(ExecuteSelectService, CanExecuteSelectService);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            CloseCommand = new RelayCommand<Window>(ExecuteClose);
            PageChangedCommand = new RelayCommand(ExecutePageChanged);

            _ = LoadCustomerDataAsync(CustomerId);
            _ = LoadData();
        }

        #endregion

        #region Properties

        public clsCust? CurrentCustomer
        {
            get => _currentCustomer;
            set
            {
                _currentCustomer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullPatientName));
            }
        }

        public string FullPatientName
        {
            get
            {
                if (CurrentCustomer == null) return string.Empty;
                return $"{CurrentCustomer.CUST_F_NAME} {CurrentCustomer.CUST_S_NAME} {CurrentCustomer.CUST_T_NAME} {CurrentCustomer.CUST_L_NAME}".Trim();
            }
        }

        public string SearchHistoryQuery
        {
            get => _searchHistoryQuery;
            set
            {
                _searchHistoryQuery = value;
                OnPropertyChanged();
                FilterServicesHistory();
            }
        }

        public clsService? SelectedHistoryService
        {
            get => _selectedHistoryService;
            set { _selectedHistoryService = value; OnPropertyChanged(); }
        }

        public ObservableCollection<clsService> ServicesHistoryList
        {
            get => _servicesHistoryList;
            set { _servicesHistoryList = value; OnPropertyChanged(); }
        }

        #region Pagination Properties

        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(); }
        }

        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand SelectServiceCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Command Methods & Actions

        private void ExecuteSelectService(object? parameter)
        {
            if (SelectedHistoryService == null) return;

            // إذا تم تمرير النافذة عبر CommandParameter (عند النقر المزدوج أو النقر على اختيار)
            if (parameter is Window window)
            {
                window.DialogResult = true; // إرجاع نتيجة النافذة بنجاح
                window.Close();
            }
            else
            {
                MessageBox.Show($"تم اختيار الإجراء: {SelectedHistoryService.SER_NAME} للمريض: {FullPatientName}",
                                "تأكيد الاختيار", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanExecuteSelectService(object? parameter)
        {
            return SelectedHistoryService != null;
        }

        private void ExecuteClearSearch(object? parameter)
        {
            SearchHistoryQuery = string.Empty;
        }

        private void ExecuteClose(Window? window)
        {
            window?.Close();
        }

        private void ExecutePageChanged(object? parameter)
        {
            // معالجة التصفح (Pagination)
        }

        #endregion

        #region Helper Methods

        private async Task LoadCustomerDataAsync(long? CustomerId)
        {
            CurrentCustomer = await clsCustomersManager.GetCustomerByIdAsync(CustomerId);
        }

        private async Task LoadData()
        {
            await LoadCustHistoryAsync();
        }

        public async Task LoadCustHistoryAsync()
        {
            if (CurrentCustomer == null || CurrentCustomer.CUST_ID == 0) return;

            try
            {
                var result = await _servicesManager.GetServiceByIdAsync(CurrentCustomer.CUST_ID);

                _masterHistoryRecords.Clear();

                if (result != null)
                {
                    _masterHistoryRecords.Add(result);
                    ServicesHistoryList = new ObservableCollection<clsService>(_masterHistoryRecords);
                }
                else
                {
                    ServicesHistoryList = new ObservableCollection<clsService>();
                }

                TotalRows = ServicesHistoryList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل سجل إجراءات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterServicesHistory()
        {
            if (_masterHistoryRecords == null) return;

            if (string.IsNullOrWhiteSpace(SearchHistoryQuery))
            {
                ServicesHistoryList = new ObservableCollection<clsService>(_masterHistoryRecords);
            }
            else
            {
                var filtered = _masterHistoryRecords.Where(s =>
                    (!string.IsNullOrEmpty(s.SER_NAME) && s.SER_NAME.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SER_CODE) && s.SER_CODE.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SER_TYPE) && s.SER_TYPE.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SER_NOTE) && s.SER_NOTE.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                ServicesHistoryList = new ObservableCollection<clsService>(filtered);
            }

            TotalRows = ServicesHistoryList.Count;
        }

        #endregion
    }
}
