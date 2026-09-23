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
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Services; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Services
{
    /// <summary>
    /// Interaction logic for ServicesListWindo.xaml
    /// </summary>
    public partial class ServicesListWindo : Window
    {
        public ServicesListWindo()
        {
            InitializeComponent();
            this.DataContext = new ServicesListViewModel(); 
        }
    }
}
