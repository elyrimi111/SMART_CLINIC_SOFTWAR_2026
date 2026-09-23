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
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Doctors; 

namespace SMART_CLINIC_SOFTWAR_2026.View.Doctors
{
    /// <summary>
    /// Interaction logic for DoctorsListWindow.xaml
    /// </summary>
    public partial class DoctorsListWindow : Window
    {
        public DoctorsListWindow()
        {
            InitializeComponent();
            this.DataContext = new DoctorsListViewModel();
        }
    }
}
