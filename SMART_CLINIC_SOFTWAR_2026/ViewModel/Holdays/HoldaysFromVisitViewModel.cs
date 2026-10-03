using BLL.Mangers.Customers;
using BLL.Mangers.Holdays;
using Core.Entites.Customer;
using Core.Entites.Holday;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Holdays
{
    public class HoldaysFromVisitViewModel : BaseViewModel
    {
        private readonly clsHoldaysManger _holdaysManager;

        #region Constructor

        public HoldaysFromVisitViewModel(long? custId, long? visitId = null)
        {
            _holdaysManager = new clsHoldaysManger();

            Customer = new clsCust();
            VisitHolidaysList = new ObservableCollection<clsHolday>();

            _visitPageSize = 10;
            _visitPage = 1;
            _visitId = visitId;
             
            HOL_DATE = DateTime.Now;
            HOL_DURATION_VALUE = 1;
            HOL_DURATION_UNIT = "يوم";
             
            AddHolidayCommand = new RelayCommand(async param => await ExecuteAddHolidayAsync());
            DeleteCommand = new RelayCommand(async param => await ExecuteDeleteAsync(), param => CanExecuteVisitSelected());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());
            PrintHolidayCommand = new RelayCommand(param => ExecutePrintHoliday(), param => CanExecuteVisitSelected());

            VisitPageChangedCommand = new RelayCommand(param => ApplyVisitPagination());
             
            _ = InitializeViewModelAsync(custId);
        }

        private async Task InitializeViewModelAsync(long? custId)
        {
            await LoadNextHolidayCodeAsync();
            await LoadCustomerDataAsync(custId);
        }

        #endregion

        #region Private Fields & Identifiers

        private long? _visitId;

        #endregion

        #region Customer & Header Properties

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

        #endregion

        #region Form Input Properties 

        private long? _holCode;
        public long? HOL_CODE
        {
            get => _holCode;
            private set
            {
                _holCode = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _holDate;
        public DateTime? HOL_DATE
        {
            get => _holDate;
            set
            {
                _holDate = value;
                OnPropertyChanged();
            }
        }

        private int _holDurationValue;
        public int HOL_DURATION_VALUE
        {
            get => _holDurationValue;
            set
            {
                _holDurationValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullHolidayDuration));
            }
        }

        private string _holDurationUnit = "يوم";
        public string HOL_DURATION_UNIT
        {
            get => _holDurationUnit;
            set
            {
                _holDurationUnit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullHolidayDuration));
            }
        }

        public string FullHolidayDuration => $"{HOL_DURATION_VALUE} {HOL_DURATION_UNIT}";

        private string _holText = string.Empty;
        public string HOL_TEXT
        {
            get => _holText;
            set
            {
                _holText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Visit Holidays Grid Properties

        private ObservableCollection<clsHolday> _visitHolidaysList;
        public ObservableCollection<clsHolday> VisitHolidaysList
        {
            get => _visitHolidaysList;
            set
            {
                _visitHolidaysList = value;
                OnPropertyChanged();
            }
        }

        private clsHolday? _selectedVisitHoliday;
        public clsHolday? SelectedVisitHoliday
        {
            get => _selectedVisitHoliday;
            set
            {
                _selectedVisitHoliday = value;
                OnPropertyChanged();
                if (_selectedVisitHoliday != null)
                {
                    PopulateFieldsFromSelected(_selectedVisitHoliday);
                }
            }
        }

        #region Pagination Properties

        private List<clsHolday> _masterVisitHolidays = new List<clsHolday>();

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

        public ICommand AddHolidayCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand PrintHolidayCommand { get; }
        public ICommand VisitPageChangedCommand { get; }

        #endregion

        #region Helper Methods & Automatic Code Generation

        private async Task LoadNextHolidayCodeAsync()
        {
            try
            {
                HOL_CODE = await _holdaysManager.GetNewHol_CodeAsync();
            }
            catch
            {
                HOL_CODE = 1;
            }
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
                    await LoadPatientHolidaysAsync(customerId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadPatientHolidaysAsync(long customerId)
        {
            try
            {
                var allHolidays = await _holdaysManager.GetAllHolidaysAsync() ?? new List<clsHolday>();
                _masterVisitHolidays = allHolidays.Where(h => h.CUST_ID == customerId).ToList();
                ApplyVisitPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل سجل إجازات المريض: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ExecuteAddHolidayAsync()
        {
            if (Customer == null || Customer.CUST_ID == 0)
            {
                MessageBox.Show("يرجى تحديد مريض أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(HOL_TEXT))
            {
                MessageBox.Show("يرجى إدخال نص الإجازة المرضية.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var holiday = new clsHolday
                {
                    HOL_CODE = HOL_CODE,
                    HOL_NAME = "إجازة مرضية",
                    HOL_DATE = HOL_DATE,
                    HOL_TEXT = HOL_TEXT,
                    HOL_NOTE = $"المدة: {FullHolidayDuration}",
                    CUST_ID = Customer.CUST_ID,
                    VIS_ID = _visitId
                };

                long generatedId = await _holdaysManager.AddHolidayAsync(holiday);

                if (generatedId > 0)
                {
                    holiday.HOL_ID = generatedId;
                    MessageBox.Show("تم حفظ الإجازة المرضية بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    _masterVisitHolidays.Insert(0, holiday);
                    ApplyVisitPagination();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ الإجازة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateFieldsFromSelected(clsHolday holiday)
        {
            if (holiday == null) return;

            HOL_CODE = holiday.HOL_CODE;
            HOL_DATE = holiday.HOL_DATE;
            HOL_TEXT = holiday.HOL_TEXT;
             
            if (!string.IsNullOrWhiteSpace(holiday.HOL_NOTE) && holiday.HOL_NOTE.Contains("المدة:"))
            {
                try
                {
                    var rawDuration = holiday.HOL_NOTE.Replace("المدة:", "").Trim();  
                    var parts = rawDuration.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out int val))
                    {
                        HOL_DURATION_VALUE = val;
                        HOL_DURATION_UNIT = parts[1];
                    }
                }
                catch
                {
                    HOL_DURATION_VALUE = 1;
                    HOL_DURATION_UNIT = "يوم";
                }
            }
        }

        private async Task ClearFieldsAsync()
        {
            await LoadNextHolidayCodeAsync();
            HOL_DATE = DateTime.Now;
            HOL_DURATION_VALUE = 1;
            HOL_DURATION_UNIT = "يوم";
            HOL_TEXT = string.Empty;
            SelectedVisitHoliday = null;
        }

        private void ApplyVisitPagination()
        {
            VisitTotalRows = _masterVisitHolidays.Count;

            var pagedResult = _masterVisitHolidays
                .Skip((VisitPage - 1) * VisitPageSize)
                .Take(VisitPageSize)
                .ToList();

            VisitHolidaysList = new ObservableCollection<clsHolday>(pagedResult);
        }

        private async Task ExecuteDeleteAsync()
        {
            if (SelectedVisitHoliday == null) return;

            if (MessageBox.Show("هل أنت متأكد من حذف الإجازة المحددة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bool isDeleted = await _holdaysManager.DeleteHolidayAsync(SelectedVisitHoliday.HOL_ID);
                    if (isDeleted)
                    {
                        _masterVisitHolidays.Remove(SelectedVisitHoliday);
                        ApplyVisitPagination();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteVisitSelected()
        {
            return SelectedVisitHoliday != null || (_masterVisitHolidays != null && _masterVisitHolidays.Any());
        }

        private void ExecutePrintHoliday()
        {
            if (Customer == null || Customer.CUST_ID == 0)
            {
                MessageBox.Show("لم يتم تحديد المريض لطباعة الإجازة.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("جاري إعداد تقرير الإجازة المرضية للطباعة...", "طباعة الإجازة", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}
