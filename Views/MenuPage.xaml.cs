using Microsoft.Maui.Controls;
using PizzeriaApp.ViewModels;
using PizzeriaApp.Converters;

namespace PizzeriaApp.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            BindingContext = new MenuViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MenuViewModel vm)
            {
                vm.RefreshCartCount();
            }
        }
    }
}