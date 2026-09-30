using BLL.Mangers.Customers;
using BLL.Mangers.Medicens;
using Core.Entites.Customer;
using Core.Entites.Medcin;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Medicens
{
    public class CustVisitMedcensViewModel : BaseViewModel
    {
        #region Fields & Private Variables

        private clsCust? _currentCustomer;
        private string _searchHistoryQuery;
        private clsMedcin _selectedHistoryMedication;
        private ObservableCollection<clsMedcin> _medicationHistoryList;
        private List<clsMedcin> _masterHistoryRecords;
        private readonly clsMedicenManeger _medicenManeger;

        private int _currentPage = 1;
        private int _totalRows = 0;
        private int _pageSize = 10;

        #endregion

        #region Constructor

        public CustVisitMedcensViewModel(long customerId)
        {
            _medicenManeger = new clsMedicenManeger();
            MedicationHistoryList = new ObservableCollection<clsMedcin>();
            _masterHistoryRecords = new List<clsMedcin>();

            SelectMedicationCommand = new RelayCommand(ExecuteSelectMedication, CanExecuteSelectMedication);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            CloseCommand = new RelayCommand<Window>(ExecuteClose);
            PageChangedCommand = new RelayCommand(ExecutePageChanged);

            _ = LoadCustomerDataAsync(customerId);
            _ = LoadData();
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
                FilterMedicationHistory();
            }
        }

        public clsMedcin SelectedHistoryMedication
        {
            get => _selectedHistoryMedication;
            set { _selectedHistoryMedication = value; OnPropertyChanged(); }
        }

        public ObservableCollection<clsMedcin> MedicationHistoryList
        {
            get => _medicationHistoryList;
            set { _medicationHistoryList = value; OnPropertyChanged(); }
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

        public ICommand SelectMedicationCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand PageChangedCommand { get; }

        #endregion

        #region Command Methods & Actions

        private void ExecuteSelectMedication(object parameter)
        {
            if (SelectedHistoryMedication == null) return;

            // إذا تم تمرير النافذة عبر CommandParameter (عند النقر المزدوج أو النقر على اختيار)
            if (parameter is Window window)
            {
                window.DialogResult = true; // إرجاع نتيجة النافذة بنجاح
                window.Close();
            }
            else
            {
                MessageBox.Show($"تم اختيار الدواء: {SelectedHistoryMedication.MED_NAME} للمريض: {FullPatientName}",
                                "تأكيد الاختيار", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanExecuteSelectMedication(object parameter)
        {
            return SelectedHistoryMedication != null;
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

        private async Task LoadCustomerDataAsync(long customerId)
        {
            CurrentCustomer = await clsCustomersManager.GetCustomerByIdAsync(customerId);
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
                // جلب الأدوية باستخدام المانجرclsMedicenManeger
                var result = await _medicenManeger.GetAllMedicinesAsync();

                _masterHistoryRecords.Clear();

                if (result != null && result.Count > 0)
                {
                    _masterHistoryRecords.AddRange(result);
                    MedicationHistoryList = new ObservableCollection<clsMedcin>(_masterHistoryRecords);
                }
                else
                {
                    MedicationHistoryList = new ObservableCollection<clsMedcin>();
                }

                TotalRows = MedicationHistoryList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل سجل أدوية المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterMedicationHistory()
        {
            if (_masterHistoryRecords == null) return;

            if (string.IsNullOrWhiteSpace(SearchHistoryQuery))
            {
                MedicationHistoryList = new ObservableCollection<clsMedcin>(_masterHistoryRecords);
            }
            else
            {
                var filtered = _masterHistoryRecords.Where(m =>
                    (!string.IsNullOrEmpty(m.MED_NAME) && m.MED_NAME.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.MED_S_NAME) && m.MED_S_NAME.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.MED_SOURES) && m.MED_SOURES.Contains(SearchHistoryQuery, StringComparison.OrdinalIgnoreCase)) ||
                    m.MED_CODE.ToString().Contains(SearchHistoryQuery) ||
                    m.MED_ID.ToString().Contains(SearchHistoryQuery)
                ).ToList();

                MedicationHistoryList = new ObservableCollection<clsMedcin>(filtered);
            }

            TotalRows = MedicationHistoryList.Count;
        }

        #endregion
    }
}
