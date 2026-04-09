using System.Collections.ObjectModel;
using PizzeriaApp.Models;

namespace PizzeriaApp.Services
{
    public class CartService
    {
        private static readonly Lazy<CartService> _instance =
            new Lazy<CartService>(() => new CartService());

        public static CartService Instance => _instance.Value;

        private ObservableCollection<CartItem> _cartItems;

        public event EventHandler CartChanged;

        private CartService()
        {
            _cartItems = new ObservableCollection<CartItem>();
        }

        public ObservableCollection<CartItem> GetItems()
        {
            return _cartItems;
        }

        public void AddItem(CartItem item)
        {
            // Проверяем, есть ли уже такая же пицца с теми же параметрами
            var existingItem = _cartItems.FirstOrDefault(i =>
                i.Pizza.Id == item.Pizza.Id &&
                i.Size.Size == item.Size.Size &&
                AreIngredientsEqual(i.SelectedIngredients, item.SelectedIngredients));

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _cartItems.Add(item);
            }

            OnCartChanged();
        }

        public void RemoveItem(CartItem item)
        {
            _cartItems.Remove(item);
            OnCartChanged();
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            OnCartChanged();
        }

        public decimal GetTotalAmount()
        {
            return _cartItems.Sum(i => i.TotalPrice);
        }

        private bool AreIngredientsEqual(List<Ingredient> list1, List<Ingredient> list2)
        {
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            var ids1 = list1.Select(i => i.Id).OrderBy(id => id);
            var ids2 = list2.Select(i => i.Id).OrderBy(id => id);

            return ids1.SequenceEqual(ids2);
        }

        private void OnCartChanged()
        {
            CartChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}