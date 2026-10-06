using SMART_CLINIC_SOFTWAR_2026.ViewModel.Users;
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

namespace SMART_CLINIC_SOFTWAR_2026.View.Users
{
    /// <summary>
    /// Interaction logic for UserMangementWindow.xaml
    /// </summary>
    public partial class UserMangementWindow : Window
    {
        public UserMangementWindow()
        {
            InitializeComponent();
            this.DataContext = new UsersMangementViewModel(); 
        }
    }
}
