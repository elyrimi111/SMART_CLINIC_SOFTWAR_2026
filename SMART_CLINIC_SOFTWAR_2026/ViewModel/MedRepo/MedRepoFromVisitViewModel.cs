using BLL.Mangers.Customers;
using BLL.Mangers.MedRepo;
using Core.CurrentSession;
using Core.Entites.Customer;
using Core.Entites.Med_Report;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.MedRepo
{
    public class MedRepoFromVisitViewModel : BaseViewModel
    {
        private readonly clsMedRepoManager _medRepoManager;

        #region Constructor

        public MedRepoFromVisitViewModel(long? custId, long? visitId = null, long? clinicId = null)
        {
            _medRepoManager = new clsMedRepoManager();

            Customer = new clsCust();
            CurrentReport = new clsMed_Report
            {
                CUST_ID = custId ?? 0,
                VIS_ID = visitId ?? 0,
                CLI_ID = clinicId ?? 0
            };

            VisitMedReportsList = new ObservableCollection<clsMed_Report>();

            _visitPageSize = 10;
            _visitPage = 1;

            // Commands Initialization
            NewMedReportCommand = new RelayCommand(async param => await ExecuteNewMedReportAsync());
            SaveMedReportCommand = new RelayCommand(async param => await ExecuteSaveMedReportAsync(), param => CanExecuteSave());
            PrintMedReportCommand = new RelayCommand(param => ExecutePrintMedReport(), param => CanExecutePrint());
            DeleteCommand = new RelayCommand(async param => await ExecuteDeleteAsync(), param => CanExecuteSelected());
            DeleteAllCommand = new RelayCommand(param => ExecuteDeleteAll(), param => CanExecuteVisitSelected());

            VisitPageChangedCommand = new RelayCommand(param => ApplyVisitPagination());

            _ = LoadCustomerDataAsync(custId);
            _ = LoadPatientMedReportsAsync();
            _ = ExecuteNewMedReportAsync();
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

        private clsMed_Report _currentReport;
        public clsMed_Report CurrentReport
        {
            get => _currentReport;
            set
            {
                _currentReport = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<clsMed_Report> _visitMedReportsList;
        public ObservableCollection<clsMed_Report> VisitMedReportsList
        {
            get => _visitMedReportsList;
            set
            {
                _visitMedReportsList = value;
                OnPropertyChanged();
            }
        }

        private clsMed_Report? _selectedVisitMedReport;
        public clsMed_Report? SelectedVisitMedReport
        {
            get => _selectedVisitMedReport;
            set
            {
                _selectedVisitMedReport = value;
                OnPropertyChanged();
                if (_selectedVisitMedReport != null)
                {
                    CurrentReport = new clsMed_Report
                    {
                        MREP_ID = _selectedVisitMedReport.MREP_ID,
                        MREP_CODE = _selectedVisitMedReport.MREP_CODE,
                        MREP_DATE = _selectedVisitMedReport.MREP_DATE,
                        MREP_TIME = _selectedVisitMedReport.MREP_TIME,
                        MREP_NAME = _selectedVisitMedReport.MREP_NAME,
                        MREP_TEXT = _selectedVisitMedReport.MREP_TEXT,
                        MREP_NOTE = _selectedVisitMedReport.MREP_NOTE,
                        CUST_ID = _selectedVisitMedReport.CUST_ID,
                        CLI_ID = _selectedVisitMedReport.CLI_ID,
                        VIS_ID = _selectedVisitMedReport.VIS_ID
                    };
                }
            }
        }

        #region Pagination Properties (Visit Grid)

        private List<clsMed_Report> _masterVisitReports = new List<clsMed_Report>();

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

        public ICommand NewMedReportCommand { get; }
        public ICommand SaveMedReportCommand { get; }
        public ICommand PrintMedReportCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteAllCommand { get; }
        public ICommand VisitPageChangedCommand { get; }

        #endregion

        #region Methods & Actions

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

        private async Task LoadPatientMedReportsAsync()
        {
            try
            {
                var reports = await _medRepoManager.GetAllMedReportsAsync() ?? new List<clsMed_Report>();

                if (Customer != null && Customer.CUST_ID > 0)
                {
                    _masterVisitReports = reports.Where(r => r.CUST_ID == Customer.CUST_ID).ToList();
                }
                else
                {
                    _masterVisitReports = reports;
                }

                ApplyVisitPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل التقارير الطبية: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteNewMedReportAsync()
        {
            try
            {
                long newCode = await _medRepoManager.GetNewMrep_CodeAsync();
                CurrentReport = new clsMed_Report
                {
                    MREP_CODE = newCode,
                    MREP_DATE = DateTime.Now.Date,
                    MREP_TIME = DateTime.Now.TimeOfDay,
                    CUST_ID = Customer?.CUST_ID ?? CurrentReport?.CUST_ID ?? 0,
                    CLI_ID = CurrentReport?.CLI_ID ?? 0,
                    VIS_ID = CurrentReport?.VIS_ID ?? 0
                };
                SelectedVisitMedReport = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء توليد كود التقرير الجديد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteSaveMedReportAsync()
        {
            try
            {
                CurrentReport.CUST_ID = Customer?.CUST_ID ?? CurrentReport.CUST_ID;
                CurrentReport.MREP_DATE = CurrentReport.MREP_DATE ?? DateTime.Now.Date;
                CurrentReport.MREP_TIME = CurrentReport.MREP_TIME ?? DateTime.Now.TimeOfDay;
                CurrentReport.CLI_ID = clsCurrentSectioncs.CurrentClinc.CLI_ID; 

                if (CurrentReport.MREP_ID > 0)
                {
                    bool isUpdated = await _medRepoManager.UpdateMedReportAsync(CurrentReport);
                    if (isUpdated)
                    {
                        MessageBox.Show("تم تعديل التقرير الطبي بنجاح.", "تم الحفظ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    long newId = await _medRepoManager.AddMedReportAsync(CurrentReport);
                    if (newId > 0)
                    {
                        MessageBox.Show("تم حفظ التقرير الطبي بنجاح.", "تم الحفظ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                await LoadPatientMedReportsAsync();
                await ExecuteNewMedReportAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ التقرير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteSave()
        {
            return CurrentReport != null && !string.IsNullOrWhiteSpace(CurrentReport.MREP_NAME) && !string.IsNullOrWhiteSpace(CurrentReport.MREP_TEXT);
        }

        private void ExecutePrintMedReport()
        {
            try
            {
                MessageBox.Show("جاري توجيه التقرير الطبي إلى الطباعة...", "طباعة التقرير", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الطباعة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecutePrint()
        {
            return CurrentReport != null && !string.IsNullOrWhiteSpace(CurrentReport.MREP_TEXT);
        }

        private void ApplyVisitPagination()
        {
            VisitTotalRows = _masterVisitReports.Count;

            var pagedResult = _masterVisitReports
                .Skip((VisitPage - 1) * VisitPageSize)
                .Take(VisitPageSize)
                .ToList();

            VisitMedReportsList = new ObservableCollection<clsMed_Report>(pagedResult);
        }

        private async Task ExecuteDeleteAsync()
        {
            if (SelectedVisitMedReport != null)
            {
                if (MessageBox.Show("هل أنت متأكد من حذف هذا التقرير الطبي؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (SelectedVisitMedReport.MREP_ID > 0)
                        {
                            await _medRepoManager.DeleteMedReportAsync(SelectedVisitMedReport.MREP_ID);
                        }

                        _masterVisitReports.Remove(SelectedVisitMedReport);
                        ApplyVisitPagination();
                        await ExecuteNewMedReportAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"حدث خطأ أثناء حذف التقرير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExecuteDeleteAll()
        {
            if (MessageBox.Show("هل أنت متأكد من مسح القائمة الحالية؟", "تأكيد المسح", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _masterVisitReports.Clear();
                VisitPage = 1;
                ApplyVisitPagination();
                _ = ExecuteNewMedReportAsync();
            }
        }

        private bool CanExecuteSelected()
        {
            return SelectedVisitMedReport != null;
        }

        private bool CanExecuteVisitSelected()
        {
            return _masterVisitReports != null && _masterVisitReports.Any();
        }

        #endregion
    }
}
