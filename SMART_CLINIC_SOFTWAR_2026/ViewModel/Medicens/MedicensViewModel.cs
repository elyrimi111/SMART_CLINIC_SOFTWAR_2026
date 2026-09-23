using BLL.Mangers.Medicens;
using Core.Entites.Medcin;
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

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Medicens
{
    public class MedicensViewModel : INotifyPropertyChanged
    {
        private readonly clsMedicenManeger _medicensManager;

        public MedicensViewModel()
        {
            _medicensManager = new clsMedicenManeger();
            MedicinesList = new ObservableCollection<clsMedcin>();

            _pageSize = 17;
            _currentPage = 1;

            LoadMedicinesCommand = new RelayCommand(async param => await GetMedicinesAsync());
            PageChangedCommand = new RelayCommand(async param => await GetMedicinesAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddMedicineCommand = new RelayCommand(async param => await AddMedicineAsync(), param => CanSaveMedicine());
            UpdateMedicineCommand = new RelayCommand(async param => await UpdateMedicineAsync(), param => CanUpdate());
            DeleteMedicineCommand = new RelayCommand(async param => await DeleteMedicineAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            // تهيئة البيانات الأولية بشكل غير متزامن
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            MED_CODE = await GetNew_MED_CODEAsync();
            await GetMedicinesAsync();
        }

        #region Properties (الخصائص)

        private long _med_ID;
        public long MED_ID
        {
            get => _med_ID;
            set
            {
                if (_med_ID != value)
                {
                    _med_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _med_CODE;
        public long? MED_CODE
        {
            get => _med_CODE;
            set
            {
                if (_med_CODE != value)
                {
                    _med_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _med_NAME;
        public string? MED_NAME
        {
            get => _med_NAME;
            set
            {
                if (_med_NAME != value)
                {
                    _med_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _med_S_NAME;
        public string? MED_S_NAME
        {
            get => _med_S_NAME;
            set
            {
                if (_med_S_NAME != value)
                {
                    _med_S_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _med_SOURES;
        public string? MED_SOURES
        {
            get => _med_SOURES;
            set
            {
                if (_med_SOURES != value)
                {
                    _med_SOURES = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _med_PRICE;
        public decimal MED_PRICE
        {
            get => _med_PRICE;
            set
            {
                if (_med_PRICE != value)
                {
                    _med_PRICE = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _cli_ID;
        public long? CLI_ID
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

        private ObservableCollection<clsMedcin> _medicinesList;
        public ObservableCollection<clsMedcin> MedicinesList
        {
            get => _medicinesList;
            set
            {
                _medicinesList = value;
                OnPropertyChanged();
            }
        }

        private clsMedcin? _selectedMedicine;
        public clsMedcin? SelectedMedicine
        {
            get => _selectedMedicine;
            set
            {
                if (_selectedMedicine != value)
                {
                    _selectedMedicine = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedMedicine();
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
                    _ = GetMedicinesAsync();
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
                    _ = GetMedicinesAsync();
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

        public ICommand LoadMedicinesCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddMedicineCommand { get; }
        public ICommand UpdateMedicineCommand { get; }
        public ICommand DeleteMedicineCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(MED_NAME) ||
                   !MED_CODE.HasValue ||
                   MED_PRICE < 0;
        }

        private async Task<long> GetNew_MED_CODEAsync()
        {
            try
            {
                return await _medicensManager.GetNewMed_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود الدواء", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreMedicinesEqual(clsMedcin med1, clsMedcin med2)
        {
            if (med1 == null || med2 == null) return med1 == med2;

            string json1 = JsonSerializer.Serialize(med1);
            string json2 = JsonSerializer.Serialize(med2);

            return json1 == json2;
        }

        private async Task GetMedicinesAsync()
        {
            try
            {
                TotalRows = await _medicensManager.GetTotalMedicinesCountAsync(SearchQuery);

                var pagedMedicines = await _medicensManager.GetMedicinesPagedAsync(CurrentPage, PageSize, SearchQuery)
                                    ?? new List<clsMedcin>();

                MedicinesList = new ObservableCollection<clsMedcin>(pagedMedicines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات الأدوية", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveMedicine()
        {
            return !IsEmptyInputs() && SelectedMedicine == null;
        }

        private bool CanDelete()
        {
            return SelectedMedicine != null && SelectedMedicine.MED_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedMedicine != null && SelectedMedicine.MED_ID > 0;
        }

        private async Task AddMedicineAsync()
        {
            try
            {
                var newMedicine = BuildMedicineFromProperties();
                long insertedId = await _medicensManager.AddMedicineAsync(newMedicine);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة الدواء بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetMedicinesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateMedicineAsync()
        {
            if (AreMedicinesEqual(SelectedMedicine, BuildMedicineFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var medicineToUpdate = BuildMedicineFromProperties();
                bool isSuccess = await _medicensManager.UpdateMedicineAsync(medicineToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات الدواء بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetMedicinesAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteMedicineAsync()
        {
            if (MED_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذا الدواء؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _medicensManager.DeleteMedicineAsync(MED_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف الدواء بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetMedicinesAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedMedicine()
        {
            if (SelectedMedicine == null) return;

            MED_ID = SelectedMedicine.MED_ID;
            MED_CODE = SelectedMedicine.MED_CODE;
            MED_NAME = SelectedMedicine.MED_NAME;
            MED_S_NAME = SelectedMedicine.MED_S_NAME;
            MED_SOURES = SelectedMedicine.MED_SOURES;
            MED_PRICE = SelectedMedicine.MED_PRICE;
            CLI_ID = SelectedMedicine.CLI_ID;
        }

        private clsMedcin BuildMedicineFromProperties()
        {
            return new clsMedcin
            {
                MED_ID = this.MED_ID,
                MED_CODE = this.MED_CODE ?? 0,
                MED_NAME = this.MED_NAME ?? string.Empty,
                MED_S_NAME = this.MED_S_NAME ?? string.Empty,
                MED_SOURES = this.MED_SOURES ?? string.Empty,
                MED_PRICE = this.MED_PRICE,
                CLI_ID = this.CLI_ID ?? 4
            };
        }

        private async Task ClearFieldsAsync()
        {
            MED_ID = 0;
            MED_CODE = await GetNew_MED_CODEAsync();
            MED_NAME = string.Empty;
            MED_S_NAME = string.Empty;
            MED_SOURES = string.Empty;
            MED_PRICE = 0;
            CLI_ID = null;
            SelectedMedicine = null;
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
