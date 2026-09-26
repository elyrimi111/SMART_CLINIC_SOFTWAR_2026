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

        #endregion

        #region Methods

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                WaitingPatientsCount = await OrdersManger.GetTotalOrdersCountAsync();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
