using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PizzeriaApp.Models;
using PizzeriaApp.Services;

namespace PizzeriaApp.ViewModels
{
    public class ConstructorViewModel : INotifyPropertyChanged
    {
        private readonly CartService _cartService;
        private readonly DataService _dataService;

        private Pizza _pizza;
        public Pizza Pizza
        {
            get => _pizza;
            set
            {
                _pizza = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PizzaSize> Sizes { get; set; }
        public ObservableCollection<Ingredient> Ingredients { get; set; }

        private PizzaSize _selectedSize;
        public PizzaSize SelectedSize
        {
            get => _selectedSize;
            set
            {
                _selectedSize = value;
                OnPropertyChanged();
                CalculatePrice();
            }
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            private set
            {
                _unitPrice = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalPrice => UnitPrice * Quantity;

        public ICommand AddToCartCommand { get; }

        public ConstructorViewModel()
        {
            _cartService = CartService.Instance;
            _dataService = DataService.Instance;

            LoadSizes();

            AddToCartCommand = new Command(OnAddToCart);
        }

        public void Initialize(Pizza pizza)
        {
            Pizza = pizza;
            SelectedSize = Sizes.FirstOrDefault();

            LoadIngredients();
            CalculatePrice();
        }

        private void LoadSizes()
        {
            Sizes = new ObservableCollection<PizzaSize>(PizzaSize.GetAll());
        }

        private async void LoadIngredients()
        {
            try
            {
                var ingredients = await _dataService.GetIngredientsAsync();
                Ingredients = new ObservableCollection<Ingredient>(ingredients);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading ingredients: {ex.Message}");
                // Загружаем данные по умолчанию, если JSON не найден
                Ingredients = new ObservableCollection<Ingredient>(_dataService.GetDefaultIngredients());
            }

            // Подписываемся на изменения ингредиентов
            foreach (var ingredient in Ingredients)
            {
                ingredient.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Ingredient.IsSelected))
                    {
                        CalculatePrice();
                    }
                };
            }

            CalculatePrice();
        }

        private void CalculatePrice()
        {
            if (Pizza == null || SelectedSize == null) return;

            var baseWithSize = Pizza.BasePrice * SelectedSize.Multiplier;
            var ingredientsSum = Ingredients?.Where(i => i.IsSelected).Sum(i => i.Price) ?? 0;
            UnitPrice = baseWithSize + ingredientsSum;

            OnPropertyChanged(nameof(TotalPrice));
        }

        private async void OnAddToCart()
        {
            if (Pizza == null || SelectedSize == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    "Пожалуйста, выберите размер пиццы",
                    "OK");
                return;
            }

            var cartItem = new CartItem
            {
                Pizza = Pizza,
                Size = SelectedSize,
                SelectedIngredients = Ingredients.Where(i => i.IsSelected).ToList(),
                Quantity = Quantity
            };

            _cartService.AddItem(cartItem);

            await Application.Current.MainPage.DisplayAlert(
                "Успешно",
                $"{Pizza.Name} добавлена в корзину!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}