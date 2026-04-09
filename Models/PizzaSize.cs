namespace PizzeriaApp.Models
{
    public class PizzaSize
    {
        public string Size { get; set; }
        public decimal Multiplier { get; set; }

        // Предопределённые размеры для удобства
        public static PizzaSize Small => new() { Size = "Маленькая", Multiplier = 1.0m };
        public static PizzaSize Medium => new() { Size = "Средняя", Multiplier = 1.3m };
        public static PizzaSize Large => new() { Size = "Большая", Multiplier = 1.6m };

        public static List<PizzaSize> GetAll() => new() { Small, Medium, Large };
    }
}