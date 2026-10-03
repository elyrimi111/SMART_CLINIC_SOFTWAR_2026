using BLL.Mangers.Customers;
using BLL.Mangers.Medicens;
using Core.Entites.Customer;
using Core.Entites.Med_Check;
using Core.Entites.Medcin;
using SMART_CLINIC_SOFTWAR_2026.View.Medicens;
using SMART_CLINIC_SOFTWAR_2026.View.Vists;
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
    public class DosageLookupItem
    {
        public string DisplayText { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        // يمنع إظهار Namespace/Class Name في أي عنصر واجهة تلقائياً
        public override string ToString()
        {
            return DisplayText;
        }
    }

    public class CustDispensingMedicatonViewModel : BaseViewModel
    {
        #region Fields & Managers

        private readonly clsCustomersManager _customerManager;
        private readonly clsMedicenManeger _medicenManager;
        private readonly long? _custID;

        #endregion

        #region Properties & Data Binding

        private clsCust? _customer;
        public clsCust? Customer
        {
            get => _customer;
            set { _customer = value; OnPropertyChanged(); }
        }

        private ObservableCollection<clsMedcin> _allMedicationsList;
        public ObservableCollection<clsMedcin> AllMedicationsList
        {
            get => _allMedicationsList;
            set { _allMedicationsList = value; OnPropertyChanged(); }
        }

        private clsMedcin? _selectedAllMedication;
        public clsMedcin? SelectedAllMedication
        {
            get => _selectedAllMedication;
            set
            {
                _selectedAllMedication = value;
                OnPropertyChanged();
                if (_selectedAllMedication != null)
                {
                    SelectedMedicationName = _selectedAllMedication.MED_NAME;
                }
            }
        }

        private string? _selectedMedicationName;
        public string? SelectedMedicationName
        {
            get => _selectedMedicationName;
            set { _selectedMedicationName = value; OnPropertyChanged(); }
        }

        private string? _searchMedicationQuery;
        public string? SearchMedicationQuery
        {
            get => _searchMedicationQuery;
            set
            {
                _searchMedicationQuery = value;
                OnPropertyChanged();
                _ = FilterAndLoadMedicationsAsync();
            }
        }

        private int _allMedicationsCurrentPage = 1;
        public int AllMedicationsCurrentPage
        {
            get => _allMedicationsCurrentPage;
            set { _allMedicationsCurrentPage = value; OnPropertyChanged(); }
        }

        private int _allMedicationsTotalRows;
        public int AllMedicationsTotalRows
        {
            get => _allMedicationsTotalRows;
            set { _allMedicationsTotalRows = value; OnPropertyChanged(); }
        }

        private int _allMedicationsPageSize = 8;
        public int AllMedicationsPageSize
        {
            get => _allMedicationsPageSize;
            set { _allMedicationsPageSize = value; OnPropertyChanged(); }
        }

        #region ComboBox Lookup Lists (مع قيم حقيقية مخفية)

        public ObservableCollection<DosageLookupItem> DosageAmounts { get; set; } = new();
        public ObservableCollection<DosageLookupItem> DosageForms { get; set; } = new();
        public ObservableCollection<DosageLookupItem> DosageCounts { get; set; } = new();
        public ObservableCollection<DosageLookupItem> DosageTimings { get; set; } = new();

        #endregion

        #region ComboBox Selected Values (قيم نصية صافية مخفية للـ DataBase)

        private string? _selectedDosageAmount;
        public string? SelectedDosageAmount
        {
            get => _selectedDosageAmount;
            set { _selectedDosageAmount = value; OnPropertyChanged(); }
        }

        private string? _selectedDosageForm;
        public string? SelectedDosageForm
        {
            get => _selectedDosageForm;
            set { _selectedDosageForm = value; OnPropertyChanged(); }
        }

        private string? _selectedDosageCount;
        public string? SelectedDosageCount
        {
            get => _selectedDosageCount;
            set { _selectedDosageCount = value; OnPropertyChanged(); }
        }

        private string? _selectedDosageTiming;
        public string? SelectedDosageTiming
        {
            get => _selectedDosageTiming;
            set { _selectedDosageTiming = value; OnPropertyChanged(); }
        }

        #endregion

        private ObservableCollection<clsMed_Check> _visitMedicationsList;
        public ObservableCollection<clsMed_Check> VisitMedicationsList
        {
            get => _visitMedicationsList;
            set { _visitMedicationsList = value; OnPropertyChanged(); }
        }

        private clsMed_Check? _selectedVisitMedication;
        public clsMed_Check? SelectedVisitMedication
        {
            get => _selectedVisitMedication;
            set { _selectedVisitMedication = value; OnPropertyChanged(); }
        }

        private int _visitPage = 1;
        public int VisitPage
        {
            get => _visitPage;
            set { _visitPage = value; OnPropertyChanged(); }
        }

        private int _visitTotalRows;
        public int VisitTotalRows
        {
            get => _visitTotalRows;
            set { _visitTotalRows = value; OnPropertyChanged(); }
        }

        private int _visitPageSize = 10;
        public int VisitPageSize
        {
            get => _visitPageSize;
            set { _visitPageSize = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand NewMedicationCommand { get; }
        public ICommand PreviousMedicationCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand SaveDosageCommand { get; }
        public ICommand CancelDosageCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteAllCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AllMedicationsPageChangedCommand { get; }
        public ICommand VisitPageChangedCommand { get; }
        public ICommand SelectMedicationOnDoubleClickCommand { get; }

        #endregion

        #region Constructor

        public CustDispensingMedicatonViewModel(long? CustID)
        {
            _custID = CustID;
            _customerManager = new clsCustomersManager();
            _medicenManager = new clsMedicenManeger();

            AllMedicationsList = new ObservableCollection<clsMedcin>();
            VisitMedicationsList = new ObservableCollection<clsMed_Check>();

            InitializeDosageLookups();

            NewMedicationCommand = new RelayCommand(ExecuteNewMedication);
            PreviousMedicationCommand = new RelayCommand(ExecutePreviousMedication);
            PrintCommand = new RelayCommand(ExecutePrint);
            SaveDosageCommand = new RelayCommand(ExecuteSaveDosage, CanExecuteSaveDosage);
            CancelDosageCommand = new RelayCommand(ExecuteCancelDosage);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            DeleteAllCommand = new RelayCommand(ExecuteDeleteAll, CanExecuteDeleteAll);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);

            AllMedicationsPageChangedCommand = new RelayCommand(async (p) => await OnAllMedicationsPageChangedAsync(p));
            VisitPageChangedCommand = new RelayCommand(OnVisitPageChanged);

            SelectMedicationOnDoubleClickCommand = new RelayCommand(ExecuteSelectMedicationOnDoubleClick);

            _ = InitializeViewModelAsync();
        }

        #endregion

        #region Initialization Helpers

        private void InitializeDosageLookups()
        {
            DosageAmounts = new ObservableCollection<DosageLookupItem>
    {
        new DosageLookupItem { DisplayText = "0.5 مل", Value = "0.5" },
        new DosageLookupItem { DisplayText = "1 مل", Value = "1" },
        new DosageLookupItem { DisplayText = "1.5 مل", Value = "1.5" },
        new DosageLookupItem { DisplayText = "2 مل", Value = "2" },
        new DosageLookupItem { DisplayText = "2.5 مل", Value = "2.5" },
        new DosageLookupItem { DisplayText = "3 مل", Value = "3" },
        new DosageLookupItem { DisplayText = "5 مل", Value = "5" },
        new DosageLookupItem { DisplayText = "10 مل", Value = "10" },
        new DosageLookupItem { DisplayText = "15 مل", Value = "15" },
        new DosageLookupItem { DisplayText = "100 ملجم", Value = "100" },
        new DosageLookupItem { DisplayText = "250 ملجم", Value = "250" },
        new DosageLookupItem { DisplayText = "500 ملجم", Value = "500" },
        new DosageLookupItem { DisplayText = "1000 ملجم", Value = "1000" }
    };

            DosageForms = new ObservableCollection<DosageLookupItem>
    {
        new DosageLookupItem { DisplayText = "قرص", Value = "قرص" },
        new DosageLookupItem { DisplayText = "كبسولة", Value = "كبسولة" },
        new DosageLookupItem { DisplayText = "شراب", Value = "شراب" },
        new DosageLookupItem { DisplayText = "نقط", Value = "نقط" },
        new DosageLookupItem { DisplayText = "حقنة / أمبول", Value = "حقنة" },
        new DosageLookupItem { DisplayText = "مرهم", Value = "مرهم" },
        new DosageLookupItem { DisplayText = "كريم", Value = "كريم" },
        new DosageLookupItem { DisplayText = "بخاخ", Value = "بخاخ" }
    };

            DosageCounts = new ObservableCollection<DosageLookupItem>
    {
        new DosageLookupItem { DisplayText = "مرة واحدة يومياً", Value = "1" },
        new DosageLookupItem { DisplayText = "مرتين يومياً", Value = "2" },
        new DosageLookupItem { DisplayText = "3 مرات يومياً", Value = "3" },
        new DosageLookupItem { DisplayText = "4 مرات يومياً", Value = "4" },
        new DosageLookupItem { DisplayText = "كل 12 ساعة", Value = "12" },
        new DosageLookupItem { DisplayText = "كل 8 ساعات", Value = "8" },
        new DosageLookupItem { DisplayText = "كل 6 ساعات", Value = "6" },
        new DosageLookupItem { DisplayText = "عند الحاجة", Value = "عند الحاجة" }
    };

            DosageTimings = new ObservableCollection<DosageLookupItem>
    {
        new DosageLookupItem { DisplayText = "قبل الطعام", Value = "قبل الطعام" },
        new DosageLookupItem { DisplayText = "بعد الطعام", Value = "بعد الطعام" },
        new DosageLookupItem { DisplayText = "مع الطعام", Value = "مع الطعام" },
        new DosageLookupItem { DisplayText = "قبل النوم", Value = "قبل النوم" },
        new DosageLookupItem { DisplayText = "صباحاً", Value = "صباحاً" },
        new DosageLookupItem { DisplayText = "مساءً", Value = "مساءً" }
    };
        }

        private async Task InitializeViewModelAsync()
        {
            await LoadCustomerDataAsync();
            await LoadAllMedicationsPagedAsync();
        }

        #endregion

        #region Data Loading Methods

        private async Task LoadCustomerDataAsync()
        {
            if (_custID.HasValue && _custID.Value > 0)
            {
                try
                {
                    Customer = await clsCustomersManager.GetCustomerByIdAsync(_custID.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ أثناء جلب بيانات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task LoadAllMedicationsPagedAsync()
        {
            try
            {
                var medicines = await _medicenManager.GetMedicinesPagedAsync(
                    AllMedicationsCurrentPage,
                    AllMedicationsPageSize,
                    SearchMedicationQuery
                );

                AllMedicationsTotalRows = await _medicenManager.GetTotalMedicinesCountAsync(SearchMedicationQuery);
                AllMedicationsList = new ObservableCollection<clsMedcin>(medicines ?? new List<clsMedcin>());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل دليل الأدوية: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task FilterAndLoadMedicationsAsync()
        {
            AllMedicationsCurrentPage = 1;
            await LoadAllMedicationsPagedAsync();
        }

        #endregion

        #region Command Implementations

        private void ExecuteNewMedication(object? obj)
        {
            var visitControl = new MidecensView();

            var window = new Window
            {
                Title = "إضافة دواء جديد",
                Content = visitControl,
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                FlowDirection = FlowDirection.RightToLeft,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["BgMainBrush"]
            };

            window.ShowDialog();
        }

        private void ExecutePreviousMedication(object? obj)
        {
            if (Customer == null || Customer.CUST_ID <= 0)
            {
                MessageBox.Show("يرجى تحديد مريض أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var historyWindow = new CustVisitMedicensWindow(Customer.CUST_ID);
            historyWindow.ShowDialog();
        }

        private void ExecutePrint(object? obj)
        {
        }

        private void ExecuteSelectMedicationOnDoubleClick(object? obj)
        {
            var targetMedication = obj as clsMedcin ?? SelectedAllMedication;
            if (targetMedication != null)
            {
                SelectedAllMedication = targetMedication;
                SelectedMedicationName = targetMedication.MED_NAME;
            }
        }

        private void ExecuteSaveDosage(object? obj)
        {
            if (SelectedAllMedication == null)
            {
                MessageBox.Show("يرجى اختيار علاج أولاً من القائمة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // تجميع القيم الرقمية الصافية المحددة بدون الرموز الظاهرية
            string dosageDetails = string.Join(" - ", new[]
            {
                SelectedDosageAmount,
                SelectedDosageForm,
                SelectedDosageCount,
                SelectedDosageTiming
            }.Where(s => !string.IsNullOrWhiteSpace(s)));

            var checkItem = new clsMed_Check
            {
                MEDCHECK_CODE = SelectedAllMedication.MED_ID,
                MEDCHECK_NAME = SelectedAllMedication.MED_NAME,
                MEDCHECK_PRICE = SelectedAllMedication.MED_PRICE,
                MEDCHECK_NOTE = string.IsNullOrWhiteSpace(dosageDetails) ? "حسب إرشادات الطبيب" : dosageDetails,
                CLI_ID = SelectedAllMedication.CLI_ID
            };

            if (!VisitMedicationsList.Any(m => m.MEDCHECK_CODE == checkItem.MEDCHECK_CODE))
            {
                VisitMedicationsList.Add(checkItem);
            }
            else
            {
                MessageBox.Show("تم إضافة هذا الدواء سابقاً للزيارة الحالية.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            VisitTotalRows = VisitMedicationsList.Count;

            ExecuteCancelDosage(null);
        }

        private bool CanExecuteSaveDosage(object? obj)
        {
            return SelectedAllMedication != null;
        }

        private void ExecuteCancelDosage(object? obj)
        {
            SelectedDosageAmount = null;
            SelectedDosageForm = null;
            SelectedDosageCount = null;
            SelectedDosageTiming = null;
            SelectedAllMedication = null;
            SelectedMedicationName = string.Empty;
        }

        private void ExecuteDelete(object? obj)
        {
            if (SelectedVisitMedication != null)
            {
                VisitMedicationsList.Remove(SelectedVisitMedication);
                VisitTotalRows = VisitMedicationsList.Count;
                SelectedVisitMedication = null;
            }
        }

        private bool CanExecuteDelete(object? obj)
        {
            return SelectedVisitMedication != null;
        }

        private void ExecuteDeleteAll(object? obj)
        {
            if (VisitMedicationsList.Count > 0)
            {
                var result = MessageBox.Show("هل أنت متأكد من حذف جميع الأدوية المضافة للزيارة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    VisitMedicationsList.Clear();
                    VisitTotalRows = 0;
                    SelectedVisitMedication = null;
                }
            }
        }

        private bool CanExecuteDeleteAll(object? obj)
        {
            return VisitMedicationsList != null && VisitMedicationsList.Count > 0;
        }

        private void ExecuteClearSearch(object? obj)
        {
            SearchMedicationQuery = string.Empty;
        }

        private async Task OnAllMedicationsPageChangedAsync(object? obj)
        {
            await LoadAllMedicationsPagedAsync();
        }

        private void OnVisitPageChanged(object? obj)
        {
        }

        #endregion
    }
}
