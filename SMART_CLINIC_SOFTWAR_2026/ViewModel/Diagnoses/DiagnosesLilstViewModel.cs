using BLL.Mangers.Diagnoses;
using Core.Entites.Diagnos;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Diagnoses
{
    public class DiagnosesListViewModel : INotifyPropertyChanged
    {
        private readonly clsDiagnosesManger _diagnosesManager;

        public DiagnosesListViewModel()
        {
            _diagnosesManager = new clsDiagnosesManger();
            DiagnosesList = new ObservableCollection<clsDiagnos>();

            _pageSize = 10;
            _currentPage = 1;

            LoadDiagnosesCommand = new RelayCommand(async param => await LoadDiagnoses());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(async param => await LoadDiagnoses());

            _ = LoadDiagnoses();
        }

        #region Properties (الخصائص)

        private ObservableCollection<clsDiagnos> _diagnosesList;
        public ObservableCollection<clsDiagnos> DiagnosesList
        {
            get => _diagnosesList;
            set
            {
                _diagnosesList = value;
                OnPropertyChanged();
            }
        }

        private clsDiagnos? _selectedDiagnos;
        public clsDiagnos? SelectedDiagnos
        {
            get => _selectedDiagnos;
            set
            {
                if (_selectedDiagnos != value)
                {
                    _selectedDiagnos = value;
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
                    _ = LoadDiagnoses();
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
                    _ = LoadDiagnoses();
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

        public ICommand LoadDiagnosesCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Methods (العمليات)

        public async Task LoadDiagnoses()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    TotalRows = await _diagnosesManager.GetTotalDiagnosCountAsync();

                    var pagedDiagnoses = await _diagnosesManager.GetDiagnosPagedAsync(CurrentPage, PageSize)
                                         ?? new List<clsDiagnos>();

                    DiagnosesList = new ObservableCollection<clsDiagnos>(pagedDiagnoses);
                }
                else
                {
                    var allDiagnoses = await clsDiagnosesManger.GetAllDiagnosAsync() ?? new List<clsDiagnos>();

                    var filteredList = allDiagnoses.Where(dig =>
                        dig != null && (
                            (!string.IsNullOrEmpty(dig.DIG_NAME) && dig.DIG_NAME.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(dig.DIG_TYPE) && dig.DIG_TYPE.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(dig.DIG_NOTE) && dig.DIG_NOTE.IndexOf(SearchQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            dig.DIG_CODE.ToString().Contains(SearchQuery.Trim())
                        )
                    ).ToList();

                    TotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    DiagnosesList = new ObservableCollection<clsDiagnos>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات التشخيصات", MessageBoxButton.OK, MessageBoxImage.Error);
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
