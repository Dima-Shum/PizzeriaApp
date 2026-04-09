using System.Collections.Generic;
using System.Linq;

namespace PizzeriaApp.Models
{
    public class CartItem
    {
        public Pizza Pizza { get; set; }
        public PizzaSize Size { get; set; }
        public List<Ingredient> SelectedIngredients { get; set; } = new();

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set => _quantity = value > 0 ? value : 1;
        }

        // Вычисляемое свойство — цена за единицу
        public decimal UnitPrice
        {
            get
            {
                var baseWithSize = Pizza.BasePrice * Size.Multiplier;
                var ingredientsSum = SelectedIngredients?.Sum(i => i.Price) ?? 0;
                return baseWithSize + ingredientsSum;
            }
        }

        // Итоговая стоимость с учётом количества
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}