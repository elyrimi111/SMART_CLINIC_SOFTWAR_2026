using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SMART_CLINIC_SOFTWAR_2026.View.Resorsus.DataViews
{
    /// <summary>
    /// Interaction logic for PaginationControl.xaml
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// رقم الصفحة الحالية (قابل للربط Data Binding)
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(
                nameof(CurrentPage),
                typeof(int),
                typeof(PaginationControl),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageDataChanged));

        /// <summary>
        /// إجمالي عدد الصفحات (قابل للربط Data Binding)
        /// </summary>
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(
                nameof(TotalPages),
                typeof(int),
                typeof(PaginationControl),
                new PropertyMetadata(1, OnPageDataChanged));

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// حدث ينطلق عند تغيير الصفحة لتنبيه الصفحة الرئيسية
        /// </summary>
        public event EventHandler<int>? PageChanged;

        #endregion

        public PaginationControl()
        {
            InitializeComponent();
            UpdatePaginationUI();
        }

        private static void OnPageDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PaginationControl control)
            {
                control.UpdatePaginationUI();
            }
        }

        /// <summary>
        /// تحديث حالة الأزرار وقائمة الصفحات في الواجهة
        /// </summary>
        private void UpdatePaginationUI()
        {
            int total = Math.Max(1, TotalPages);
            int current = Math.Min(Math.Max(1, CurrentPage), total);

            // تفعيل/إلغاء تفعيل زري السابق والتالي
            if (BtnPrev != null) BtnPrev.IsEnabled = current > 1;
            if (BtnNext != null) BtnNext.IsEnabled = current < total;

            // بناء قائمة أرقام الصفحات
            var pages = GeneratePageList(current, total);
            if (PagesItemsControl != null)
            {
                PagesItemsControl.ItemsSource = pages;
            }
        }

        /// <summary>
        /// توليد قائمة أرقام الصفحات والنقاط الثلاث بناءً على الصفحة الحالية والإجمالي
        /// </summary>
        private List<PageModel> GeneratePageList(int current, int total)
        {
            var list = new List<PageModel>();

            // إذا كان عدد الصفحات قليل، نعرض جميع الصفحات
            if (total <= 7)
            {
                for (int i = 1; i <= total; i++)
                {
                    list.Add(CreatePageItem(i, current));
                }
            }
            else
            {
                // عرض الصفحة الأولى دائماً
                list.Add(CreatePageItem(1, current));

                if (current <= 4)
                {
                    for (int i = 2; i <= 5; i++)
                        list.Add(CreatePageItem(i, current));

                    list.Add(CreateEllipsisItem());
                    list.Add(CreatePageItem(total, current));
                }
                else if (current >= total - 3)
                {
                    list.Add(CreateEllipsisItem());
                    for (int i = total - 4; i <= total; i++)
                        list.Add(CreatePageItem(i, current));
                }
                else
                {
                    list.Add(CreateEllipsisItem());
                    list.Add(CreatePageItem(current - 1, current));
                    list.Add(CreatePageItem(current, current));
                    list.Add(CreatePageItem(current + 1, current));
                    list.Add(CreateEllipsisItem());
                    list.Add(CreatePageItem(total, current));
                }
            }

            return list;
        }

        private PageModel CreatePageItem(int page, int current)
        {
            return new PageModel
            {
                DisplayText = page.ToString(),
                PageNumber = page,
                IsCurrent = (page == current),
                IsClickable = true,
                IsEllipsis = false
            };
        }

        private PageModel CreateEllipsisItem()
        {
            return new PageModel
            {
                DisplayText = "...",
                PageNumber = 0,
                IsCurrent = false,
                IsClickable = false,
                IsEllipsis = true
            };
        }

        #region Event Handlers

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                PageChanged?.Invoke(this, CurrentPage);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                PageChanged?.Invoke(this, CurrentPage);
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is PageModel item)
            {
                if (item.IsClickable && item.PageNumber != CurrentPage)
                {
                    CurrentPage = item.PageNumber;
                    PageChanged?.Invoke(this, CurrentPage);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// نموذج البيانات الخاص بكل زر صفحة
    /// </summary>
    public class PageModel
    {
        public string DisplayText { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsClickable { get; set; }
        public bool IsEllipsis { get; set; }
    }
}
