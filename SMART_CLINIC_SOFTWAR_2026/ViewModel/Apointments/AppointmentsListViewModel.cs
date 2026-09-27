using BLL.Mangers.Apointments;
using Core.Entities.Appointments;
using SMART_CLINIC_SOFTWAR_2026.View.Customers;
using SMART_CLINIC_SOFTWAR_2026.View.Doctors;
using SMART_CLINIC_SOFTWAR_2026.ViewModel;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Customers;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Apointments
{
    public class AppointmentsListViewModel : BaseViewModel
    {
        public enum enScreenMode { eChoseMode, eViewMode }
        private enScreenMode _screenMode;

        private readonly clsApointmentsManger _appointmentsManager;

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
            set { _selectedAppointment = value; OnPropertyChanged(); }
        }

        private string? _custName;
        public string? CUST_NAME
        {
            get => _custName;
            set
            {
                _custName = value;
                OnPropertyChanged();
                RefreshSearchCommandState();
            }
        }

        private string? _docName;
        public string? DOC_NAME
        {
            get => _docName;
            set
            {
                _docName = value;
                OnPropertyChanged();
                RefreshSearchCommandState();
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;
                OnPropertyChanged();
                RefreshSearchCommandState();
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                OnPropertyChanged();
                RefreshSearchCommandState();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(); }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; }
        public ICommand OpenCustomersListCommand { get; }
        public ICommand OpenDoctorsListCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand SelectAppointmentCommand { get; }

        #endregion

        public AppointmentsListViewModel(enScreenMode mode = enScreenMode.eViewMode)
        {
            _screenMode = mode;
            _appointmentsManager = new clsApointmentsManger();
            _appointmentsList = new ObservableCollection<clsAppointments>();

            SearchCommand = new RelayCommand(async p => await ExecuteSearchAsync(), p => CanExecuteSearch());
            ClearSearchCommand = new RelayCommand(p => ClearSearch());
            PageChangedCommand = new RelayCommand(async p => await LoadAppointmentsAsync());
            SelectAppointmentCommand = new RelayCommand(SelectAppointment);
            OpenCustomersListCommand = new RelayCommand(p => OpenCustomersList());
            OpenDoctorsListCommand = new RelayCommand(p => OpenDoctorsList());

            _ = LoadAppointmentsAsync();
        }

        #region Methods

        private bool CanExecuteSearch() => true;

        private void RefreshSearchCommandState()
        {
            (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private async Task ExecuteSearchAsync()
        {
            CurrentPage = 1;
            await LoadAppointmentsAsync();
        }

        public async Task LoadAppointmentsAsync()
        {
            try
            {
                DateOnly? fromDate = FromDate.HasValue ? DateOnly.FromDateTime(FromDate.Value) : null;
                DateOnly? toDate = ToDate.HasValue ? DateOnly.FromDateTime(ToDate.Value) : null;

                var list = await _appointmentsManager.GetAppointmentsPagedAsync(
                    CurrentPage,
                    PageSize,
                    CUST_NAME,
                    DOC_NAME,
                    fromDate,
                    toDate);

                AppointmentsList = new ObservableCollection<clsAppointments>(list);

                TotalRows = await clsApointmentsManger.GetTotalAppointmentsCountAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل المواعيد: " + ex.Message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            _custName = null;
            _docName = null;
            _fromDate = null;
            _toDate = null;
            _currentPage = 1;

            OnPropertyChanged(nameof(CUST_NAME));
            OnPropertyChanged(nameof(DOC_NAME));
            OnPropertyChanged(nameof(FromDate));
            OnPropertyChanged(nameof(ToDate));
            OnPropertyChanged(nameof(CurrentPage));

            RefreshSearchCommandState();
            _ = LoadAppointmentsAsync();
        }

        private void SelectAppointment(object? parameter)
        {
            if (SelectedAppointment == null || !(parameter is Window window)) return;

            if (ComponentDispatcher.IsThreadModal)
            {
                window.DialogResult = true; 
            }
            else
            {
                window.Close();
            }
        }

        private void OpenDoctorsList()
        {
            var doctorsWin = new DoctorsListWindow();

            var activeWindow = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.IsActive && w != doctorsWin);

            if (activeWindow != null)
            {
                doctorsWin.Owner = activeWindow;
            }

            if (doctorsWin.ShowDialog() == true)
            {
                if (doctorsWin.DataContext is DoctorsListViewModel vm && vm.SelectedDoctor != null)
                {
                    DOC_NAME = vm.SelectedDoctor.DOC_NAME;
                }
            }
        }

        private void OpenCustomersList()
        {
            var customersWin = new CustomersListWindow();

            var activeWindow = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.IsActive && w != customersWin);

            if (activeWindow != null)
            {
                customersWin.Owner = activeWindow;
            }

            if (customersWin.ShowDialog() == true)
            {
                if (customersWin.DataContext is CustomersListViewModel vm && vm.SelectedCustomer != null)
                {
                    CUST_NAME = $"{vm.SelectedCustomer.CUST_F_NAME} {vm.SelectedCustomer.CUST_T_NAME} {vm.SelectedCustomer.CUST_L_NAME}".Trim();
                }
            }
        }


        #endregion
    }
}
