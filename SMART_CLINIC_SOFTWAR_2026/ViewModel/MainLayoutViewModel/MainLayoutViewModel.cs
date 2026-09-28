using Core.CurrentSession;
using Core.Entites.Clinc;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.View.Doctors;
using SMART_CLINIC_SOFTWAR_2026.View.LoginView;
using SMART_CLINIC_SOFTWAR_2026.View.Clinc; 
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using SMART_CLINIC_SOFTWAR_2026.View.Company;
using SMART_CLINIC_SOFTWAR_2026.View.Services;
using SMART_CLINIC_SOFTWAR_2026.View.Diagnose;
using SMART_CLINIC_SOFTWAR_2026.View.Medicens;
using SMART_CLINIC_SOFTWAR_2026.View.Card;
using SMART_CLINIC_SOFTWAR_2026.View.Customers;
using SMART_CLINIC_SOFTWAR_2026.View.Apointments;
using SMART_CLINIC_SOFTWAR_2026.View.Resption;
using SMART_CLINIC_SOFTWAR_2026.View.Vists; 
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Security.Cryptography.Xml;
using SMART_CLINIC_SOFTWAR_2026.View.Users;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.MainLayoutViewModel
{
    public class TabItemModel
    {
        public string Title { get; set; } = string.Empty;
        public object? ViewContent { get; set; }
    }

    public class MainLayoutViewModel : BaseViewModel
    {
        #region Properties
        public ObservableCollection<TabItemModel> OpenTabs { get; set; } = new ObservableCollection<TabItemModel>();

        public clsUser CurrentUser => clsCurrentSectioncs.CurrentUser;

        public clsClinc CurrentClinc => clsCurrentSectioncs.CurrentClinc; 
        

        public string CLI_NAME
        {
            set { CurrentClinc.CLI_NAME = value; }
            get { return CurrentClinc.CLI_NAME; }
        }


        public string CLI_LOC
        {
            set { CurrentClinc.CLI_LOC = value; }
            get { return CurrentClinc.CLI_LOC; }
        }


        public long CLI_CODE
        {
            set { CurrentClinc.CLI_CODE = value; }
            get { return CurrentClinc.CLI_CODE; }
        }

        #region Dashboard Properties

        private int _waitingPatientsCount = 0;
        public int WaitingPatientsCount
        {
            get => _waitingPatientsCount;
            set
            {
                _waitingPatientsCount = value;
                OnPropertyChanged();
            }
        }

        private int _todayAppointmentsCount = 0;
        public int TodayAppointmentsCount
        {
            get => _todayAppointmentsCount;
            set
            {
                _todayAppointmentsCount = value;
                OnPropertyChanged();
            }
        }

        // دالة لتحديث أو جلب البيانات من الخدمات / قاعدة البيانات
        public void LoadDashboardData()
        {
            // مثال: اسند القيم الحقيقية هنا من Database أو Services
            WaitingPatientsCount = 5;
            TodayAppointmentsCount = 12;
        }

        #endregion

        #endregion

        private TabItemModel? _selectedTab;
        public TabItemModel? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        #region Navigation Commands
        public ICommand NavigateToReceptionCommand { get; }
        public ICommand NavigateToPatientsCommand { get; }
        public ICommand NavigateToAppointmentsCommand { get; }
        public ICommand NavigateToDoctorsCommand { get; }
        public ICommand NavigateToClinicsCommand { get; }
        public ICommand NavigateToMedicalDeptCommand { get; }
        public ICommand NavigateToInsuranceCommand { get; }
        public ICommand NavigateToAccountsCommand { get; }
        public ICommand NavigateToReportsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToAboutCommand { get; }
        public ICommand OpenTestingCommand { get; }

        #endregion

        #region Tab & Session Commands
        public ICommand CloseTabCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ExitAppCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        #endregion

        public MainLayoutViewModel()
        {
            NavigateToReceptionCommand = new RelayCommand(ExecuteNavigateToReception);
            NavigateToPatientsCommand = new RelayCommand(ExecuteNavigateToPatients);
            NavigateToAppointmentsCommand = new RelayCommand(ExecuteNavigateToAppointments);
            NavigateToDoctorsCommand = new RelayCommand(ExecuteNavigateToDoctors);
            NavigateToClinicsCommand = new RelayCommand(ExecuteNavigateToClinics);
            NavigateToMedicalDeptCommand = new RelayCommand(ExecuteNavigateToMedicalDept);
            NavigateToInsuranceCommand = new RelayCommand(ExecuteNavigateToInsurance);
            NavigateToAccountsCommand = new RelayCommand(ExecuteNavigateToAccounts);
            NavigateToReportsCommand = new RelayCommand(ExecuteNavigateToReports);
            NavigateToSettingsCommand = new RelayCommand(ExecuteNavigateToSettings);
            NavigateToAboutCommand = new RelayCommand(ExecuteNavigateToAbout);

            CloseTabCommand = new RelayCommand(ExecuteCloseTab);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            ExitAppCommand = new RelayCommand(ExecuteExitApp);
            ChangePasswordCommand = new RelayCommand(ExecuteChangePassword);

            OpenTestingCommand = new RelayCommand(OpenTestingWindo);

           //ExecuteNavigateToDoctors(null);
        }

        #region Helper Methods - Tabs System
        private void OpenOrSelectTab(string title, object? viewContent)
        {
            var existingTab = OpenTabs.FirstOrDefault(t => t.Title == title);
            if (existingTab != null)
            {
                SelectedTab = existingTab;
            }
            else
            {
                var newTab = new TabItemModel
                {
                    Title = title,
                    ViewContent = viewContent
                };
                OpenTabs.Add(newTab);
                SelectedTab = newTab;
            }
        }

        private void ExecuteCloseTab(object? parameter)
        {
            if (parameter is TabItemModel tabToRemove)
            {
                OpenTabs.Remove(tabToRemove);

                if (SelectedTab == tabToRemove)
                {
                    SelectedTab = OpenTabs.LastOrDefault();
                }
            }
        }
        #endregion

        #region Methods - Navigation Execution
        private void ExecuteNavigateToReception(object? parameter)
        {
            OpenOrSelectTab("استقبال المرضى", new Cust_Resption());
        }

        private void ExecuteNavigateToPatients(object? parameter)
        {
            OpenOrSelectTab("المرضى", new CustomersView());
        }

        private void ExecuteNavigateToAppointments(object? parameter)
        {
            OpenOrSelectTab("المواعيد", new ApointmentsView());
        }

        private void ExecuteNavigateToDoctors(object? parameter)
        {
            OpenOrSelectTab("الأطباء", new DoctorsView());
        }

        private void ExecuteNavigateToClinics(object? parameter)
        {
            OpenOrSelectTab("العيادات", new ClincView());
        }

        private void ExecuteNavigateToMedicalDept(object? parameter)
        {
            OpenOrSelectTab("القسم الطبي", new DiagnosesView());
        }

        private void ExecuteNavigateToInsurance(object? parameter)
        {
            OpenOrSelectTab("التأمين الصحي", new CompanesView());
        }

        private void ExecuteNavigateToAccounts(object? parameter)
        {
            OpenOrSelectTab("الحسابات", new VistView());
        }

        private void ExecuteNavigateToReports(object? parameter)
        {
            OpenOrSelectTab("التقارير", new CardsView()); 
        }

        private void ExecuteNavigateToSettings(object? parameter)
        {
            OpenOrSelectTab("الإعدادات", new MidecensView());
        }

        private void ExecuteNavigateToAbout(object? parameter)
        {
            OpenOrSelectTab("حول النظام", new ServicesView());
        }
        #endregion

        #region Methods - Session Execution
        private void ExecuteLogout(object? parameter)
        {
            var loginWindow = new LogWind();
            loginWindow.Show();

            var currentWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            currentWindow?.Close();
        }

        private void ExecuteExitApp(object? parameter)
        {
            Application.Current.Shutdown();
        }

        private void ExecuteChangePassword(object? parameter)
        {
            var changePasswordWindow = new ChangeUserPasswordWindow(); 
            changePasswordWindow.Show ();
        }

        private void OpenTestingWindo(object? parameter)
        {
            AppointmentsListWindow doctorsWindow = new AppointmentsListWindow();

            doctorsWindow.ShowDialog();
        }


        #endregion

    }
}
