using Microsoft.Maui.Controls;
using PizzeriaApp.ViewModels;

namespace PizzeriaApp.Views
{
    public partial class CartPage : ContentPage
    {
        public CartPage()
        {
            InitializeComponent();
            BindingContext = new CartViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CartViewModel vm)
            {
                vm.RefreshCart();
            }
        }
    }
}