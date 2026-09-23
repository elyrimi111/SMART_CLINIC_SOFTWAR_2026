using System.Windows;
using System.Windows.Input;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Users;

namespace SMART_CLINIC_SOFTWAR_2026.View.Users
{
    public partial class UsersListWindow : Window
    {
        public clsUser? SelectedUser { get; private set; }

        public UsersListWindow()
        {
            InitializeComponent();
            DataContext = new UsersListViewModel();

            DgUsers.MouseDoubleClick += DgUsers_MouseDoubleClick;
        }

        private void DgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is UsersListViewModel vm && vm.SelectedUser != null)
            {
                SelectedUser = vm.SelectedUser;

                this.DialogResult = true;
            }
        }
    }
}
