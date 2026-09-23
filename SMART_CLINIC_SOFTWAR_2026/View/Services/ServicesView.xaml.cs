 using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands; 
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
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Services; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Services
{
    /// <summary>
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    public partial class ServicesView : UserControl
    {
        public ServicesView()
        {
            InitializeComponent();
            this.DataContext = new ServicesViewModel(); 
        }
    }
}
