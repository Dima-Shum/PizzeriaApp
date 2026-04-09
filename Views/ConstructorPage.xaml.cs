using Microsoft.Maui.Controls;
using PizzeriaApp.ViewModels;

namespace PizzeriaApp.Views
{
    public partial class ConstructorPage : ContentPage
    {
        public ConstructorPage()
        {
            InitializeComponent();

            var viewModel = new ConstructorViewModel();
            BindingContext = viewModel;

            // Получаем параметр после создания ViewModel
            LoadPizzaParameter(viewModel);
        }

        private void LoadPizzaParameter(ConstructorViewModel viewModel)
        {
            var pizza = Services.NavigationParametersStore.Instance.GetParameter<Models.Pizza>("SelectedPizza");
            if (pizza != null)
            {
                viewModel.Initialize(pizza);
                Title = pizza.Name;
            }
        }
    }
}