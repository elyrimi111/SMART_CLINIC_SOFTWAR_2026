using BLL.Mangers.Customers;
using BLL.Mangers.MedCheck;
using Core.Entites.Customer;
using Core.Entites.Med_Check;
using SMART_CLINIC_SOFTWAR_2026.View.Diagnose;
using SMART_CLINIC_SOFTWAR_2026.View.MedCheck;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.MedCheck
{
    public class CustMedCheckFromVistViewModel : BaseViewModel
    {
        private readonly clsMedCheckManger _medCheckManager;

        #region Constructor

        public CustMedCheckFromVistViewModel(long? CustId)
        {
            _medCheckManager = new clsMedCheckManger();

            Customer = new clsCust();
            VisitMedChecksList = new ObservableCollection<clsMed_Check>();
            AllMedChecksList = new ObservableCollection<clsMed_Check>();

            _allMedChecksPageSize = 14;
            _allMedChecksCurrentPage = 1;

            _visitPageSize = 11;
            _visitPage = 1;

            DeleteAllCommand = new RelayCommand(param => ExecuteDeleteAll(), param => CanExecuteVisitSelected());
            DeleteCommand = new RelayCommand(param => ExecuteDelete(), param => CanExecuteVisitSelected());
            PreviousDiagnosisCommand = new RelayCommand(param => ExecutePreviousDiagnosis());
            NewMedCheckCommand = new RelayCommand(param => ExecuteNewMedCheck());
            PrintCommand = new RelayCommand(param => ExecutePrint());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AllMedChecksPageChangedCommand = new RelayCommand(async param => await LoadAllMedChecks());
            VisitPageChangedCommand = new RelayCommand(param => ApplyVisitPagination());

            _ = LoadCustomerDataAsync(CustId);
            _ = LoadAllMedChecks();
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

        private ObservableCollection<clsMed_Check> _visitMedChecksList;
        public ObservableCollection<clsMed_Check> VisitMedChecksList
        {
            get => _visitMedChecksList;
            set
            {
                _visitMedChecksList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<clsMed_Check> _allMedChecksList;
        public ObservableCollection<clsMed_Check> AllMedChecksList
        {
            get => _allMedChecksList;
            set
            {
                _allMedChecksList = value;
                OnPropertyChanged();
            }
        }

        private clsMed_Check? _selectedVisitMedCheck;
        public clsMed_Check? SelectedVisitMedCheck
        {
            get => _selectedVisitMedCheck;
            set
            {
                _selectedVisitMedCheck = value;
                OnPropertyChanged();
            }
        }

        private clsMed_Check? _selectedAllMedCheck;
        public clsMed_Check? SelectedAllMedCheck
        {
            get => _selectedAllMedCheck;
            set
            {
                _selectedAllMedCheck = value;
                OnPropertyChanged();
                if (_selectedAllMedCheck != null)
                {
                    AddMedCheckToVisit(_selectedAllMedCheck);
                }
            }
        }

        private string? _searchMedCheckQuery;
        public string? SearchMedCheckQuery
        {
            get => _searchMedCheckQuery;
            set
            {
                if (_searchMedCheckQuery != value)
                {
                    _searchMedCheckQuery = value;
                    OnPropertyChanged();
                    AllMedChecksCurrentPage = 1;
                    _ = LoadAllMedChecks();
                }
            }
        }

        #region Pagination Properties (All MedChecks Grid)

        private int _allMedChecksPageSize;
        public int AllMedChecksPageSize
        {
            get => _allMedChecksPageSize;
            set
            {
                if (_allMedChecksPageSize != value && value > 0)
                {
                    _allMedChecksPageSize = value;
                    OnPropertyChanged();
                    AllMedChecksCurrentPage = 1;
                    _ = LoadAllMedChecks();
                }
            }
        }

        private int _allMedChecksTotalRows;
        public int AllMedChecksTotalRows
        {
            get => _allMedChecksTotalRows;
            set
            {
                _allMedChecksTotalRows = value;
                OnPropertyChanged();
            }
        }

        private int _allMedChecksCurrentPage;
        public int AllMedChecksCurrentPage
        {
            get => _allMedChecksCurrentPage;
            set
            {
                if (_allMedChecksCurrentPage != value)
                {
                    _allMedChecksCurrentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Pagination Properties (Visit Grid)

        private List<clsMed_Check> _masterVisitMedChecks = new List<clsMed_Check>();

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
        public ICommand NewMedCheckCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AllMedChecksPageChangedCommand { get; }
        public ICommand VisitPageChangedCommand { get; }

        #endregion

        #region Methods & Actions

        public async Task LoadAllMedChecks()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchMedCheckQuery))
                {
                    var allMedChecks = await _medCheckManager.GetAllMedChecksAsync() ?? new List<clsMed_Check>();

                    AllMedChecksTotalRows = allMedChecks.Count;

                    var pagedResult = allMedChecks
                        .Skip((AllMedChecksCurrentPage - 1) * AllMedChecksPageSize)
                        .Take(AllMedChecksPageSize)
                        .ToList();

                    AllMedChecksList = new ObservableCollection<clsMed_Check>(pagedResult);
                }
                else
                {
                    var allMedChecks = await _medCheckManager.GetAllMedChecksAsync() ?? new List<clsMed_Check>();

                    var filteredList = allMedChecks.Where(m =>
                        m != null && (
                            (!string.IsNullOrEmpty(m.MEDCHECK_NAME) && m.MEDCHECK_NAME.IndexOf(SearchMedCheckQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(m.MEDCHECK_TYPE) && m.MEDCHECK_TYPE.IndexOf(SearchMedCheckQuery.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) ||
                            m.MEDCHECK_CODE.ToString().Contains(SearchMedCheckQuery.Trim())
                        )
                    ).ToList();

                    AllMedChecksTotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((AllMedChecksCurrentPage - 1) * AllMedChecksPageSize)
                        .Take(AllMedChecksPageSize)
                        .ToList();

                    AllMedChecksList = new ObservableCollection<clsMed_Check>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل قائمة الفحوصات الطبية", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchMedCheckQuery = string.Empty;
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
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMedCheckToVisit(clsMed_Check medCheck)
        {
            if (medCheck == null) return;

            if (!_masterVisitMedChecks.Any(m => m.MEDCHECK_ID == medCheck.MEDCHECK_ID))
            {
                _masterVisitMedChecks.Add(medCheck);
                ApplyVisitPagination();
            }
        }

        private void ApplyVisitPagination()
        {
            VisitTotalRows = _masterVisitMedChecks.Count;

            var pagedResult = _masterVisitMedChecks
                .Skip((VisitPage - 1) * VisitPageSize)
                .Take(VisitPageSize)
                .ToList();

            VisitMedChecksList = new ObservableCollection<clsMed_Check>(pagedResult);
        }

        private void ExecuteDelete()
        {
            if (SelectedVisitMedCheck != null)
            {
                _masterVisitMedChecks.Remove(SelectedVisitMedCheck);
                ApplyVisitPagination();
            }
        }

        private void ExecuteDeleteAll()
        {
            if (MessageBox.Show("هل أنت متأكد من حذف جميع فحوصات هذه الزيارة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _masterVisitMedChecks.Clear();
                VisitPage = 1;
                ApplyVisitPagination();
            }
        }

        private bool CanExecuteVisitSelected()
        {
            return _masterVisitMedChecks != null && _masterVisitMedChecks.Any();
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

        private void ExecuteNewMedCheck()
        {
            var medCheckView = new MedCheckView();

            var popupWindow = new Window
            {
                Title = "إدارة الفحوصات الطبية",
                Content = medCheckView,
                Width = 950,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                FlowDirection = FlowDirection.RightToLeft,
                Background = (System.Windows.Media.Brush)Application.Current.FindResource("BgMainBrush")
            };

            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                               ?? Application.Current.MainWindow;

            if (activeWindow != null && activeWindow != popupWindow)
            {
                popupWindow.Owner = activeWindow;
            }

            popupWindow.ShowDialog();

            _ = LoadAllMedChecks();
        }

        private void ExecutePrint()
        {
            if (!_masterVisitMedChecks.Any())
            {
                MessageBox.Show("لا توجد فحوصات مضافة لهذه الزيارة لطباعتها.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // تنفيذ كود الطباعة الخاص بالفحوصات الطبية
            MessageBox.Show("جاري إرسال طلب الفحوصات إلى الطباعة...", "طباعة", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}


