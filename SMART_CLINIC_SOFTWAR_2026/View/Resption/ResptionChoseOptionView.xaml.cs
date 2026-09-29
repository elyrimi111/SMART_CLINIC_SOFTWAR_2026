using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Resption;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART_CLINIC_SOFTWAR_2026.View.Resption
{
    /// <summary>
    /// Interaction logic for ResptionChoseOptionView.xaml
    /// </summary>
    public partial class ResptionChoseOptionView : UserControl
    {
        public ResptionChoseOptionView()
        {
            InitializeComponent();
            this.DataContext = new ResptionChoseOptionViewModel();
        }
    }
}
