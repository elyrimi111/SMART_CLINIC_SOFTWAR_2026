using System;

namespace SMART_CLINIC_SOFTWAR_2026.View.Models.DataView
{
    public class PageModel
    {
        /// <summary>
        /// النص المعروض على الزر (مثال: "1", "2", "...")
        /// </summary>
        public string DisplayText { get; set; } = string.Empty;

        /// <summary>
        /// رقم الصفحة الفعلي
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// حدد ما إذا كانت هذه هي الصفحة الحالية لتمييزها بصرياً
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// يحدد إمكانية النقر على الزر (يكون false للنقاط الثلاث)
        /// </summary>
        public bool IsClickable { get; set; }

        /// <summary>
        /// يحدد ما إذا كان الزر يمثل النقاط الثلاث "..." لإلغاء إطاره
        /// </summary>
        public bool IsEllipsis { get; set; }
    }
}
