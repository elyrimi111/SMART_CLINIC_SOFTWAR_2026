using System.Windows;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.LoginViewModel;

namespace SMART_CLINIC_SOFTWAR_2026.View.LoginView
{
    public partial class LogWind : Window
    {
        public LogWind()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();
        }
    }
}
