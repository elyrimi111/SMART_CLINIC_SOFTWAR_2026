using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SMART_CLINIC_SOFTWAR_2026.View.Resorsus.DataViews
{
    [ContentProperty(nameof(Columns))]
    public partial class PaginatedGridControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(System.Collections.IEnumerable), typeof(PaginatedGridControl));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(PaginatedGridControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(PaginatedGridControl),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageDataChanged));

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(PaginatedGridControl),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageDataChanged));

        public static readonly DependencyProperty TotalRowsProperty =
            DependencyProperty.Register(nameof(TotalRows), typeof(int), typeof(PaginatedGridControl),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageDataChanged));

        public static readonly DependencyProperty NumberOfRowsProperty =
            DependencyProperty.Register(nameof(NumberOfRows), typeof(int), typeof(PaginatedGridControl),
                new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageDataChanged));

        public static readonly DependencyProperty PageChangedCommandProperty =
            DependencyProperty.Register(nameof(PageChangedCommand), typeof(ICommand), typeof(PaginatedGridControl));

        // Command الخاص بالنقر المزدوج على السطر
        public static readonly DependencyProperty RowDoubleClickCommandProperty =
            DependencyProperty.Register(nameof(RowDoubleClickCommand), typeof(ICommand), typeof(PaginatedGridControl));

        // Parameter اختياري يُمرر مع الـ Command (مثل Window لإغلاقها)
        public static readonly DependencyProperty RowDoubleClickCommandParameterProperty =
            DependencyProperty.Register(nameof(RowDoubleClickCommandParameter), typeof(object), typeof(PaginatedGridControl));

        public System.Collections.IEnumerable ItemsSource
        {
            get => (System.Collections.IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            private set => SetValue(TotalPagesProperty, value);
        }

        public int TotalRows
        {
            get => (int)GetValue(TotalRowsProperty);
            set => SetValue(TotalRowsProperty, value);
        }

        public int NumberOfRows
        {
            get => (int)GetValue(NumberOfRowsProperty);
            set => SetValue(NumberOfRowsProperty, value);
        }

        public ICommand PageChangedCommand
        {
            get => (ICommand)GetValue(PageChangedCommandProperty);
            set => SetValue(PageChangedCommandProperty, value);
        }

        public ICommand RowDoubleClickCommand
        {
            get => (ICommand)GetValue(RowDoubleClickCommandProperty);
            set => SetValue(RowDoubleClickCommandProperty, value);
        }

        public object RowDoubleClickCommandParameter
        {
            get => GetValue(RowDoubleClickCommandParameterProperty);
            set => SetValue(RowDoubleClickCommandParameterProperty, value);
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent RowDoubleClickEvent =
            EventManager.RegisterRoutedEvent(
                nameof(RowDoubleClick),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(PaginatedGridControl));

        public event MouseButtonEventHandler RowDoubleClick
        {
            add => AddHandler(RowDoubleClickEvent, value);
            remove => RemoveHandler(RowDoubleClickEvent, value);
        }

        public event EventHandler<int>? PageChanged;

        #endregion

        public ObservableCollection<DataGridColumn> Columns => MainDataGrid.Columns;

        public PaginatedGridControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                CalculateTotalPages();
                UpdatePaginationUI();
            };
        }

        private static void OnPageDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PaginatedGridControl control)
            {
                control.CalculateTotalPages();
                control.UpdatePaginationUI();
            }
        }

        private void CalculateTotalPages()
        {
            if (NumberOfRows > 0)
            {
                int pages = (int)Math.Ceiling((double)TotalRows / NumberOfRows);
                TotalPages = pages < 1 ? 1 : pages;
            }
            else
            {
                TotalPages = 1;
            }
        }

        private void UpdatePaginationUI()
        {
            if (BtnPrev != null)
                BtnPrev.IsEnabled = CurrentPage > 1;

            if (BtnNext != null)
                BtnNext.IsEnabled = CurrentPage < TotalPages;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                TriggerPageChanged();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                TriggerPageChanged();
            }
        }

        private void TriggerPageChanged()
        {
            PageChanged?.Invoke(this, CurrentPage);

            if (PageChangedCommand != null && PageChangedCommand.CanExecute(CurrentPage))
            {
                PageChangedCommand.Execute(CurrentPage);
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 1. إطلاق الـ RoutedEvent للأحداث التقليدية
            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = RowDoubleClickEvent,
                Source = this
            });

            // 2. تنفيذ الـ Command المربوط بـ ViewModel مباشرة
            object param = RowDoubleClickCommandParameter ?? SelectedItem;

            if (RowDoubleClickCommand != null && RowDoubleClickCommand.CanExecute(param))
            {
                RowDoubleClickCommand.Execute(param);
            }
        }
    }
}
