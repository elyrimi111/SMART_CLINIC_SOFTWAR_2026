using BLL.Mangers.Clinc;
using Core.Entites.Clinc;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using SMART_CLINIC_SOFTWAR_2026.View.LoginView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Core.CurrentSession;



namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Clinc
{
    public class ClincListWindowViewModel : BaseViewModel
    {
        private readonly clsClincManager _clincManager;

        public ClincListWindowViewModel()
        {
            _clincManager = new clsClincManager();
            ClinicsList = new ObservableCollection<clsClinc>();

            LoadClinicsCommand = new RelayCommand(param => _ = LoadClinics());
            SelectClinicCommand = new RelayCommand(SelectClinic);

            _ = LoadClinics();
        }

        #region Properties  

        private ObservableCollection<clsClinc> _clinicsList = new();
        public ObservableCollection<clsClinc> ClinicsList
        {
            get => _clinicsList;
            set
            {
                _clinicsList = value;
                OnPropertyChanged();
            }
        }

        private clsClinc? _selectedClinic;
        public clsClinc? SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                if (_selectedClinic != value)
                {
                    _selectedClinic = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _totalClinicsCountText = "إجمالي العيادات المسجلة: 0";
        public string TotalClinicsCountText
        {
            get => _totalClinicsCountText;
            set
            {
                _totalClinicsCountText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands  

        public ICommand LoadClinicsCommand { get; }
        public ICommand SelectClinicCommand { get; }

        #endregion

        #region Methods  

        public async Task LoadClinics()
        {
            try
            {
                var list = await _clincManager.GetClinicsPagedAsync(1, int.MaxValue, null)
                           ?? new List<clsClinc>();

                ClinicsList.Clear();
                foreach (var item in list)
                {
                    ClinicsList.Add(item);
                }

                TotalClinicsCountText = $"إجمالي العيادات المسجلة: {ClinicsList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ أثناء تحميل بيانات العيادات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectClinic(object? param)
        {
            if (param is clsClinc clinic)
            {
                SelectedClinic = clinic;
            }

            if (SelectedClinic == null)
            {
                MessageBox.Show("يرجى تحديد عيادة أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            clsCurrentSectioncs.CurrentClinc = SelectedClinic;

            Window? currentWindow = param as Window
                                 ?? Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            LogWind logWindow = new LogWind();
            Application.Current.MainWindow = logWindow;
            logWindow.Show();

            currentWindow?.Close();
        }

        #endregion
    }
}
