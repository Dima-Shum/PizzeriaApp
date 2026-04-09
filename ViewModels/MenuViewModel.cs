using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PizzeriaApp.Models;
using PizzeriaApp.Services;

namespace PizzeriaApp.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        private readonly CartService _cartService;
        private readonly DataService _dataService;

        private ObservableCollection<Pizza> _pizzas;
        public ObservableCollection<Pizza> Pizzas
        {
            get => _pizzas;
            set
            {
                _pizzas = value;
                OnPropertyChanged();
            }
        }

        private int _cartItemCount;
        public int CartItemCount
        {
            get => _cartItemCount;
            set
            {
                _cartItemCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCartBadgeVisible));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;
        public bool IsCartBadgeVisible => CartItemCount > 0;

        public ICommand SelectPizzaCommand { get; }
        public ICommand GoToCartCommand { get; }

        public MenuViewModel()
        {
            _cartService = CartService.Instance;
            _dataService = DataService.Instance;

            SelectPizzaCommand = new Command<Pizza>(OnSelectPizza);
            GoToCartCommand = new Command(OnGoToCart);

            // Подписываемся на изменения корзины
            _cartService.CartChanged += (s, e) => RefreshCartCount();

            // Загружаем данные
            LoadData();
        }

        private async void LoadData()
        {
            IsLoading = true;

            try
            {
                var pizzas = await _dataService.GetPizzasAsync();
                Pizzas = new ObservableCollection<Pizza>(pizzas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading pizzas: {ex.Message}");
                // Загружаем данные по умолчанию, если JSON не найден
                Pizzas = new ObservableCollection<Pizza>(_dataService.GetDefaultPizzas());
            }
            finally
            {
                IsLoading = false;
                RefreshCartCount();
            }
        }

        public void RefreshCartCount()
        {
            CartItemCount = _cartService.GetItems().Sum(i => i.Quantity);
        }

        private async void OnSelectPizza(Pizza pizza)
        {
            if (pizza == null) return;

            // Сохраняем выбранную пиццу в хранилище параметров
            NavigationParametersStore.Instance.SetParameter("SelectedPizza", pizza);

            // Используем прямую навигацию Shell
            await Shell.Current.GoToAsync("ConstructorPage");
        }

        private async void OnGoToCart()
        {
            await Shell.Current.GoToAsync("CartPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}