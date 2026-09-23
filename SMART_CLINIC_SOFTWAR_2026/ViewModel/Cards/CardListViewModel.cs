using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SMART_CLINIC_SOFTWAR_2026.View.Card; 
using System.Windows;
using System.Windows.Input;
using BLL.Mangers.Card;
using Core.Entites.Card;
using SMART_CLINIC_SOFTWAR_2026.ViewModel.Commands;

namespace SMART_CLINIC_SOFTWAR_2026.ViewModel.Cards
{
    public class CardListViewModel : INotifyPropertyChanged
    {
        private readonly clsCardManger _cardManger;

        public CardListViewModel()
        {
            _cardManger = new clsCardManger();
            CardsList = new ObservableCollection<clsCard>();

            _pageSize = 10;
            _currentPage = 1;

            LoadCardsCommand = new RelayCommand(async param => await GetCardsAsync());
            PageChangedCommand = new RelayCommand(async param => await GetCardsAsync());
            ClearSearchCommand = new RelayCommand(param => ClearSearch());
            SelectCardCommand = new RelayCommand(param => SelectCard(param), param => CanSelectCard());

            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            await GetCardsAsync();
        }

        #region Properties (الخصائص)

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
        public ICommand SelectCardCommand { get; }

        #endregion

        #region Methods (العمليات)

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
                MessageBox.Show(ex.Message, "خطأ في جلب قائمة البطائق", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
        }

        private bool CanSelectCard()
        {
            return SelectedCard != null && SelectedCard.CARD_ID > 0;
        }

        private void SelectCard(object? parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
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
