using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PizzeriaApp.Models;
using PizzeriaApp.Services;

namespace PizzeriaApp.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged
    {
        private readonly CartService _cartService;
        private readonly NavigationService _navigationService;

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        private string _customerPhone;
        public string CustomerPhone
        {
            get => _customerPhone;
            set
            {
                _customerPhone = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        private string _deliveryAddress;
        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set
            {
                _deliveryAddress = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        private string _nameError;
        public string NameError
        {
            get => _nameError;
            set
            {
                _nameError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowNameError));
            }
        }

        private string _phoneError;
        public string PhoneError
        {
            get => _phoneError;
            set
            {
                _phoneError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowPhoneError));
            }
        }

        private string _addressError;
        public string AddressError
        {
            get => _addressError;
            set
            {
                _addressError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowAddressError));
            }
        }

        public bool ShowNameError => !string.IsNullOrEmpty(NameError);
        public bool ShowPhoneError => !string.IsNullOrEmpty(PhoneError);
        public bool ShowAddressError => !string.IsNullOrEmpty(AddressError);

        private bool _isFormValid;
        public bool IsFormValid
        {
            get => _isFormValid;
            set
            {
                _isFormValid = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CartItem> OrderItems { get; set; }

        public decimal TotalAmount => _cartService.GetTotalAmount();

        public ICommand SubmitOrderCommand { get; }
        public ICommand GoBackCommand { get; }

        public CheckoutViewModel()
        {
            _cartService = CartService.Instance;
            _navigationService = new NavigationService();

            SubmitOrderCommand = new Command(OnSubmitOrder);
            GoBackCommand = new Command(OnGoBack);

            LoadOrderItems();
        }

        private void LoadOrderItems()
        {
            OrderItems = _cartService.GetItems();
        }

        private void ValidateForm()
        {
            // Валидация имени
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                NameError = "Имя обязательно для заполнения";
            }
            else if (CustomerName.Length < 2)
            {
                NameError = "Имя должно содержать минимум 2 символа";
            }
            else
            {
                NameError = null;
            }

            // Валидация телефона
            if (string.IsNullOrWhiteSpace(CustomerPhone))
            {
                PhoneError = "Телефон обязателен для заполнения";
            }
            else if (!IsValidPhone(CustomerPhone))
            {
                PhoneError = "Введите корректный номер телефона";
            }
            else
            {
                PhoneError = null;
            }

            // Валидация адреса
            if (string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                AddressError = "Адрес обязателен для заполнения";
            }
            else if (DeliveryAddress.Length < 10)
            {
                AddressError = "Введите полный адрес доставки";
            }
            else
            {
                AddressError = null;
            }

            IsFormValid = string.IsNullOrEmpty(NameError) &&
                         string.IsNullOrEmpty(PhoneError) &&
                         string.IsNullOrEmpty(AddressError) &&
                         OrderItems != null && OrderItems.Any();
        }

        private bool IsValidPhone(string phone)
        {
            // Простая валидация - минимум 10 цифр
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length >= 10;
        }

        private async void OnSubmitOrder()
        {
            if (!IsFormValid) return;

            var order = new Order
            {
                Id = GenerateOrderNumber(),
                Items = OrderItems.ToList(),
                TotalAmount = TotalAmount,
                OrderDate = DateTime.Now,
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                DeliveryAddress = DeliveryAddress
            };

            // Здесь можно сохранить заказ в БД или отправить на сервер

            // Показываем информацию о заказе
            await Application.Current.MainPage.DisplayAlert(
                "Заказ оформлен!",
                $"Номер заказа: {order.Id}\n" +
                $"Сумма: {order.TotalAmount:N0} ₽\n" +
                $"Ожидаемое время доставки: 45-60 минут\n\n" +
                $"Спасибо за заказ, {order.CustomerName}!",
                "OK");

            // Очищаем корзину
            _cartService.ClearCart();

            // Возвращаемся в меню
            await _navigationService.NavigateTo("MenuPage");
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        private async void OnGoBack()
        {
            await _navigationService.GoBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}