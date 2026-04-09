using Microsoft.Maui.Controls;
using PizzeriaApp.ViewModels;

namespace PizzeriaApp.Views
{
    public partial class CheckoutPage : ContentPage
    {
        public CheckoutPage()
        {
            InitializeComponent();
            BindingContext = new CheckoutViewModel();
        }
    }
}