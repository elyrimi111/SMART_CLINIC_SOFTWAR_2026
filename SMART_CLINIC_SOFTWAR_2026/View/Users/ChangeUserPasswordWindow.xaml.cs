using System.Windows;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Users;

namespace SMART_CLINIC_SOFTWAR_2026.View.Users
{
    public partial class ChangeUserPasswordWindow : Window
    {
        public ChangeUserPasswordWindow()
        {
            InitializeComponent();
            this.DataContext = new ChangeUserPasswordViewModel();
        }


    }
}
