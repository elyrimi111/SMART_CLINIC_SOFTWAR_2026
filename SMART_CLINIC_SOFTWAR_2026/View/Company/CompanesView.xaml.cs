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
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Company; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Company
{
    /// <summary>
    /// Interaction logic for CompanesView.xaml
    /// </summary>
    public partial class CompanesView : UserControl
    {
        public CompanesView()
        {
            InitializeComponent();
            this.DataContext = new CompanyViewModel();
        }
    }
}
