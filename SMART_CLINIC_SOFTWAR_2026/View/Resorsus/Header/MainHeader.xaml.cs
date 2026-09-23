using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.View.Resorsus.Header
{
    public partial class MainHeader : UserControl
    {
        // إعلان خاصية Title كـ DependencyProperty لتتمكن من استخدامها في XAML
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MainHeader), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // 1. الباني الافتراضي (مطلوب لـ XAML)
        public MainHeader()
        {
            InitializeComponent();
        }

        // 2. الباني المخصص لتمرير العنوان برمجياً عند إنشاء الهيدر من C#
        public MainHeader(string title) : this()
        {
            Title = title;
        }

        // حدث سحب النافذة عند الضغط بالماوس على الهيدر
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window parentWindow = Window.GetWindow(this);
                parentWindow?.DragMove();
            }
        }

        // حدث زر التصغير
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
        }

        // حدث زر التكبير / الاستعادة
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Maximized)
                {
                    parentWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    parentWindow.WindowState = WindowState.Maximized;
                }
            }
        }

        // حدث زر الإغلاق
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }
    }
}
