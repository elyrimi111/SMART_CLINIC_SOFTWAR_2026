using BLL.Mangers.Dcotros;
using Core.Entites.Doctors;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors
{
    public class DoctorsListViewModel : BaseViewModel
    {
        private readonly clsDoctorsManger _doctorsManager;

        public DoctorsListViewModel()
        {
            _doctorsManager = new clsDoctorsManger();
            DoctorsList = new ObservableCollection<clsDoctors>();

            _pageSize = 10;
            _currentPage = 1;

            LoadDoctorsCommand = new RelayCommand(param => LoadDoctors());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(param => LoadDoctors());

            SelectDoctorCommand = new RelayCommand(param => SelectDoctor(param));

            LoadDoctors();
        }

        #region Properties  

        private ObservableCollection<clsDoctors> _doctorsList;
        public ObservableCollection<clsDoctors> DoctorsList
        {
            get => _doctorsList;
            set
            {
                _doctorsList = value;
                OnPropertyChanged();
            }
        }

        private clsDoctors? _selectedDoctor;
        public clsDoctors? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (_selectedDoctor != value)
                {
                    _selectedDoctor = value;
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
                    LoadDoctors();
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
                    LoadDoctors();
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

        #region Commands  

        public ICommand LoadDoctorsCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand SelectDoctorCommand { get; }

        #endregion

        #region Methods  

        private void SelectDoctor(object? param)
        {
            if (SelectedDoctor != null && param is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public async Task LoadDoctors()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    TotalRows = await _doctorsManager.GetTotalDoctorsCountAsync();

                    var pagedDoctors = await _doctorsManager.GetDoctorsPagedAsync(CurrentPage, PageSize)
                                      ?? new List<clsDoctors>();

                    DoctorsList = new ObservableCollection<clsDoctors>(pagedDoctors);
                }
                else
                {
                    var allDoctors = await _doctorsManager.GetAllDoctorsAsync() ?? new List<clsDoctors>();

                    var filteredList = allDoctors.Where(doc =>
                        doc != null && (
                            (!string.IsNullOrEmpty(doc.DOC_NAME) && doc.DOC_NAME.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(doc.DOC_MAJOR) && doc.DOC_MAJOR.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(doc.DOC_MOBILE) && doc.DOC_MOBILE.Contains(SearchQuery.Trim()))
                        )
                    ).ToList();

                    TotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    DoctorsList = new ObservableCollection<clsDoctors>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات الأطباء", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        #endregion
    }
}
