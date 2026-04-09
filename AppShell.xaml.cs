namespace PizzeriaApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрация маршрутов для навигации
        Routing.RegisterRoute("MenuPage", typeof(Views.MenuPage));
        Routing.RegisterRoute("ConstructorPage", typeof(Views.ConstructorPage));
        Routing.RegisterRoute("CartPage", typeof(Views.CartPage));
        Routing.RegisterRoute("CheckoutPage", typeof(Views.CheckoutPage));

        // Устанавливаем главную страницу
        CurrentItem = MenuPageItem;
    }
}