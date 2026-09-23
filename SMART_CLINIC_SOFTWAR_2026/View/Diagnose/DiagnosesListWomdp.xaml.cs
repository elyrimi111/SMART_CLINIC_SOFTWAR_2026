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
using System.Windows.Shapes;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Diagnoses;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors;
using SMART_CLINIC_SOFTWAR_2026.View.Diagnose; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Diagnose
{
    /// <summary>
    /// Interaction logic for DiagnosesListWomdp.xaml
    /// </summary>
    public partial class DiagnosesListWomdp : Window
    {
        public DiagnosesListWomdp()
        {
            InitializeComponent();
            this.DataContext = new DiagnosesListViewModel();
        }
    }
}
