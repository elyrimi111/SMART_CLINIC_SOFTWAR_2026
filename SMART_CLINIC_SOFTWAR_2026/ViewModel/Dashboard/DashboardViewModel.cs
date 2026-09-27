using BLL.Mangers.Apointments;
using BLL.Mangers.Orders;
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
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
