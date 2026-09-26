using System.Windows;
using System.Windows.Controls;
using SMART_CLINIC_SOFTWAR_2026.View.Doctors;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Dashboard; 
namespace SMART_CLINIC_SOFTWAR_2026.View.Dashboard
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            this.DataContext = new  DashboardViewModel(); 

        }

    }
}
