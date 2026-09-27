using BLL.Mangers.Apointments;
using Core.Entities.Appointments;
using SMART_CLINIC_SOFTWAR_2026.View.Customers;
using SMART_CLINIC_SOFTWAR_2026.View.Doctors;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Customers;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Apointments
{
    public class AppointmentsViewModel : BaseViewModel
    {
        private readonly clsApointmentsManger _appointmentsManger;

        public AppointmentsViewModel()
        {
            _appointmentsManger = new clsApointmentsManger();
            AppointmentsList = new ObservableCollection<clsAppointments>();

            _pageSize = 15;
            _currentPage = 1;

            LoadAppointmentsCommand = new RelayCommand(async param => await LoadAppointments());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            PageChangedCommand = new RelayCommand(async param => await LoadAppointments());

            SelectAppointmentCommand = new RelayCommand(param => SelectAppointment(param), param => CanSelectAppointment());
            SaveAppointmentCommand = new RelayCommand(async param => await SaveAppointmentAsync());
            UpdateAppointmentCommand = new RelayCommand(async param => await UpdateAppointmentAsync(), param => CanUpdateOrDelete());
            DeleteAppointmentCommand = new RelayCommand(async param => await DeleteAppointmentAsync(), param => CanUpdateOrDelete());
            ClearFieldsCommand = new RelayCommand(async param => await PrepareNewAppointmentAsync());

            OpenCustomersListCommand = new RelayCommand(param => OpenCustomersList());
            OpenDoctorsListCommand = new RelayCommand(param => OpenDoctorsList());

            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            await LoadAppointments();
            await PrepareNewAppointmentAsync();
        }

        #region Properties

        private ObservableCollection<clsAppointments> _appointmentsList;
        public ObservableCollection<clsAppointments> AppointmentsList
        {
            get => _appointmentsList;
            set { _appointmentsList = value; OnPropertyChanged(); }
        }

        private clsAppointments? _selectedAppointment;
        public clsAppointments? SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                if (_selectedAppointment != value)
                {
                    _selectedAppointment = value;
                    OnPropertyChanged();

                    LoadSelectedAppointmentDetails();
                    RefreshCommandStates();
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
                    _ = LoadAppointments();
                }
            }
        }

        private long? _apo_ID;
        public long? APO_ID
        {
            get => _apo_ID;
            set { _apo_ID = value; OnPropertyChanged(); }
        }

        private long? _apo_CODE;
        public long? APO_CODE
        {
            get => _apo_CODE;
            set { _apo_CODE = value; OnPropertyChanged(); }
        }

        private DateTime? _apo_DATE;
        public DateTime? APO_DATE
        {
            get => _apo_DATE;
            set { _apo_DATE = value; OnPropertyChanged(); }
        }

        private string? _selectedHour;
        public string? SelectedHour
        {
            get => _selectedHour;
            set { _selectedHour = value; OnPropertyChanged(); CombineTime(); }
        }

        private string? _selectedMinute;
        public string? SelectedMinute
        {
            get => _selectedMinute;
            set { _selectedMinute = value; OnPropertyChanged(); CombineTime(); }
        }

        private string? _selectedPeriod = "AM";
        public string? SelectedPeriod
        {
            get => _selectedPeriod;
            set { _selectedPeriod = value; OnPropertyChanged(); CombineTime(); }
        }

        private TimeSpan? _apo_TIME;
        public TimeSpan? APO_TIME
        {
            get => _apo_TIME;
            set { _apo_TIME = value; OnPropertyChanged(); }
        }

        private string? _apo_note;
        public string? APO_NOTE
        {
            get => _apo_note;
            set { _apo_note = value; OnPropertyChanged(); }
        }

        private long? _cust_ID;
        public long? CUST_ID
        {
            get => _cust_ID;
            set { _cust_ID = value; OnPropertyChanged(); }
        }

        private string? _cust_NAME;
        public string? CUST_NAME
        {
            get => _cust_NAME;
            set { _cust_NAME = value; OnPropertyChanged(); }
        }

        private long? _doc_ID;
        public long? DOC_ID
        {
            get => _doc_ID;
            set { _doc_ID = value; OnPropertyChanged(); }
        }

        private string? _doc_NAME;
        public string? DOC_NAME
        {
            get => _doc_NAME;
            set { _doc_NAME = value; OnPropertyChanged(); }
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
                    _ = LoadAppointments();
                }
            }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(); }
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

        public ICommand LoadAppointmentsCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand SelectAppointmentCommand { get; }
        public ICommand SaveAppointmentCommand { get; }
        public ICommand UpdateAppointmentCommand { get; }
        public ICommand DeleteAppointmentCommand { get; }
        public ICommand ClearFieldsCommand { get; }
        public ICommand OpenCustomersListCommand { get; }
        public ICommand OpenDoctorsListCommand { get; }

        #endregion

        #region Methods

        public async Task LoadAppointments()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    TotalRows = await clsApointmentsManger.GetTotalAppointmentsCountAsync(null);

                    var pagedAppointments = await _appointmentsManger.GetAppointmentsPagedAsync(CurrentPage, PageSize, null)
                                           ?? new List<clsAppointments>();

                    AppointmentsList = new ObservableCollection<clsAppointments>(pagedAppointments);
                }
                else
                {
                    var allAppointments = await _appointmentsManger.GetAllAppointmentsAsync() ?? new List<clsAppointments>();
                    string query = SearchQuery.Trim();

                    var filteredList = allAppointments.Where(apo =>
                        apo != null && (
                            (!string.IsNullOrEmpty(apo.CUST_NAME) && apo.CUST_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (!string.IsNullOrEmpty(apo.DOC_NAME) && apo.DOC_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            apo.APO_CODE.ToString().Contains(query) ||
                            (apo.CUST_ID.HasValue && apo.CUST_ID.ToString()!.Contains(query)) ||
                            (apo.DOC_ID.HasValue && apo.DOC_ID.ToString()!.Contains(query))
                        )
                    ).ToList();

                    TotalRows = filteredList.Count;

                    var pagedResult = filteredList
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    AppointmentsList = new ObservableCollection<clsAppointments>(pagedResult);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل بيانات المواعيد", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CombineTime()
        {
            if (int.TryParse(SelectedHour, out int h) && int.TryParse(SelectedMinute, out int m))
            {
                if (SelectedPeriod == "PM" && h < 12)
                    h += 12;
                else if (SelectedPeriod == "AM" && h == 12)
                    h = 0;

                _apo_TIME = new TimeSpan(h, m, 0);
                OnPropertyChanged(nameof(APO_TIME));
            }
        }

        private void LoadSelectedAppointmentDetails()
        {
            if (SelectedAppointment != null)
            {
                _apo_ID = SelectedAppointment.APO_ID;
                _apo_CODE = SelectedAppointment.APO_CODE;
                _cust_ID = SelectedAppointment.CUST_ID;
                _cust_NAME = SelectedAppointment.CUST_NAME;
                _doc_ID = SelectedAppointment.DOC_ID;
                _doc_NAME = SelectedAppointment.DOC_NAME;
                _apo_DATE = SelectedAppointment.APO_DATE;
                _apo_TIME = SelectedAppointment.APO_TIME;
                _apo_note = SelectedAppointment.APO_NOTE;

                if (SelectedAppointment.APO_TIME.HasValue)
                {
                    int hours = SelectedAppointment.APO_TIME.Value.Hours;
                    _selectedPeriod = hours >= 12 ? "PM" : "AM";

                    int hour12 = hours % 12;
                    if (hour12 == 0) hour12 = 12;

                    _selectedHour = hour12.ToString("D2");
                    _selectedMinute = SelectedAppointment.APO_TIME.Value.Minutes.ToString("D2");
                }
                else
                {
                    _selectedHour = null;
                    _selectedMinute = null;
                    _selectedPeriod = "AM";
                }

                NotifyAllDetailsProperties();
            }
            else
            {
                ClearAppointmentFields();
            }
        }

        private void ClearAppointmentFields()
        {
            _apo_ID = null;
            _apo_CODE = null;
            _cust_ID = null;
            _cust_NAME = string.Empty;
            _doc_ID = null;
            _doc_NAME = string.Empty;
            _apo_DATE = null;
            _apo_TIME = null;
            _selectedHour = null;
            _selectedMinute = null;
            _selectedPeriod = "AM";
            _apo_note = string.Empty;

            NotifyAllDetailsProperties();
        }

        private void NotifyAllDetailsProperties()
        {
            OnPropertyChanged(nameof(APO_ID));
            OnPropertyChanged(nameof(APO_CODE));
            OnPropertyChanged(nameof(CUST_ID));
            OnPropertyChanged(nameof(CUST_NAME));
            OnPropertyChanged(nameof(DOC_ID));
            OnPropertyChanged(nameof(DOC_NAME));
            OnPropertyChanged(nameof(APO_DATE));
            OnPropertyChanged(nameof(APO_TIME));
            OnPropertyChanged(nameof(SelectedHour));
            OnPropertyChanged(nameof(SelectedMinute));
            OnPropertyChanged(nameof(SelectedPeriod));
            OnPropertyChanged(nameof(APO_NOTE));
        }

        public async Task PrepareNewAppointmentAsync()
        {
            ClearAppointmentFields();
            SelectedAppointment = null;
            try
            {
                APO_CODE = await _appointmentsManger.GetNewApo_CodeAsync();
                APO_DATE = DateTime.Today;

                var now = DateTime.Now;
                int hour12 = now.Hour % 12;
                if (hour12 == 0) hour12 = 12;

                SelectedPeriod = now.Hour >= 12 ? "PM" : "AM";
                SelectedHour = hour12.ToString("D2");
                SelectedMinute = now.Minute.ToString("D2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الموعد", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private async Task SaveAppointmentAsync()
        {
            try
            {
                var appointment = CreateAppointmentObject();
                long newId = await _appointmentsManger.AddAppointmentAsync(appointment);
                if (newId > 0)
                {
                    MessageBox.Show("تم حفظ الموعد بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAppointments();
                    await PrepareNewAppointmentAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ عند الحفظ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateAppointmentAsync()
        {
            try
            {
                var appointment = CreateAppointmentObject();
                appointment.APO_ID = APO_ID ?? 0;

                bool isUpdated = await _appointmentsManger.UpdateAppointmentAsync(appointment);
                if (isUpdated)
                {
                    MessageBox.Show("تم تعديل الموعد بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAppointments();
                    await PrepareNewAppointmentAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ عند التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteAppointmentAsync()
        {
            if (APO_ID <= 0) return;

            if (MessageBox.Show("هل أنت تأكد من حذف هذا الموعد؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bool isDeleted = await _appointmentsManger.DeleteAppointmentAsync(APO_ID.Value);
                    if (isDeleted)
                    {
                        MessageBox.Show("تم حذف الموعد بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadAppointments();
                        await PrepareNewAppointmentAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ عند الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private clsAppointments CreateAppointmentObject()
        {
            return new clsAppointments
            {
                APO_CODE = APO_CODE ?? 0,
                CUST_ID = CUST_ID,
                DOC_ID = DOC_ID,
                APO_DATE = APO_DATE,
                APO_TIME = APO_TIME,
                APO_NOTE = APO_NOTE
            };
        }

        private bool CanUpdateOrDelete() => APO_ID.HasValue && APO_ID.Value > 0;

        private bool CanSelectAppointment() => SelectedAppointment != null && SelectedAppointment.APO_ID > 0;

        private void RefreshCommandStates()
        {
            (SelectAppointmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (UpdateAppointmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteAppointmentCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void SelectAppointment(object? parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void OpenDoctorsList()
        {
            var doctorsWin = new DoctorsListWindow();
            if (Application.Current.MainWindow != null)
                doctorsWin.Owner = Application.Current.MainWindow;

            if (doctorsWin.ShowDialog() == true)
            {
                if (doctorsWin.DataContext is DoctorsListViewModel vm && vm.SelectedDoctor != null)
                {
                    DOC_ID = vm.SelectedDoctor.DOC_ID;
                    DOC_NAME = vm.SelectedDoctor.DOC_NAME;
                }
            }
        }

        private void OpenCustomersList()
        {
            var customersWin = new CustomersListWindow();
            if (Application.Current.MainWindow != null)
                customersWin.Owner = Application.Current.MainWindow;

            if (customersWin.ShowDialog() == true)
            {
                if (customersWin.DataContext is CustomersListViewModel vm && vm.SelectedCustomer != null)
                {
                    CUST_ID = vm.SelectedCustomer.CUST_ID;
                    CUST_NAME = $"{vm.SelectedCustomer.CUST_F_NAME} {vm.SelectedCustomer.CUST_T_NAME} {vm.SelectedCustomer.CUST_L_NAME}".Trim();
                }
            }
        }

        #endregion
    }
}
