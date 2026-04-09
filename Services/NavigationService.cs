using System.Collections.Generic;

namespace PizzeriaApp.Services
{
    public class NavigationService
    {
        public async Task NavigateTo(string page, Dictionary<string, object> parameters = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                // Формируем строку параметров для навигации
                var paramList = new List<string>();
                foreach (var param in parameters)
                {
                    if (param.Value is string)
                    {
                        paramList.Add($"{param.Key}='{param.Value}'");
                    }
                    else
                    {
                        // Для нестроковых параметров используем QueryProperty
                        paramList.Add($"{param.Key}={param.Value}");
                    }
                }

                var queryString = string.Join("&", paramList);
                await Shell.Current.GoToAsync($"{page}?{queryString}");
            }
            else
            {
                await Shell.Current.GoToAsync(page);
            }
        }

        public async Task NavigateToWithObject<T>(string page, string paramName, T paramObject)
        {
            // Сохраняем объект во временном хранилище
            NavigationParametersStore.Instance.SetParameter(paramName, paramObject);
            await Shell.Current.GoToAsync(page);
        }

        public async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    // Вспомогательный класс для хранения параметров навигации
    public class NavigationParametersStore
    {
        private static readonly Lazy<NavigationParametersStore> _instance =
            new(() => new NavigationParametersStore());

        public static NavigationParametersStore Instance => _instance.Value;

        private readonly Dictionary<string, object> _parameters = new();

        public void SetParameter(string key, object value)
        {
            _parameters[key] = value;
        }

        public T GetParameter<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                _parameters.Remove(key); // Удаляем после получения
                return typedValue;
            }
            return default;
        }

        public bool HasParameter(string key) => _parameters.ContainsKey(key);
    }
}