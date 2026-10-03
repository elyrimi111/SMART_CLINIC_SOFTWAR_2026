using BLL.Mangers.MedCheck;
using Core.Entites.Med_Check;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.MedCheck
{
    public class MedCheckViewModel : BaseViewModel
    {
        private readonly clsMedCheckManger _medCheckManger;

        public MedCheckViewModel()
        {
            _medCheckManger = new clsMedCheckManger();
            MedChecksList = new ObservableCollection<clsMed_Check>();

            _pageSize = 17;
            _currentPage = 1;

            LoadMedChecksCommand = new RelayCommand(async param => await GetMedChecksAsync());
            PageChangedCommand = new RelayCommand(async param => await GetMedChecksAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddMedCheckCommand = new RelayCommand(async param => await AddMedCheckAsync(), param => CanSaveMedCheck());
            UpdateMedCheckCommand = new RelayCommand(async param => await UpdateMedCheckAsync(), param => CanUpdate());
            DeleteMedCheckCommand = new RelayCommand(async param => await DeleteMedCheckAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            MEDCHECK_CODE = await GetNew_MEDCHECK_CODEAsync();
            await GetMedChecksAsync();
        }

        #region Properties

        private long _medCheck_ID;
        public long MEDCHECK_ID
        {
            get => _medCheck_ID;
            set
            {
                if (_medCheck_ID != value)
                {
                    _medCheck_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _medCheck_CODE;
        public long MEDCHECK_CODE
        {
            get => _medCheck_CODE;
            set
            {
                if (_medCheck_CODE != value)
                {
                    _medCheck_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _medCheck_NAME = string.Empty;
        public string MEDCHECK_NAME
        {
            get => _medCheck_NAME;
            set
            {
                if (_medCheck_NAME != value)
                {
                    _medCheck_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _medCheck_TYPE = string.Empty;
        public string MEDCHECK_TYPE
        {
            get => _medCheck_TYPE;
            set
            {
                if (_medCheck_TYPE != value)
                {
                    _medCheck_TYPE = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal? _medCheck_PRICE = decimal.MinValue;
        public decimal?  MEDCHECK_PRICE
        {
            get => _medCheck_PRICE;
            set
            {
                if (_medCheck_PRICE != value)
                {
                    _medCheck_PRICE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _medCheck_NOTE = string.Empty;
        public string MEDCHECK_NOTE
        {
            get => _medCheck_NOTE;
            set
            {
                if (_medCheck_NOTE != value)
                {
                    _medCheck_NOTE = value;
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

        private ObservableCollection<clsMed_Check> _medChecksList;
        public ObservableCollection<clsMed_Check> MedChecksList
        {
            get => _medChecksList;
            set
            {
                _medChecksList = value;
                OnPropertyChanged();
            }
        }

        private clsMed_Check? _selectedMedCheck;
        public clsMed_Check? SelectedMedCheck
        {
            get => _selectedMedCheck;
            set
            {
                if (_selectedMedCheck != value)
                {
                    _selectedMedCheck = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedMedCheck();
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
                    _ = GetMedChecksAsync();
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
                    _ = GetMedChecksAsync();
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

        #region Commands

        public ICommand LoadMedChecksCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddMedCheckCommand { get; }
        public ICommand UpdateMedCheckCommand { get; }
        public ICommand DeleteMedCheckCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(MEDCHECK_NAME) ||
                   string.IsNullOrWhiteSpace(MEDCHECK_TYPE) ||
                    MEDCHECK_PRICE  > 0 ||
                   MEDCHECK_CODE <= 0;
        }

        private async Task<long> GetNew_MEDCHECK_CODEAsync()
        {
            try
            {
                return await _medCheckManger.GetNewMedCheck_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الفحص الطبي", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreMedChecksEqual(clsMed_Check check1, clsMed_Check check2)
        {
            if (check1 == null || check2 == null) return check1 == check2;

            string json1 = JsonSerializer.Serialize(check1);
            string json2 = JsonSerializer.Serialize(check2);

            return json1 == json2;
        }

        private async Task GetMedChecksAsync()
        {
            try
            {
                TotalRows = await _medCheckManger.GetTotalMedChecksCountAsync(SearchQuery);

                var pagedMedChecks = await _medCheckManger.GetMedChecksPagedAsync(CurrentPage, PageSize, SearchQuery)
                                      ?? new List<clsMed_Check>();

                MedChecksList = new ObservableCollection<clsMed_Check>(pagedMedChecks);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الفحوصات الطبية", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveMedCheck()
        {
            return !IsEmptyInputs() && SelectedMedCheck == null;
        }

        private bool CanDelete()
        {
            return SelectedMedCheck != null && SelectedMedCheck.MEDCHECK_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedMedCheck != null && SelectedMedCheck.MEDCHECK_ID > 0;
        }

        private async Task AddMedCheckAsync()
        {
            try
            {
                var newMedCheck = BuildMedCheckFromProperties();
                long insertedId = await _medCheckManger.AddMedCheckAsync(newMedCheck);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الفحص الطبي بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetMedChecksAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateMedCheckAsync()
        {
            if (AreMedChecksEqual(SelectedMedCheck, BuildMedCheckFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var medCheckToUpdate = BuildMedCheckFromProperties();
                bool isSuccess = await _medCheckManger.UpdateMedCheckAsync(medCheckToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الفحص الطبي بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetMedChecksAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteMedCheckAsync()
        {
            if (MEDCHECK_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا الفحص الطبي؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _medCheckManger.DeleteMedCheckAsync(MEDCHECK_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الفحص الطبي بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetMedChecksAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedMedCheck()
        {
            if (SelectedMedCheck == null) return;

            MEDCHECK_ID = SelectedMedCheck.MEDCHECK_ID;
            MEDCHECK_CODE = SelectedMedCheck.MEDCHECK_CODE;
            MEDCHECK_NAME = SelectedMedCheck.MEDCHECK_NAME;
            MEDCHECK_TYPE = SelectedMedCheck.MEDCHECK_TYPE;
            MEDCHECK_PRICE = SelectedMedCheck.MEDCHECK_PRICE;
            MEDCHECK_NOTE = SelectedMedCheck.MEDCHECK_NOTE;
            CLI_ID = SelectedMedCheck.CLI_ID;
        }

        private clsMed_Check BuildMedCheckFromProperties()
        {
            return new clsMed_Check
            {
                MEDCHECK_ID = this.MEDCHECK_ID,
                MEDCHECK_CODE = this.MEDCHECK_CODE,
                MEDCHECK_NAME = this.MEDCHECK_NAME ?? string.Empty,
                MEDCHECK_TYPE = this.MEDCHECK_TYPE ?? string.Empty,
                MEDCHECK_PRICE = this.MEDCHECK_PRICE ?? 0,
                MEDCHECK_NOTE = this.MEDCHECK_NOTE ?? string.Empty,
                CLI_ID = this.CLI_ID
            };
        }

        private async Task ClearFieldsAsync()
        {
            MEDCHECK_ID = 0;
            MEDCHECK_CODE = await GetNew_MEDCHECK_CODEAsync();
            MEDCHECK_NAME = string.Empty;
            MEDCHECK_TYPE = string.Empty;
            MEDCHECK_PRICE = 0;
            MEDCHECK_NOTE = string.Empty;
            CLI_ID = 0;
            SelectedMedCheck = null;
        }

        #endregion
    }
}
