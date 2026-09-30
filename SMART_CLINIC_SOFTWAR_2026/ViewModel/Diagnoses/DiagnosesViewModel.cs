using BLL.Mangers.Diagnoses;
using Core.Entites.Diagnos;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Diagnoses
{
    public class DiagnosesViewModel : INotifyPropertyChanged
    {
        private readonly clsDiagnosesManger _diagnosesManger;

        public DiagnosesViewModel()
        {
            _diagnosesManger = new clsDiagnosesManger();
            DiagnosesList = new ObservableCollection<clsDiagnos>();

            _pageSize = 17;
            _currentPage = 1;

            LoadDiagnosesCommand = new RelayCommand(async param => await GetDiagnosesAsync());
            PageChangedCommand = new RelayCommand(async param => await GetDiagnosesAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddDiagnosCommand = new RelayCommand(async param => await AddDiagnosAsync(), param => CanSaveDiagnos());
            UpdateDiagnosCommand = new RelayCommand(async param => await UpdateDiagnosAsync(), param => CanUpdate());
            DeleteDiagnosCommand = new RelayCommand(async param => await DeleteDiagnosAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            DIG_CODE = await GetNew_DIG_CODEAsync();
            await GetDiagnosesAsync();
        }

        #region Properties (الخصائص)

        private long _dig_ID;
        public long DIG_ID
        {
            get => _dig_ID;
            set
            {
                if (_dig_ID != value)
                {
                    _dig_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _dig_CODE;
        public long DIG_CODE
        {
            get => _dig_CODE;
            set
            {
                if (_dig_CODE != value)
                {
                    _dig_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _dig_NAME;
        public string? DIG_NAME
        {
            get => _dig_NAME;
            set
            {
                if (_dig_NAME != value)
                {
                    _dig_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _dig_TYPE;
        public string? DIG_TYPE
        {
            get => _dig_TYPE;
            set
            {
                if (_dig_TYPE != value)
                {
                    _dig_TYPE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _dig_NOTE;
        public string? DIG_NOTE
        {
            get => _dig_NOTE;
            set
            {
                if (_dig_NOTE != value)
                {
                    _dig_NOTE = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private ObservableCollection<clsDiagnos> _diagnosesList;
        public ObservableCollection<clsDiagnos> DiagnosesList
        {
            get => _diagnosesList;
            set
            {
                _diagnosesList = value;
                OnPropertyChanged();
            }
        }

        private clsDiagnos? _selectedDiagnos;
        public clsDiagnos? SelectedDiagnos
        {
            get => _selectedDiagnos;
            set
            {
                if (_selectedDiagnos != value)
                {
                    _selectedDiagnos = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedDiagnos();
                }
            }
        }

        // --- خصائص التصفح المرقّم والبحث المفتوحة للـ PaginatedGridControl ---

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
                    _ = GetDiagnosesAsync();
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
                    _ = GetDiagnosesAsync();
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

        public ICommand LoadDiagnosesCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddDiagnosCommand { get; }
        public ICommand UpdateDiagnosCommand { get; }
        public ICommand DeleteDiagnosCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(DIG_NAME) ||
                   DIG_CODE <= 0;
        }

        private async Task<long> GetNew_DIG_CODEAsync()
        {
            try
            {
                return await _diagnosesManger.GetNewDig_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود التشخيص", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreDiagnosesEqual(clsDiagnos diagnos1, clsDiagnos diagnos2)
        {
            if (diagnos1 == null || diagnos2 == null) return diagnos1 == diagnos2;

            string json1 = JsonSerializer.Serialize(diagnos1);
            string json2 = JsonSerializer.Serialize(diagnos2);

            return json1 == json2;
        }

        private async Task GetDiagnosesAsync()
        {
            try
            {
                TotalRows = await _diagnosesManger.GetTotalDiagnosCountAsync(SearchQuery);

                var pagedDiagnoses = await _diagnosesManger.GetDiagnosPagedAsync(CurrentPage, PageSize, SearchQuery)
                                     ?? new List<clsDiagnos>();

                DiagnosesList = new ObservableCollection<clsDiagnos>(pagedDiagnoses);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات التشخيصات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveDiagnos()
        {
            return !IsEmptyInputs() && SelectedDiagnos == null;
        }

        private bool CanDelete()
        {
            return SelectedDiagnos != null && SelectedDiagnos.DIG_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedDiagnos != null && SelectedDiagnos.DIG_ID > 0;
        }

        private async Task AddDiagnosAsync()
        {
            try
            {
                var newDiagnos = BuildDiagnosFromProperties();
                long insertedId = await _diagnosesManger.AddDiagnosAsync(newDiagnos);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة التشخيص بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetDiagnosesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateDiagnosAsync()
        {
            if (SelectedDiagnos != null && AreDiagnosesEqual(SelectedDiagnos, BuildDiagnosFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var diagnosToUpdate = BuildDiagnosFromProperties();
                bool isSuccess = await _diagnosesManger.UpdateDiagnosAsync(diagnosToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات التشخيص بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetDiagnosesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteDiagnosAsync()
        {
            if (DIG_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا التشخيص؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _diagnosesManger.DeleteDiagnosAsync(DIG_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف التشخيص بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetDiagnosesAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedDiagnos()
        {
            if (SelectedDiagnos == null) return;

            DIG_ID = SelectedDiagnos.DIG_ID;
            DIG_CODE = SelectedDiagnos.DIG_CODE;
            DIG_NAME = SelectedDiagnos.DIG_NAME;
            DIG_TYPE = SelectedDiagnos.DIG_TYPE;
            DIG_NOTE = SelectedDiagnos.DIG_NOTE;
            CLI_ID = SelectedDiagnos.CLI_ID;
        }

        private clsDiagnos BuildDiagnosFromProperties()
        {
            return new clsDiagnos
            {
                DIG_ID = this.DIG_ID,
                DIG_CODE = this.DIG_CODE,
                DIG_NAME = this.DIG_NAME ?? string.Empty,
                DIG_TYPE = this.DIG_TYPE ?? string.Empty,
                DIG_NOTE = this.DIG_NOTE ?? string.Empty,
                CLI_ID = this.CLI_ID
            };
        }

        private async Task ClearFieldsAsync()
        {
            DIG_ID = 0;
            DIG_CODE = await GetNew_DIG_CODEAsync();
            DIG_NAME = string.Empty;
            DIG_TYPE = string.Empty;
            DIG_NOTE = string.Empty;
            CLI_ID = 0;
            SelectedDiagnos = null;
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
