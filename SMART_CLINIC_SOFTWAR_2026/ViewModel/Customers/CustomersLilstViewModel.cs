using BLL.Mangers.Customers;
using Core.Entites.Customer;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Customers
{
    public class CustomersListViewModel : INotifyPropertyChanged
    {
        private readonly clsCustomersManager _customersManager;

        public CustomersListViewModel()
        {
            _customersManager = new clsCustomersManager();
            CustomersList = new ObservableCollection<clsCust>();

            _pageSize = 10;
            _currentPage = 1;

            LoadCustomersCommand = new RelayCommand(param => _ = LoadCustomers());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(param => _ = LoadCustomers());

            SelectCustomerCommand = new RelayCommand(param => SelectCustomer(param));

            _ = LoadCustomers();
        }

        #region Properties (الخصائص)

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
                    _ = LoadCustomers();
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
                    _ = LoadCustomers();
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
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand SelectCustomerCommand { get; }

        #endregion

        #region Methods (العمليات)

        private void SelectCustomer(object? param)
        {
            if (SelectedCustomer != null && param is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public async Task LoadCustomers()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    TotalRows = await _customersManager.GetTotalCustomersCountAsync();

                    var pagedCustomers = await _customersManager.GetCustomersPagedAsync(CurrentPage, PageSize)
                                        ?? new List<clsCust>();

                    CustomersList = new ObservableCollection<clsCust>(pagedCustomers);
                }
                else
                {
                    var allCustomers = await _customersManager.GetAllCustomersAsync() ?? new List<clsCust>();

                    string query = SearchQuery.Trim();

                    var filteredList = allCustomers.Where(cust =>
                        cust != null && (
                            (!string.IsNullOrEmpty(cust.CUST_F_NAME) && cust.CUST_F_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(cust.CUST_S_NAME) && cust.CUST_S_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(cust.CUST_T_NAME) && cust.CUST_T_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(cust.CUST_L_NAME) && cust.CUST_L_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(cust.CUST_MOBILE1) && cust.CUST_MOBILE1.Contains(query)) ||
                            (!string.IsNullOrEmpty(cust.CUST_MOBILE2) && cust.CUST_MOBILE2.Contains(query)) ||
                            (!string.IsNullOrEmpty(cust.CUST_ADDRESS) && cust.CUST_ADDRESS.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                    ).ToList();

                    TotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    CustomersList = new ObservableCollection<clsCust>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات العملاء", MessageBoxButton.OK, MessageBoxImage.Error);
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
