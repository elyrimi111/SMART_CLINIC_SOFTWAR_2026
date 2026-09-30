using SMART_CLINIC_SOFTWAR_2026.ViewModel.Medicens;
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

namespace SMART_CLINIC_SOFTWAR_2026.View.Medicens
{
    /// <summary>
    /// Interaction logic for CustVisitMedicensWindow.xaml
    /// </summary>
    public partial class CustVisitMedicensWindow : Window
    {
        public CustVisitMedicensWindow(long CustID)
        {
            InitializeComponent();
            this.DataContext = new CustVisitMedcensViewModel(CustID);
        }
    }
}
