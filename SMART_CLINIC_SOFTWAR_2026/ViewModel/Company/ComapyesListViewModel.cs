using BLL.Mangers.Companies;
using Core.Entites.Company;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Company
{
    public class ComapyesListViewModel : INotifyPropertyChanged
    {
        private readonly clsCompanyesManger _companyManager;

        public ComapyesListViewModel()
        {
            _companyManager = new clsCompanyesManger();
            CompaniesList = new ObservableCollection<clsCompany>();

            _pageSize = 10;
            _currentPage = 1;

            LoadCompaniesCommand = new RelayCommand(async param => await LoadCompanies());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(async param => await LoadCompanies());

            _ = LoadCompanies();
        }

        #region Properties (الخصائص)

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
                    _ = LoadCompanies();
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
                    _ = LoadCompanies();
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
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Methods (العمليات)

        public async Task LoadCompanies()
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
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات الشركات", MessageBoxButton.OK, MessageBoxImage.Error);
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
