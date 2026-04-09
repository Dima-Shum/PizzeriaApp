using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using PizzeriaApp.Models;

namespace PizzeriaApp.Services
{
    public class DataService
    {
        private static readonly Lazy<DataService> _instance =
            new(() => new DataService());

        public static DataService Instance => _instance.Value;

        private List<Pizza> _pizzasCache;
        private List<Ingredient> _ingredientsCache;

        private DataService()
        {
        }

        public async Task<List<Pizza>> GetPizzasAsync()
        {
            if (_pizzasCache != null)
                return _pizzasCache;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("pizzas.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                _pizzasCache = JsonSerializer.Deserialize<List<Pizza>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return _pizzasCache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading pizzas: {ex.Message}");
                return GetDefaultPizzas();
            }
        }

        public async Task<List<Ingredient>> GetIngredientsAsync()
        {
            if (_ingredientsCache != null)
                return _ingredientsCache;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("ingredients.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                _ingredientsCache = JsonSerializer.Deserialize<List<Ingredient>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return _ingredientsCache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading ingredients: {ex.Message}");
                return GetDefaultIngredients();
            }
        }

        // Синхронные методы для совместимости
        public List<Pizza> GetPizzas()
        {
            if (_pizzasCache != null)
                return _pizzasCache;

            try
            {
                var assembly = typeof(DataService).GetTypeInfo().Assembly;
                using var stream = assembly.GetManifestResourceStream("PizzeriaApp.Resources.Raw.pizzas.json");

                if (stream == null)
                    throw new FileNotFoundException("pizzas.json not found in resources");

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                _pizzasCache = JsonSerializer.Deserialize<List<Pizza>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return _pizzasCache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading pizzas: {ex.Message}");
                return GetDefaultPizzas();
            }
        }

        public List<Ingredient> GetIngredients()
        {
            if (_ingredientsCache != null)
                return _ingredientsCache;

            try
            {
                var assembly = typeof(DataService).GetTypeInfo().Assembly;
                using var stream = assembly.GetManifestResourceStream("PizzeriaApp.Resources.Raw.ingredients.json");

                if (stream == null)
                    throw new FileNotFoundException("ingredients.json not found in resources");

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();

                _ingredientsCache = JsonSerializer.Deserialize<List<Ingredient>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return _ingredientsCache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading ingredients: {ex.Message}");
                return GetDefaultIngredients();
            }
        }

        // ИЗМЕНЕНО: private -> public
        public List<Pizza> GetDefaultPizzas()
        {
            return new List<Pizza>
            {
                new Pizza
                {
                    Id = 1,
                    Name = "Маргарита",
                    Description = "Томатный соус, моцарелла, базилик, оливковое масло",
                    BasePrice = 350,
                    ImageUrl = "margherita.png"
                },
                new Pizza
                {
                    Id = 2,
                    Name = "Пепперони",
                    Description = "Томатный соус, моцарелла, пепперони, орегано",
                    BasePrice = 420,
                    ImageUrl = "pepperoni.png"
                },
                new Pizza
                {
                    Id = 3,
                    Name = "Четыре сыра",
                    Description = "Моцарелла, горгонзола, пармезан, фонталь",
                    BasePrice = 480,
                    ImageUrl = "four_cheese.png"
                },
                new Pizza
                {
                    Id = 4,
                    Name = "Гавайская",
                    Description = "Томатный соус, моцарелла, курица, ананас",
                    BasePrice = 450,
                    ImageUrl = "hawaiian.png"
                }
            };
        }

        // ИЗМЕНЕНО: private -> public
        public List<Ingredient> GetDefaultIngredients()
        {
            return new List<Ingredient>
            {
                new Ingredient { Id = 1, Name = "Сыр моцарелла", Price = 50 },
                new Ingredient { Id = 2, Name = "Бекон", Price = 80 },
                new Ingredient { Id = 3, Name = "Грибы", Price = 45 },
                new Ingredient { Id = 4, Name = "Оливки", Price = 40 },
                new Ingredient { Id = 5, Name = "Ветчина", Price = 70 },
                new Ingredient { Id = 6, Name = "Томаты черри", Price = 35 },
                new Ingredient { Id = 7, Name = "Перец халапеньо", Price = 30 },
                new Ingredient { Id = 8, Name = "Пармезан", Price = 60 }
            };
        }
    }
}