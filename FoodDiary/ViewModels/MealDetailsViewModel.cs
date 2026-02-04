using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FoodDiary.Models;
using FoodDiary.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using FoodDiary.Views;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi szczegółów posiłku (wyświetlanie i zarządzanie produktami w posiłku)
    [QueryProperty(nameof(MealId), "MealId")]
    public class MealDetailsViewModel : INotifyPropertyChanged
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        // Aktualnie wybrany posiłek
        private Meal _currentMeal;
        // Identyfikator posiłku
        private int _mealId;
        // Flaga informująca, czy trwa operacja (np. ładowanie danych)
        private bool _isBusy;

        // Kolekcja wpisów posiłku (produkty wchodzące w skład posiłku)
        public ObservableCollection<MealEntry> Entries { get; }
        // Właściwość przechowująca aktualny posiłek
        public Meal CurrentMeal { get => _currentMeal; set { _currentMeal = value; OnPropertyChanged(); } }
        // Flaga informująca o stanie operacji (np. ładowanie)
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }
        // Identyfikator posiłku, ustawiany przez mechanizm nawigacji
        public int MealId { get => _mealId; set { _mealId = value; MainThread.BeginInvokeOnMainThread(async () => await LoadMealDetailsAsync()); } }
        // Komenda do przejścia do widoku dodawania produktu do posiłku
        public ICommand AddProductToMealCommand { get; }
        // Komenda do usuwania produktu z posiłku
        public ICommand DeleteEntryCommand { get; }

        // Konstruktor ViewModelu
        public MealDetailsViewModel(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            Entries = new ObservableCollection<MealEntry>();
            AddProductToMealCommand = new Command(async () => await GoToAddProductToMealPage());
            DeleteEntryCommand = new Command<MealEntry>(async (entry) => await DeleteEntryAsync(entry));
        }

        // Asynchroniczne ładowanie szczegółów posiłku oraz jego produktów
        public async Task LoadMealDetailsAsync()
        {
            if (IsBusy || MealId == 0) return;
            IsBusy = true;
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var meal = await context.Meals
                    .Include(m => m.MealEntries).ThenInclude(me => me.FoodItem)
                    .AsNoTracking().FirstOrDefaultAsync(m => m.MealId == this.MealId);

                CurrentMeal = meal;
                Entries.Clear();
                if (meal?.MealEntries != null)
                {
                    foreach (var entry in meal.MealEntries.OrderBy(e => e.FoodItem.Name)) { Entries.Add(entry); }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Failed to load meal details: {ex.Message}"); }
            finally { IsBusy = false; }
        }

        // Asynchroniczne usuwanie wybranego produktu z posiłku
        private async Task DeleteEntryAsync(MealEntry entryToDelete)
        {
            if (entryToDelete == null) return;
            // Potwierdzenie usunięcia produktu przez użytkownika
            bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć {entryToDelete.FoodItem?.Name ?? "produkt"}?", "Tak, usuń", "Anuluj");
            if (!confirm) return;
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var entryInDb = await context.MealEntries.FindAsync(entryToDelete.MealEntryId);
                if (entryInDb != null)
                {
                    context.MealEntries.Remove(entryInDb);
                    await context.SaveChangesAsync();
                }
                await LoadMealDetailsAsync();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Failed to delete meal entry: {ex.Message}"); }
        }

        // Przejście do widoku dodawania produktu do posiłku
        private async Task GoToAddProductToMealPage() { if (MealId > 0) await Shell.Current.GoToAsync($"{nameof(AddProductToMealPage)}?MealId={MealId}"); }
        
        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
