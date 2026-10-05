using BLL.Mangers.Apointments;
using BLL.Mangers.Customers;
using Core.Entites.Customer;
using Core.Entities.Appointments;
using SMART_CLINIC_SOFTWAR_2026.View.Apointments;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Apointments
{
    public class AppointmentManagementViewModel : BaseViewModel
    {
        private readonly clsApointmentsManger _appointmentsManager;

        #region Constructors

        /// <summary>
        /// Default constructor for WPF Design-time support
        /// </summary>
        public AppointmentManagementViewModel() : this(null)
        {
        }

        public AppointmentManagementViewModel(long? custId)
        {
            _appointmentsManager = new clsApointmentsManger();

            _customer = new clsCust();
            _patientAppointmentsList = new ObservableCollection<clsAppointments>();

            _patientPageSize = 10;
            _patientCurrentPage = 1;

            NewAppointmentCommand = new RelayCommand(param => ExecuteNewAppointment());
            SearchPatientAppointmentsCommand = new RelayCommand(param => ExecuteSearchPatientAppointments());
            CancelAppointmentCommand = new RelayCommand(async param => await ExecuteCancelAppointmentAsync(), param => CanExecuteAppointmentSelected());
            DeleteAppointmentCommand = new RelayCommand(async param => await ExecuteDeleteAppointmentAsync(), param => CanExecuteAppointmentSelected());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            PatientPageChangedCommand = new RelayCommand(async param => await LoadPatientAppointmentsAsync());

            if (custId.HasValue && custId.Value > 0)
            {
                _ = LoadCustomerDataAsync(custId.Value);
            }
        }

        #endregion

        #region Properties

        private clsCust _customer;
        public clsCust Customer
        {
            get => _customer;
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FullPatientName));
                    _ = LoadPatientAppointmentsAsync();
                }
            }
        }

        public string FullPatientName
        {
            get
            {
                if (Customer == null) return string.Empty;
                string rawName = $"{Customer.CUST_F_NAME} {Customer.CUST_S_NAME} {Customer.CUST_T_NAME} {Customer.CUST_L_NAME}";
                return string.Join(" ", rawName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();
            }
        }

        private ObservableCollection<clsAppointments> _patientAppointmentsList;
        public ObservableCollection<clsAppointments> PatientAppointmentsList
        {
            get => _patientAppointmentsList;
            set
            {
                _patientAppointmentsList = value;
                OnPropertyChanged();
            }
        }

        private clsAppointments? _selectedPatientAppointment;
        public clsAppointments? SelectedPatientAppointment
        {
            get => _selectedPatientAppointment;
            set
            {
                _selectedPatientAppointment = value;
                OnPropertyChanged();
            }
        }

        private string? _searchPatientAppointmentQuery;
        public string? SearchPatientAppointmentQuery
        {
            get => _searchPatientAppointmentQuery;
            set
            {
                if (_searchPatientAppointmentQuery != value)
                {
                    _searchPatientAppointmentQuery = value;
                    OnPropertyChanged();
                    _patientCurrentPage = 1;
                    OnPropertyChanged(nameof(PatientCurrentPage));
                    _ = LoadPatientAppointmentsAsync();
                }
            }
        }

        #region Pagination Properties

        private int _patientPageSize;
        public int PatientPageSize
        {
            get => _patientPageSize;
            set
            {
                if (_patientPageSize != value && value > 0)
                {
                    _patientPageSize = value;
                    OnPropertyChanged();
                    _patientCurrentPage = 1;
                    OnPropertyChanged(nameof(PatientCurrentPage));
                    _ = LoadPatientAppointmentsAsync();
                }
            }
        }

        private int _patientTotalRows;
        public int PatientTotalRows
        {
            get => _patientTotalRows;
            set
            {
                _patientTotalRows = value;
                OnPropertyChanged();
            }
        }

        private int _patientCurrentPage;
        public int PatientCurrentPage
        {
            get => _patientCurrentPage;
            set
            {
                if (_patientCurrentPage != value && value > 0)
                {
                    _patientCurrentPage = value;
                    OnPropertyChanged();
                    _ = LoadPatientAppointmentsAsync();
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand NewAppointmentCommand { get; }
        public ICommand SearchPatientAppointmentsCommand { get; }
        public ICommand CancelAppointmentCommand { get; }
        public ICommand DeleteAppointmentCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand PatientPageChangedCommand { get; }

        #endregion

        #region Methods & Actions

        public async Task LoadPatientAppointmentsAsync()
        {
            if (Customer == null || Customer.CUST_ID <= 0)
            {
                PatientAppointmentsList.Clear();
                PatientTotalRows = 0;
                return;
            }

            try
            {
                var list = await _appointmentsManager.GetAppointmentsPagedAsync(
                    PatientCurrentPage,
                    PatientPageSize,
                    custName: FullPatientName,
                    docName: null,
                    fromDate: null,
                    toDate: null) ?? new List<clsAppointments>();

                if (!string.IsNullOrWhiteSpace(SearchPatientAppointmentQuery))
                {
                    string query = SearchPatientAppointmentQuery.Trim();
                    list = list.Where(a =>
                        a != null && (
                            (!string.IsNullOrEmpty(a.DOC_NAME) && a.DOC_NAME.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (a.APO_CODE.HasValue && a.APO_CODE.ToString()!.Contains(query)) ||
                            (!string.IsNullOrEmpty(a.APO_NOTE) && a.APO_NOTE.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                    ).ToList();
                }

                PatientAppointmentsList = new ObservableCollection<clsAppointments>(list);
                PatientTotalRows = await clsApointmentsManger.GetTotalAppointmentsCountAsync(SearchPatientAppointmentQuery);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في تحميل قائمة المواعيد", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCustomerDataAsync(long customerId)
        {
            if (customerId <= 0) return;

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

        private void ClearSearch()
        {
            SearchPatientAppointmentQuery = string.Empty;
        }

        private bool CanExecuteAppointmentSelected()
        {
            return SelectedPatientAppointment != null;
        }

        private void ExecuteNewAppointment()
        {
            var window = new Window
            {
                Content = new ApointmentsView(),
                Title = "حجز موعد جديد",
                Width = 900,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.ShowDialog();
        }


        private void ExecuteSearchPatientAppointments()
        {
            // فتح شاشة البحث الخاصة بالمواعيد
        }

        private async Task ExecuteCancelAppointmentAsync()
        {
            if (SelectedPatientAppointment == null) return;

            if (MessageBox.Show("هل أنت متأكد من إلغاء الموعد المحدد؟", "تأكيد الإلغاء", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    await LoadPatientAppointmentsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء إلغاء الموعد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task ExecuteDeleteAppointmentAsync()
        {
            if (SelectedPatientAppointment == null) return;

            if (MessageBox.Show("هل أنت متأكد من حذف الموعد المحدد نهائياً؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _appointmentsManager.DeleteAppointmentAsync(SelectedPatientAppointment.APO_ID);
                    if (success)
                    {
                        MessageBox.Show("تم حذف الموعد بنجاح.", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadPatientAppointmentsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء حذف الموعد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
