using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FoodDiary.Data;
using FoodDiary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace FoodDiary.ViewModels
{
    // ViewModel do dodawania produktu do wybranego posiłku
    [QueryProperty(nameof(MealId), "MealId")]
    public class AddProductToMealViewModel : INotifyPropertyChanged
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        // Identyfikator posiłku, do którego dodawany jest produkt
        private int _mealId;
        // Lista dostępnych produktów spożywczych
        private ObservableCollection<FoodItem> _availableFoodItems;
        // Aktualnie wybrany produkt spożywczy
        private FoodItem _selectedFoodItem;
        // Ilość produktu do dodania (domyślnie 100)
        private double _quantity = 100;
        // Flaga informująca, czy trwa operacja zapisu
        private bool _isSaving;

        // Właściwość do przekazywania identyfikatora posiłku (ustawia i ładuje produkty)
        public int MealId { get => _mealId; set { if (_mealId != value) { _mealId = value; LoadAvailableFoodItemsAsync(); } } }
        // Lista dostępnych produktów spożywczych do wyboru
        public ObservableCollection<FoodItem> AvailableFoodItems { get => _availableFoodItems; set { _availableFoodItems = value; OnPropertyChanged(); } }
        // Wybrany produkt spożywczy
        public FoodItem SelectedFoodItem { get => _selectedFoodItem; set { _selectedFoodItem = value; OnPropertyChanged(); } }
        // Ilość produktu do dodania
        public double Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }
        // Flaga informująca, czy trwa zapis
        public bool IsSaving { get => _isSaving; private set { _isSaving = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); } }
        // Komenda do zapisania wpisu posiłku
        public ICommand SaveCommand { get; }

        // Konstruktor ViewModelu
        public AddProductToMealViewModel(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            SaveCommand = new Command(async () => await SaveMealEntryAsync(), () => !IsSaving);
            AvailableFoodItems = new ObservableCollection<FoodItem>();
        }

        // Asynchroniczne ładowanie dostępnych produktów spożywczych z bazy
        private async Task LoadAvailableFoodItemsAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var items = await context.FoodItems.OrderBy(f => f.Name).AsNoTracking().ToListAsync();
                AvailableFoodItems = new ObservableCollection<FoodItem>(items);
                SelectedFoodItem = AvailableFoodItems.FirstOrDefault();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Failed to load food items: {ex.Message}"); }
        }

        // Asynchroniczne zapisanie nowego wpisu posiłku (produkt + ilość)
        private async Task SaveMealEntryAsync()
        {
            // Walidacja wyboru produktu i ilości
            if (SelectedFoodItem == null || Quantity <= 0) { await Shell.Current.DisplayAlert("Błąd walidacji", "Proszę wybrać produkt i wprowadzić ilość większą niż zero.", "OK"); return; }
            IsSaving = true;
            bool isSuccess = false;
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var mealEntry = new MealEntry { MealId = this.MealId, FoodItemId = this.SelectedFoodItem.FoodItemId, Quantity = this.Quantity };
                context.MealEntries.Add(mealEntry);
                await context.SaveChangesAsync();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save meal entry: {ex.Message}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się zapisać produktu.", "OK");
            }
            finally
            {
                IsSaving = false;
            }

            // Powrót do poprzedniego widoku po udanym zapisie
            if (isSuccess)
            {
                await Shell.Current.GoToAsync($"../..");
            }
        }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
