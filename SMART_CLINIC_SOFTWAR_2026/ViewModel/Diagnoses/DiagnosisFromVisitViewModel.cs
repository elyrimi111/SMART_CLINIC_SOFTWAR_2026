using BLL.Mangers.Customers;
using BLL.Mangers.Diagnoses;
using Core.Entites.Customer;
using Core.Entites.Diagnos;
using SMART_CLINIC_SOFTWAR_2026.View.Diagnose;
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
    public class DiagnosisFromVisitViewModel : BaseViewModel
    {
        private readonly clsDiagnosesManger _diagnosesManager;

        #region Constructor

        public DiagnosisFromVisitViewModel(long? CustId)
        {
            _diagnosesManager = new clsDiagnosesManger();

            Customer = new clsCust();
            VisitDiagnosesList = new ObservableCollection<clsDiagnos>();
            AllDiagnosesList = new ObservableCollection<clsDiagnos>();

            _allDiagnosesPageSize = 14;
            _allDiagnosesCurrentPage = 1;

            _visitPageSize = 11;
            _visitPage = 1;

            DeleteAllCommand = new RelayCommand(param => ExecuteDeleteAll(), param => CanExecuteVisitSelected());
            DeleteCommand = new RelayCommand(param => ExecuteDelete(), param => CanExecuteVisitSelected());
            PreviousDiagnosisCommand = new RelayCommand(param => ExecutePreviousDiagnosis());
            NewDiagnosisCommand = new RelayCommand(param => ExecuteNewDiagnosis());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AllDiagnosesPageChangedCommand = new RelayCommand(async param => await LoadAllDiagnoses());
            VisitPageChangedCommand = new RelayCommand(param => ApplyVisitPagination());

            _ = LoadCustomerDataAsync(CustId);
            _ = LoadAllDiagnoses();
        }

        #endregion

        #region Properties

        private clsCust _customer;
        public clsCust Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullPatientName));
            }
        }

        public string FullPatientName
        {
            get
            {
                if (Customer == null) return string.Empty;
                return $"{Customer.CUST_F_NAME} {Customer.CUST_S_NAME} {Customer.CUST_T_NAME} {Customer.CUST_L_NAME}".Trim();
            }
        }

        private ObservableCollection<clsDiagnos> _visitDiagnosesList;
        public ObservableCollection<clsDiagnos> VisitDiagnosesList
        {
            get => _visitDiagnosesList;
            set
            {
                _visitDiagnosesList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<clsDiagnos> _allDiagnosesList;
        public ObservableCollection<clsDiagnos> AllDiagnosesList
        {
            get => _allDiagnosesList;
            set
            {
                _allDiagnosesList = value;
                OnPropertyChanged();
            }
        }

        private clsDiagnos? _selectedVisitDiagnosis;
        public clsDiagnos? SelectedVisitDiagnosis
        {
            get => _selectedVisitDiagnosis;
            set
            {
                _selectedVisitDiagnosis = value;
                OnPropertyChanged();
            }
        }

        private clsDiagnos? _selectedAllDiagnosis;
        public clsDiagnos? SelectedAllDiagnosis
        {
            get => _selectedAllDiagnosis;
            set
            {
                _selectedAllDiagnosis = value;
                OnPropertyChanged();
                if (_selectedAllDiagnosis != null)
                {
                    AddDiagnosisToVisit(_selectedAllDiagnosis);
                }
            }
        }

        private string? _searchDiagnosisQuery;
        public string? SearchDiagnosisQuery
        {
            get => _searchDiagnosisQuery;
            set
            {
                if (_searchDiagnosisQuery != value)
                {
                    _searchDiagnosisQuery = value;
                    OnPropertyChanged();
                    AllDiagnosesCurrentPage = 1;
                    _ = LoadAllDiagnoses();
                }
            }
        }

        #region Pagination Properties (All Diagnoses Grid)

        private int _allDiagnosesPageSize;
        public int AllDiagnosesPageSize
        {
            get => _allDiagnosesPageSize;
            set
            {
                if (_allDiagnosesPageSize != value && value > 0)
                {
                    _allDiagnosesPageSize = value;
                    OnPropertyChanged();
                    AllDiagnosesCurrentPage = 1;
                    _ = LoadAllDiagnoses();
                }
            }
        }

        private int _allDiagnosesTotalRows;
        public int AllDiagnosesTotalRows
        {
            get => _allDiagnosesTotalRows;
            set
            {
                _allDiagnosesTotalRows = value;
                OnPropertyChanged();
            }
        }

        private int _allDiagnosesCurrentPage;
        public int AllDiagnosesCurrentPage
        {
            get => _allDiagnosesCurrentPage;
            set
            {
                if (_allDiagnosesCurrentPage != value)
                {
                    _allDiagnosesCurrentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Pagination Properties (Visit Grid)

        private List<clsDiagnos> _masterVisitDiagnoses = new List<clsDiagnos>();

        private int _visitPageSize;
        public int VisitPageSize
        {
            get => _visitPageSize;
            set
            {
                if (_visitPageSize != value && value > 0)
                {
                    _visitPageSize = value;
                    OnPropertyChanged();
                    VisitPage = 1;
                    ApplyVisitPagination();
                }
            }
        }

        private int _visitTotalRows;
        public int VisitTotalRows
        {
            get => _visitTotalRows;
            set
            {
                _visitTotalRows = value;
                OnPropertyChanged();
            }
        }

        private int _visitPage;
        public int VisitPage
        {
            get => _visitPage;
            set
            {
                if (_visitPage != value)
                {
                    _visitPage = value;
                    OnPropertyChanged();
                    ApplyVisitPagination();
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand DeleteAllCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PreviousDiagnosisCommand { get; }
        public ICommand NewDiagnosisCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AllDiagnosesPageChangedCommand { get; }
        public ICommand VisitPageChangedCommand { get; }

        #endregion

        #region Methods & Actions

        public async Task LoadAllDiagnoses()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchDiagnosisQuery))
                {
                    var allDiagnoses = await clsDiagnosesManger.GetAllDiagnosAsync() ?? new List<clsDiagnos>();

                    AllDiagnosesTotalRows = allDiagnoses.Count;

                    var pagedResult = allDiagnoses
                        .Skip((AllDiagnosesCurrentPage - 1) * AllDiagnosesPageSize)
                        .Take(AllDiagnosesPageSize)
                        .ToList();

                    AllDiagnosesList = new ObservableCollection<clsDiagnos>(pagedResult);
                }
                else
                {
                    var allDiagnoses = await clsDiagnosesManger.GetAllDiagnosAsync() ?? new List<clsDiagnos>();

                    var filteredList = allDiagnoses.Where(d =>
                        d != null && (
                            (!string.IsNullOrEmpty(d.DIG_NAME) && d.DIG_NAME.IndexOf(SearchDiagnosisQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            d.DIG_CODE.ToString().Contains(SearchDiagnosisQuery.Trim())
                        )
                    ).ToList();

                    AllDiagnosesTotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((AllDiagnosesCurrentPage - 1) * AllDiagnosesPageSize)
                        .Take(AllDiagnosesPageSize)
                        .ToList();

                    AllDiagnosesList = new ObservableCollection<clsDiagnos>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل قائمة التشخيصات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchDiagnosisQuery = string.Empty;
        }

        private async Task LoadCustomerDataAsync(long? customerId)
        {
            if (!customerId.HasValue || customerId.Value == 0) return;

            try
            {
                var custData = await clsCustomersManager.GetCustomerByIdAsync(customerId);
                if (custData != null)
                {
                    Customer = custData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات العميل: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDiagnosisToVisit(clsDiagnos diagnosis)
        {
            if (diagnosis == null) return;

            if (!_masterVisitDiagnoses.Any(d => d.DIG_ID == diagnosis.DIG_ID))
            {
                _masterVisitDiagnoses.Add(diagnosis);
                ApplyVisitPagination();
            }
        }

        private void ApplyVisitPagination()
        {
            VisitTotalRows = _masterVisitDiagnoses.Count;

            var pagedResult = _masterVisitDiagnoses
                .Skip((VisitPage - 1) * VisitPageSize)
                .Take(VisitPageSize)
                .ToList();

            VisitDiagnosesList = new ObservableCollection<clsDiagnos>(pagedResult);
        }

        private void ExecuteDelete()
        {
            if (SelectedVisitDiagnosis != null)
            {
                _masterVisitDiagnoses.Remove(SelectedVisitDiagnosis);
                ApplyVisitPagination();
            }
        }

        private void ExecuteDeleteAll()
        {
            if (MessageBox.Show("هل أنت متأكد من حذف جميع تشخيصات هذه الزيارة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _masterVisitDiagnoses.Clear();
                VisitPage = 1;
                ApplyVisitPagination();
            }
        }

        private bool CanExecuteVisitSelected()
        {
            return _masterVisitDiagnoses != null && _masterVisitDiagnoses.Any();
        }

        private void ExecutePreviousDiagnosis()
        {
            if (Customer == null || Customer.CUST_ID == 0)
            {
                MessageBox.Show("لم يتم تحديد المريض لعرض السجل السابق له.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var previousDiagnosisWin = new Cust_DiagnosisHestoryWindow(Customer.CUST_ID);
            previousDiagnosisWin.ShowDialog();
        }

        private void ExecuteNewDiagnosis()
        {
            var diagnosesView = new DiagnosesView();
            var popupWindow = new Window
            {
                Title = "إدارة التشخيصات",
                Content = diagnosesView,
                Width = 850,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                FlowDirection = FlowDirection.RightToLeft,
                Background = (System.Windows.Media.Brush)Application.Current.FindResource("BgMainBrush")
            };

            popupWindow.ShowDialog();
            _ = LoadAllDiagnoses();
        }

        #endregion
    }
}
