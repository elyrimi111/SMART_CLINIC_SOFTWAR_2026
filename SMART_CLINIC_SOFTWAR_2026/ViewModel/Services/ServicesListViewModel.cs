using BLL.Mangers.Services;
using Core.Entites.Services;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Services
{
    public class ServicesListViewModel : INotifyPropertyChanged
    {
        private readonly clsServicesManger _servicesManager;

        public ServicesListViewModel()
        {
            _servicesManager = new clsServicesManger();
            ServicesList = new ObservableCollection<clsService>();

            _pageSize = 10;
            _currentPage = 1;

            LoadServicesCommand = new RelayCommand(param => _ = LoadServices());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(param => _ = LoadServices());

            _ = LoadServices();
        }

        #region Properties (الخصائص)

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
                    _ = LoadServices();
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
                    _ = LoadServices();
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
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Methods (العمليات)

        public async Task LoadServices()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    TotalRows = await _servicesManager.GetTotalServicesCountAsync();

                    var pagedServices = await _servicesManager.GetServicesPagedAsync(CurrentPage, PageSize)
                                      ?? new List<clsService>();

                    ServicesList = new ObservableCollection<clsService>(pagedServices);
                }
                else
                {
                    var allServices = await _servicesManager.GetAllServicesAsync() ?? new List<clsService>();

                    var filteredList = allServices.Where(service =>
                        service != null && (
                            (!string.IsNullOrEmpty(service.SER_NAME) && service.SER_NAME.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(service.SER_CODE) && service.SER_CODE.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(service.SER_TYPE) && service.SER_TYPE.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                    ).ToList();

                    TotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    ServicesList = new ObservableCollection<clsService>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات الخدمات", MessageBoxButton.OK, MessageBoxImage.Error);
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
