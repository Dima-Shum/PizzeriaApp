using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PizzeriaApp.Models;
using PizzeriaApp.Services;

namespace PizzeriaApp.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly CartService _cartService;
        private readonly NavigationService _navigationService;

        private ObservableCollection<CartItem> _cartItems;
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged();
            }
        }

        private decimal _totalAmount;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
            }
        }

        private bool _canCheckout;
        public bool CanCheckout
        {
            get => _canCheckout;
            set
            {
                _canCheckout = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteItemCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand GoToMenuCommand { get; }

        public CartViewModel()
        {
            _cartService = CartService.Instance;
            _navigationService = new NavigationService();

            DeleteItemCommand = new Command<CartItem>(OnDeleteItem);
            CheckoutCommand = new Command(OnCheckout);
            GoToMenuCommand = new Command(OnGoToMenu);

            LoadCartItems();

            // Подписываемся на изменения в корзине
            _cartService.CartChanged += OnCartChanged;
        }

        private void LoadCartItems()
        {
            CartItems = _cartService.GetItems();
            UpdateTotalAndCheckout();
        }

        public void RefreshCart()
        {
            LoadCartItems();
        }

        private void OnCartChanged(object sender, System.EventArgs e)
        {
            UpdateTotalAndCheckout();
            OnPropertyChanged(nameof(CartItems));
        }

        private void UpdateTotalAndCheckout()
        {
            TotalAmount = _cartService.GetTotalAmount();
            CanCheckout = CartItems != null && CartItems.Any();
        }

        private async void OnDeleteItem(CartItem item)
        {
            if (item == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Подтверждение",
                $"Удалить {item.Pizza.Name} из корзины?",
                "Да", "Нет");

            if (confirm)
            {
                _cartService.RemoveItem(item);
            }
        }

        private async void OnCheckout()
        {
            if (!CanCheckout)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    "Корзина пуста",
                    "OK");
                return;
            }

            await _navigationService.NavigateTo("CheckoutPage");
        }

        private async void OnGoToMenu()
        {
            await _navigationService.NavigateTo("MenuPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}