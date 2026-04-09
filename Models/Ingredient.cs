using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PizzeriaApp.Models
{
    public class Ingredient : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}