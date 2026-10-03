using SMART_CLINIC_SOFTWAR_2026.ViewModel.MedCheck;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Shapes;

namespace SMART_CLINIC_SOFTWAR_2026.View.MedCheck
{
    /// <summary>
    /// Interaction logic for CustMedCheckFromVisitWindow.xaml
    /// </summary>
    public partial class CustMedCheckFromVisitWindow : Window
    {
        public CustMedCheckFromVisitWindow(long? CustID)
        {
            InitializeComponent();
            this.DataContext = new CustMedCheckFromVistViewModel(CustID); 
        }
    }
}
