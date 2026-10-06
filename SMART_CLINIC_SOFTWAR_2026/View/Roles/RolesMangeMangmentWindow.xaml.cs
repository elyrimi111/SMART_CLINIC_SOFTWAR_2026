using SMART_CLINIC_SOFTWAR_2026.ViewModel.Roles;
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

namespace SMART_CLINIC_SOFTWAR_2026.View.Roles
{
    /// <summary>
    /// Interaction logic for RolesMangeMangmentWindow.xaml
    /// </summary>
    public partial class RolesMangeMangmentWindow : Window
    {
        public RolesMangeMangmentWindow(long CustId)
        {
            InitializeComponent();
            this.DataContext = new RolesViewModel();
        }
    }
}
