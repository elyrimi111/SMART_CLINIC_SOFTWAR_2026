using System.Windows;
using Core.Entites.User;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Users;

namespace SMART_CLINIC_SOFTWAR_2026.View.Users
{
    /// <summary>
    /// Interaction logic for ChangeUserPasswordWindow.xaml
    /// </summary>
    public partial class ChangeUserPasswordWindow : Window
    {
        /// <summary>
        /// مشيد افتراضي
        /// </summary>
        public ChangeUserPasswordWindow()
        {
            InitializeComponent();
            this.DataContext = new ChangeUserPasswordViewModel();
        }

        /// <summary>
        /// مشيد يمرر المستخدم الحالي مباشرة إلى ViewModel
        /// </summary>
        /// <param name="user">كائن المستخدم المراد تغيير كلمة المرور له</param>

    }
}
