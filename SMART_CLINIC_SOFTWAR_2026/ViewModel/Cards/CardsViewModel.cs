using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BLL.Mangers.Card;
using Core.Entites.Card;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Cards
{
    public class CardsViewModel : INotifyPropertyChanged
    {
        private readonly clsCardManger _cardManger;

        public CardsViewModel()
        {
            _cardManger = new clsCardManger();
            CardsList = new ObservableCollection<clsCard>();

            _pageSize = 17;
            _currentPage = 1;

            LoadCardsCommand = new RelayCommand(async param => await GetCardsAsync());
            PageChangedCommand = new RelayCommand(async param => await GetCardsAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());

            AddCardCommand = new RelayCommand(async param => await AddCardAsync(), param => CanSaveCard());
            UpdateCardCommand = new RelayCommand(async param => await UpdateCardAsync(), param => CanUpdate());
            DeleteCardCommand = new RelayCommand(async param => await DeleteCardAsync(), param => CanDelete());
            ClearFieldsCommand = new RelayCommand(async param => await ClearFieldsAsync());

            // تهيئة البيانات الأولية بشكل غير متزامن
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            CARD_CODE = await GetNew_CARD_CODEAsync();
            await GetCardsAsync();
        }

        #region Properties (الخصائص)

        private long _card_ID;
        public long CARD_ID
        {
            get => _card_ID;
            set
            {
                if (_card_ID != value)
                {
                    _card_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _card_CODE;
        public long? CARD_CODE
        {
            get => _card_CODE;
            set
            {
                if (_card_CODE != value)
                {
                    _card_CODE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _card_NAME;
        public string? CARD_NAME
        {
            get => _card_NAME;
            set
            {
                if (_card_NAME != value)
                {
                    _card_NAME = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _card_DATE = DateTime.Now;
        public DateTime? CARD_DATE
        {
            get => _card_DATE;
            set
            {
                if (_card_DATE != value)
                {
                    _card_DATE = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _card_STATE = true;
        public bool CARD_STATE
        {
            get => _card_STATE;
            set
            {
                if (_card_STATE != value)
                {
                    _card_STATE = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _card_PER;
        public string? CARD_PER
        {
            get => _card_PER;
            set
            {
                if (_card_PER != value)
                {
                    _card_PER = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _card_NOTE;
        public string? CARD_NOTE
        {
            get => _card_NOTE;
            set
            {
                if (_card_NOTE != value)
                {
                    _card_NOTE = value;
                    OnPropertyChanged();
                }
            }
        }

        private long? _com_ID;
        public long? COM_ID
        {
            get => _com_ID;
            set
            {
                if (_com_ID != value)
                {
                    _com_ID = value;
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

        private ObservableCollection<clsCard> _cardsList;
        public ObservableCollection<clsCard> CardsList
        {
            get => _cardsList;
            set
            {
                _cardsList = value;
                OnPropertyChanged();
            }
        }

        private clsCard? _selectedCard;
        public clsCard? SelectedCard
        {
            get => _selectedCard;
            set
            {
                if (_selectedCard != value)
                {
                    _selectedCard = value;
                    OnPropertyChanged();
                    FillFieldsFromSelectedCard();
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
                    _ = GetCardsAsync();
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
                    _ = GetCardsAsync();
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

        public ICommand LoadCardsCommand { get; }
        public ICommand PageChangedCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand AddCardCommand { get; }
        public ICommand UpdateCardCommand { get; }
        public ICommand DeleteCardCommand { get; }
        public ICommand ClearFieldsCommand { get; }

        #endregion

        #region Methods (العمليات)

        private bool IsEmptyInputs()
        {
            return string.IsNullOrWhiteSpace(CARD_NAME) ||
                   !CARD_CODE.HasValue;
        }

        private async Task<long> GetNew_CARD_CODEAsync()
        {
            try
            {
                return await _cardManger.GetNewCard_CodeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في توليد كود البطاقة", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public bool AreCardsEqual(clsCard card1, clsCard card2)
        {
            if (card1 == null || card2 == null) return card1 == card2;

            string json1 = JsonSerializer.Serialize(card1);
            string json2 = JsonSerializer.Serialize(card2);

            return json1 == json2;
        }

        private async Task GetCardsAsync()
        {
            try
            {
                TotalRows = await _cardManger.GetTotalCardsCountAsync(SearchQuery);

                var pagedCards = await _cardManger.GetCardsPagedAsync(CurrentPage, PageSize, SearchQuery)
                                 ?? new List<clsCard>();

                CardsList = new ObservableCollection<clsCard>(pagedCards);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في جلب بيانات البطائق", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSaveCard()
        {
            return !IsEmptyInputs() && SelectedCard == null;
        }

        private bool CanDelete()
        {
            return SelectedCard != null && SelectedCard.CARD_ID > 0;
        }

        private bool CanUpdate()
        {
            return SelectedCard != null && SelectedCard.CARD_ID > 0;
        }

        private async Task AddCardAsync()
        {
            try
            {
                var newCard = BuildCardFromProperties();
                long insertedId = await _cardManger.AddCardAsync(newCard);

                if (insertedId > 0)
                {
                    MessageBox.Show("تمت إضافة البطاقة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCardsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الإضافة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateCardAsync()
        {
            if (AreCardsEqual(SelectedCard!, BuildCardFromProperties()))
            {
                MessageBox.Show("لايوجد فرق بين البيانات السابقة والحالية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var cardToUpdate = BuildCardFromProperties();
                bool isSuccess = await _cardManger.UpdateCardAsync(cardToUpdate);

                if (isSuccess)
                {
                    MessageBox.Show("تم تعديل بيانات البطاقة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    await GetCardsAsync();
                    await ClearFieldsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في التعديل", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteCardAsync()
        {
            if (CARD_ID <= 0) return;

            var result = MessageBox.Show("هل أنت متأكد من حذف هذه البطاقة؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool isSuccess = await _cardManger.DeleteCardAsync(CARD_ID);
                    if (isSuccess)
                    {
                        MessageBox.Show("تم حذف البطاقة بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                        await GetCardsAsync();
                        await ClearFieldsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في الحذف", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillFieldsFromSelectedCard()
        {
            if (SelectedCard == null) return;

            CARD_ID = SelectedCard.CARD_ID;
            CARD_CODE = SelectedCard.CARD_CODE;
            CARD_NAME = SelectedCard.CARD_NAME;

            if (DateTime.TryParse(SelectedCard.CARD_DATE, out DateTime dtDate))
            {
                CARD_DATE = dtDate;
            }
            else
            {
                CARD_DATE = DateTime.Now;
            }

            if (bool.TryParse(SelectedCard.CARD_STATE, out bool bState))
            {
                CARD_STATE = bState;
            }
            else
            {
                CARD_STATE = false;
            }

            CARD_PER = SelectedCard.CARD_PER;
            CARD_NOTE = SelectedCard.CARD_NOTE;
            COM_ID = SelectedCard.COM_ID;
            CLI_ID = SelectedCard.CLI_ID;
        }

        private clsCard BuildCardFromProperties()
        {
            return new clsCard
            {
                CARD_ID = this.CARD_ID,
                CARD_CODE = this.CARD_CODE ?? 0,
                CARD_NAME = this.CARD_NAME ?? string.Empty,
                CARD_DATE = this.CARD_DATE.HasValue ? this.CARD_DATE.Value.ToString("yyyy-MM-dd") : string.Empty,
                CARD_STATE = this.CARD_STATE.ToString(),
                CARD_PER = this.CARD_PER ?? string.Empty,
                CARD_NOTE = this.CARD_NOTE ?? string.Empty,
                COM_ID = this.COM_ID ?? 0,
                CLI_ID = this.CLI_ID ?? 4
            };
        }

        private async Task ClearFieldsAsync()
        {
            CARD_ID = 0;
            CARD_CODE = await GetNew_CARD_CODEAsync();
            CARD_NAME = string.Empty;
            CARD_DATE = DateTime.Now;
            CARD_STATE = true;
            CARD_PER = string.Empty;
            CARD_NOTE = string.Empty;
            COM_ID = null;
            CLI_ID = null;
            SelectedCard = null;
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
