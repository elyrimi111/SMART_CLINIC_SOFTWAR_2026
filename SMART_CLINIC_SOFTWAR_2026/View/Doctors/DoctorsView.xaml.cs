using System.Windows.Controls;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Doctors
{
    public partial class DoctorsView : UserControl
    {
        public DoctorsView()
        {
            InitializeComponent();
            this.DataContext = new DoctorsViewModel();
        }
    }
}
