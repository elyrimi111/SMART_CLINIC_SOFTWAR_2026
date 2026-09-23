using BLL.Mangers.Clinc;
using Core.Entites.Clinc;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Clinc
{
    public class ClincListViewModel : BaseViewModel
    {
        private readonly clsClincManager _clincManager;

        public ClincListViewModel()
        {
            _clincManager = new clsClincManager();
            ClinicsList = new ObservableCollection<clsClinc>();

            _pageSize = 10;
            _currentPage = 1;

            LoadClinicsCommand = new RelayCommand(param => _ = LoadClinics());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(param => _ = LoadClinics());

            SelectClinicCommand = new RelayCommand(param => SelectClinic(param));

            _ = LoadClinics();
        }

        #region Properties (الخصائص)

        private ObservableCollection<clsClinc> _clinicsList = new();
        public ObservableCollection<clsClinc> ClinicsList
        {
            get => _clinicsList;
            set
            {
                _clinicsList = value;
                OnPropertyChanged();
            }
        }

        private clsClinc? _selectedClinic;
        public clsClinc? SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                if (_selectedClinic != value)
                {
                    _selectedClinic = value;
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
                    _ = LoadClinics();
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
                    _ = LoadClinics();
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

        public ICommand LoadClinicsCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand SelectClinicCommand { get; }

        #endregion

        #region Methods (العمليات)

        private void SelectClinic(object? param)
        {
            if (SelectedClinic != null && param is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public async Task LoadClinics()
        {
            try
            {
                TotalRows = await _clincManager.GetTotalClinicsCountAsync(SearchQuery);

                var pagedClinics = await _clincManager.GetClinicsPagedAsync(CurrentPage, PageSize, SearchQuery)
                                  ?? new List<clsClinc>();

                ClinicsList = new ObservableCollection<clsClinc>(pagedClinics);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات العيادات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        #endregion
    }
}
