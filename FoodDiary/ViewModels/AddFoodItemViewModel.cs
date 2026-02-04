using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FoodDiary.Models;
using FoodDiary.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using FoodDiary.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi dodawania nowego produktu spożywczego
    public class AddFoodItemViewModel : INotifyPropertyChanged
    {
        // Serwis do operacji na produktach spożywczych
        private readonly IFoodItemService _foodItemService;
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        // Pola prywatne przechowujące wartości właściwości
        private string _name;
        private double _calories;
        private double _protein;
        private double _fat;
        private double _carbohydrates;
        private string _servingUnit;
        private string _validationErrors;
        private bool _isSaving;

        // Lista dostępnych kategorii produktów (do wyboru w UI)
        private ObservableCollection<FoodCategory> _availableCategories;
        public ObservableCollection<FoodCategory> AvailableCategories
        {
            get => _availableCategories;
            set { _availableCategories = value; OnPropertyChanged(); }
        }

        // Wybrana kategoria produktu
        private FoodCategory _selectedCategory;
        public FoodCategory SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        // Lista dostępnych jednostek porcji
        public List<string> AvailableUnits { get; } = new List<string> { "g", "ml", "szt." };

        // Właściwości powiązane z polami formularza
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); ClearValidationError(); } }
        public double Calories { get => _calories; set { _calories = value; OnPropertyChanged(); ClearValidationError(); } }
        public double Protein { get => _protein; set { _protein = value; OnPropertyChanged(); ClearValidationError(); } }
        public double Fat { get => _fat; set { _fat = value; OnPropertyChanged(); ClearValidationError(); } }
        public double Carbohydrates { get => _carbohydrates; set { _carbohydrates = value; OnPropertyChanged(); ClearValidationError(); } }
        public string ServingUnit { get => _servingUnit; set { _servingUnit = value; OnPropertyChanged(); ClearValidationError(); } }

        // Komunikaty o błędach walidacji
        public string ValidationErrors { get => _validationErrors; private set { _validationErrors = value; OnPropertyChanged(); } }

        // Flaga informująca, czy trwa zapisywanie produktu
        public bool IsSaving { get => _isSaving; private set { _isSaving = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); } }

        // Komenda do zapisu produktu
        public ICommand SaveCommand { get; }

        // Konstruktor ViewModelu
        public AddFoodItemViewModel(IFoodItemService foodItemService, IDbContextFactory<AppDbContext> contextFactory)
        {
            _foodItemService = foodItemService;
            _contextFactory = contextFactory;
            SaveCommand = new Command(async () => await SaveFoodItemAsync(), () => !IsSaving);
            ServingUnit = AvailableUnits.FirstOrDefault();
            AvailableCategories = new ObservableCollection<FoodCategory>();
        }

        // Asynchroniczne ładowanie kategorii produktów z bazy
        public async Task LoadCategoriesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var categories = await context.FoodCategories.OrderBy(c => c.Name).AsNoTracking().ToListAsync();
            AvailableCategories = new ObservableCollection<FoodCategory>(categories);
        }

        // Asynchroniczne zapisywanie nowego produktu spożywczego
        private async Task SaveFoodItemAsync()
        {
            IsSaving = true;
            ValidationErrors = null;

            // Tworzenie nowego obiektu produktu na podstawie danych z formularza
            var newFoodItem = new FoodItem
            {
                Name = this.Name,
                Calories = this.Calories,
                Protein = this.Protein,
                Fat = this.Fat,
                Carbohydrates = this.Carbohydrates,
                ServingUnit = this.ServingUnit,
                FoodCategoryId = this.SelectedCategory?.FoodCategoryId
            };

            // Walidacja produktu przed zapisem
            if (!_foodItemService.ValidateFoodItem(newFoodItem, out var validationResults))
            {
                var errors = new StringBuilder();
                foreach (var result in validationResults) { errors.AppendLine(result.ErrorMessage); }
                ValidationErrors = errors.ToString().Trim();
                IsSaving = false;
                return;
            }

            // Próba dodania produktu do bazy
            bool success = await _foodItemService.AddFoodItemAsync(newFoodItem);
            IsSaving = false;

            if (success)
            {
                // Powrót do poprzedniej strony po udanym zapisie
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                // Wyświetlenie komunikatu o błędzie
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się zapisać produktu.", "OK");
            }
        }

        // Czyści komunikaty o błędach walidacji po zmianie wartości w formularzu
        private void ClearValidationError() { if (!string.IsNullOrWhiteSpace(ValidationErrors)) { ValidationErrors = null; } }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
