using BLL.Mangers.Clinc;
using Core.Entites.Clinc;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Clinc
{
    public class ClincViewModel : INotifyPropertyChanged
    {
        private readonly clsClincManager _clincManager;

        public ClincViewModel()
        {
            _clincManager = new clsClincManager();
            ClinicsList = new ObservableCollection<clsClinc>();

            _pageSize = 17;
            _currentPage = 1;

            LoadClinicsCommand = new RelayCommand(async param => await GetClinicsAsync());
            PageChangedCommand = new RelayCommand(async param => await GetClinicsAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddClinicCommand = new RelayCommand(async param => await AddClinicAsync(), param => CanSaveClinic());
            UpdateClinicCommand = new RelayCommand(async param => await UpdateClinicAsync(), param => CanUpdate());
            DeleteClinicCommand = new RelayCommand(async param => await DeleteClinicAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            CLI_CODE = await GetNew_CLI_CODEAsync();
            await GetClinicsAsync();
        }

        #region Properties (الخصائص)

        private long _cli_ID;
        public long CLI_ID
        {
            get => _cli_ID;
            set
            {
                if (_cli_ID != value)
                {
                    _cli_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _cli_CODE;
        public long? CLI_CODE
        {
            get => _cli_CODE;
            set
            {
                if (_cli_CODE != value)
                {
                    _cli_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cli_NAME;
        public string? CLI_NAME
        {
            get => _cli_NAME;
            set
            {
                if (_cli_NAME != value)
                {
                    _cli_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cli_LOC;
        public string? CLI_LOC
        {
            get => _cli_LOC;
            set
            {
                if (_cli_LOC != value)
                {
                    _cli_LOC = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _cli_NOTE;
        public string? CLI_NOTE
        {
            get => _cli_NOTE;
            set
            {
                if (_cli_NOTE != value)
                {
                    _cli_NOTE = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<clsClinc> _clinicsList;
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
                    FillFieldsFromSelectedClinic();
                }
            }
        }

        private string? _searchQuery;
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = GetClinicsAsync();
                }
            }
        }

        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize != value && value > 0)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    CurrentPage = 1;
                    _ = GetClinicsAsync();
                }
            }
        }

        private int _totalRows;
        public int TotalRows
        {
            get => _totalRows;
            set
            {
                _totalRows = value;
                OnPropertyChanged();
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands (الأوامر)

        public ICommand LoadClinicsCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddClinicCommand { get; }
        public ICommand UpdateClinicCommand { get; }
        public ICommand DeleteClinicCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(CLI_NAME) ||
                   !CLI_CODE.HasValue;
        }

        private async Task<long> GetNew_CLI_CODEAsync()
        {
            try
            {
                return await _clincManager.GetNewCli_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود العيادة", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreClinicsEqual(clsClinc clinic1, clsClinc clinic2)
        {
            if (clinic1 == null || clinic2 == null) return clinic1 == clinic2;

            string json1 = JsonSerializer.Serialize(clinic1);
            string json2 = JsonSerializer.Serialize(clinic2);

            return json1 == json2;
        }

        private async Task GetClinicsAsync()
        {
            try
            {
                TotalRows = await _clincManager.GetTotalClinicsCountAsync(SearchQuery);

                var pagedClinics = await _clincManager.GetClinicsPagedAsync(CurrentPage, PageSize, SearchQuery)
                                  ?? new List<clsClinc>();

                ClinicsList = new ObservableCollection<clsClinc>(pagedClinics);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات العيادات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveClinic()
        {
            return !IsEmptyInputs() && SelectedClinic == null;
        }

        private bool CanDelete()
        {
            return SelectedClinic != null && SelectedClinic.CLI_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedClinic != null && SelectedClinic.CLI_ID > 0;
        }

        private async Task AddClinicAsync()
        {
            try
            {
                var newClinic = BuildClinicFromProperties();
                long insertedId = await _clincManager.AddClinicAsync(newClinic);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة العيادة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetClinicsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateClinicAsync()
        {
            if (AreClinicsEqual(SelectedClinic, BuildClinicFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var clinicToUpdate = BuildClinicFromProperties();
                bool isSuccess = await _clincManager.UpdateClinicAsync(clinicToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات العيادة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetClinicsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteClinicAsync()
        {
            if (CLI_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذه العيادة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _clincManager.DeleteClinicAsync(CLI_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف العيادة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetClinicsAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedClinic()
        {
            if (SelectedClinic == null) return;

            CLI_ID = SelectedClinic.CLI_ID;
            CLI_CODE = SelectedClinic.CLI_CODE;
            CLI_NAME = SelectedClinic.CLI_NAME;
            CLI_LOC = SelectedClinic.CLI_LOC;
            CLI_NOTE = SelectedClinic.CLI_NOTE.ToString();
        }

        private clsClinc BuildClinicFromProperties()
        {
            return new clsClinc
            {
                CLI_ID = this.CLI_ID,
                CLI_CODE = this.CLI_CODE ?? 0,
                CLI_NAME = this.CLI_NAME ?? string.Empty,
                CLI_LOC = this.CLI_LOC ?? string.Empty,
                CLI_NOTE = this.CLI_NOTE ?? string.Empty
            };
        }

        private async Task ClearFieldsAsync()
        {
            CLI_ID = 0;
            CLI_CODE = await GetNew_CLI_CODEAsync();
            CLI_NAME = string.Empty;
            CLI_LOC = string.Empty;
            CLI_NOTE = string.Empty;
            SelectedClinic = null;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
