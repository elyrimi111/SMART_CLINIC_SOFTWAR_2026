using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Apointments; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Apointments
{
    /// <summary>
    /// Interaction logic for ApointmentsView.xaml
    /// </summary>
    public partial class ApointmentsView : UserControl
    {
        public ApointmentsView()
        {
            InitializeComponent();
            this.DataContext = new AppointmentsViewModel();
        }
    }
}
