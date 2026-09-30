using BLL.Mangers.Customers;
using BLL.Mangers.Diagnoses;  
using Core.Entites.Customer;
using Core.Entites.Diagnos;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Diagnoses
{
    public class Cust_DiagnosisHestoryViewModel : BaseViewModel
    {
        #region Fields & Private Variables

        private clsCust? _currentCustomer;
        private string _searchHistoryQuery;
        private clsDiagnos _selectedHistoryDiagnosis;
        private ObservableCollection<clsDiagnos> _diagnosisHistoryList;
        private List<clsDiagnos> _masterHistoryRecords;  
         
        private int _currentPage = 1;
        private int _totalRows = 0;
        private int _pageSize = 10;

        #endregion

        #region Constructor

        public Cust_DiagnosisHestoryViewModel(long CustomerId)
        {
       
            DiagnosisHistoryList = new ObservableCollection<clsDiagnos>();
            _masterHistoryRecords = new List<clsDiagnos>();

            SelectDiagnosisCommand = new RelayCommand(ExecuteSelectDiagnosis, CanExecuteSelectDiagnosis);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            CloseCommand = new RelayCommand<Window>(ExecuteClose);
            PageChangedCommand = new RelayCommand(ExecutePageChanged);

            _ = LoadCustomerDataAsync(CustomerId); 
           _ =  LoadData();  
        }


        #endregion

        #region Properties

        public clsCust CurrentCustomer
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
                FilterDiagnosisHistory();
            }
        }

        public clsDiagnos SelectedHistoryDiagnosis
        {
            get => _selectedHistoryDiagnosis;
            set { _selectedHistoryDiagnosis = value; OnPropertyChanged(); }
        }

        public ObservableCollection<clsDiagnos> DiagnosisHistoryList
        {
            get => _diagnosisHistoryList;
            set { _diagnosisHistoryList = value; OnPropertyChanged(); }
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

        public ICommand SelectDiagnosisCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Command Methods & Actions

        private void ExecuteSelectDiagnosis(object parameter)
        {
            if (SelectedHistoryDiagnosis == null) return;

            // إذا تم تمرير النافذة عبر CommandParameter (عند النقر المزدوج أو النقر على اختيار)
            if (parameter is Window window)
            {
                window.DialogResult = true; // إرجاع نتيجة النافذة بنجاح
                window.Close();
            }
            else
            {
                MessageBox.Show($"تم اختيار التشخيص: {SelectedHistoryDiagnosis.DIG_NAME} للمريض: {FullPatientName}",
                                "تأكيد الاختيار", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanExecuteSelectDiagnosis(object parameter)
        {
            return SelectedHistoryDiagnosis != null;
        }

        private void ExecuteClearSearch(object parameter)
        {
            SearchHistoryQuery = string.Empty;
        }

        private void ExecuteClose(Window window)
        {
            window?.Close();
        }

        private void ExecutePageChanged(object parameter)
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
                var result = await clsDiagnosesManger.GetDiagnosByIdAsync(CurrentCustomer.CUST_ID);

                _masterHistoryRecords.Clear();

                if (result != null)
                {
                    _masterHistoryRecords.Add(result);
                    DiagnosisHistoryList = new ObservableCollection<clsDiagnos>(_masterHistoryRecords);
                }
                else
                {
                    DiagnosisHistoryList = new ObservableCollection<clsDiagnos>();
                }

                TotalRows = DiagnosisHistoryList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل سجل تشخيصات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void FilterDiagnosisHistory()
        {
            if (_masterHistoryRecords == null) return;

            if (string.IsNullOrWhiteSpace(SearchHistoryQuery))
            {
                DiagnosisHistoryList = new ObservableCollection<clsDiagnos>(_masterHistoryRecords);
            }
            else
            {
                var filtered = _masterHistoryRecords.Where(d =>
                    (!string.IsNullOrEmpty(d.DIG_NAME) && d.DIG_NAME.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    d.DIG_CODE.ToString().Contains(SearchHistoryQuery) ||
                    (!string.IsNullOrEmpty(d.DIG_NOTE) && d.DIG_NOTE.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                DiagnosisHistoryList = new ObservableCollection<clsDiagnos>(filtered);
            }

            TotalRows = DiagnosisHistoryList.Count;
        }

        #endregion
    }
}
