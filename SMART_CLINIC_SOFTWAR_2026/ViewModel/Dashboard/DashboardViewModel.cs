using BLL.Mangers.Apointments;
using BLL.Mangers.Orders;
using Core.CurrentSession;
using System;
using System.Threading.Tasks;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {

            _ = LoadDashboardDataAsync();
        }

        #region Properties 

        private string _currentClincName
        {  get; set; }

        public string CurrentClincName
        {
            get => _currentClincName;
            set
            {
                if (_currentClincName != value)
                {
                    _currentClincName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentUsername
        { get; set; }

        public string CurrentUsername
        {
            get => _currentUsername;
            set
            {
                if (_currentUsername != value)
                {
                    _currentUsername = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _waitingPatientsCount;

        public int WaitingPatientsCount
        {
            get => _waitingPatientsCount;
            set
            {
                if (_waitingPatientsCount != value)
                {
                    _waitingPatientsCount = value; 
                    OnPropertyChanged();   
                }
            }
        }

        private int _todayAppointmentsCount { get; set;  }
        public int TodayAppointmentsCount
        {
            get => _todayAppointmentsCount;
            set { 
                if (_todayAppointmentsCount != value)
                {
                    _todayAppointmentsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                WaitingPatientsCount = await OrdersManger.GetTotalOrdersCountAsync();
                TodayAppointmentsCount = await 
                    clsApointmentsManger.GetTotalAppointmentsCountAsync();
                CurrentClincName = clsCurrentSectioncs.CurrentClinc.CLI_NAME;
                CurrentUsername = clsCurrentSectioncs.CurrentUser.USER_NAME; 
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
