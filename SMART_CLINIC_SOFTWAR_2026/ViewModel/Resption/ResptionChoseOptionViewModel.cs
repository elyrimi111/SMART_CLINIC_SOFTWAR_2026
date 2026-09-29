using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Resption
{
    public class ResptionChoseOptionViewModel : BaseViewModel
    {
        #region Fields & Properties

        private int _totalVisits;
        private int _confirmedAppointments;
        private int _waitingListCount;
        private int _completedExaminations;

        /// <summary>
        /// إجمالي الزيارات اليومية
        /// </summary>
        public int TotalVisits
        {
            get => _totalVisits;
            set { _totalVisits = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// المواعيد المؤكدة
        /// </summary>
        public int ConfirmedAppointments
        {
            get => _confirmedAppointments;
            set { _confirmedAppointments = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// عدد المرضى في قائمة الانتظار
        /// </summary>
        public int WaitingListCount
        {
            get => _waitingListCount;
            set { _waitingListCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// الكشوفات المكتملة
        /// </summary>
        public int CompletedExaminations
        {
            get => _completedExaminations;
            set { _completedExaminations = value; OnPropertyChanged(); }
        }

        #endregion

        #region Actions (التنبيهات الموجهة لـ MainLayoutViewModel)

        /// <summary>
        /// تنبيه لفتح شاشة استقبال المرضى
        /// </summary>
        public Action? OnNavigateToPatientReception { get; set; }

        /// <summary>
        /// تنبيه لفتح شاشة حجز المواعيد
        /// </summary>
        public Action? OnNavigateToAppointmentBooking { get; set; }

        /// <summary>
        /// تنبيه لفتح شاشة الفحص الأولي والكشف
        /// </summary>
        public Action? OnNavigateToExaminationEntry { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// أمر فتح شاشة الاستقبال وتنسيق المرضى
        /// </summary>
        public ICommand OpenPatientReceptionCommand { get; }

        /// <summary>
        /// أمر فتح شاشة حجز المواعيد والجدولة
        /// </summary>
        public ICommand OpenAppointmentBookingCommand { get; }

        /// <summary>
        /// أمر فتح شاشة إدخال الفحص الأولي والتسجيل
        /// </summary>
        public ICommand OpenExaminationEntryCommand { get; }

        #endregion

        #region Constructor

        public ResptionChoseOptionViewModel()
        {
            // تهيئة الأوامر (Commands)
            OpenPatientReceptionCommand = new RelayCommand(ExecuteOpenPatientReception);
            OpenAppointmentBookingCommand = new RelayCommand(ExecuteOpenAppointmentBooking);
            OpenExaminationEntryCommand = new RelayCommand(ExecuteOpenExaminationEntry);

            // تحميل مؤشرات وإحصائيات اليوم (ERP Metrics)
            LoadTodayMetrics();
        }

        #endregion

        #region Private Execution Methods

        private void ExecuteOpenPatientReception(object? parameter)
        {
            // استدعاء التنبيه المرتبط بـ MainLayoutViewModel لفتح تبويبة جديدة
            OnNavigateToPatientReception?.Invoke();
        }

        private void ExecuteOpenAppointmentBooking(object? parameter)
        {
            OnNavigateToAppointmentBooking?.Invoke();
        }

        private void ExecuteOpenExaminationEntry(object? parameter)
        {
            OnNavigateToExaminationEntry?.Invoke();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// جلب وقراءة بيانات المؤشرات الحية لليوم من داتابيز أو خدمة البيانات
        /// </summary>
        private void LoadTodayMetrics()
        {
            // قيم افتراضية للاختبار — قم بربطها بقاعدة البيانات لاحقاً
            TotalVisits = 0;
            ConfirmedAppointments = 0;
            WaitingListCount = 0;
            CompletedExaminations = 0;
        }

        #endregion
    }
}
