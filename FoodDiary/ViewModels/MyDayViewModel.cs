using FoodDiary.Data;
using FoodDiary.Models;
using FoodDiary.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization; 
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi widoku "Mój dzień" - prezentuje podsumowanie dnia i listę posiłków
    public class MyDayViewModel : INotifyPropertyChanged
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        // Podsumowanie dzienne dla bieżącego dnia
        private DailySummary _todaySummary;
        // Kolekcja posiłków w bieżącym dniu
        private ObservableCollection<Meal> _meals;
        // Flaga informująca, czy trwa ładowanie danych
        private bool _isLoading;
        // Sformatowana data do wyświetlenia w UI
        private string _formattedDate; 
        // Identyfikator aktualnego użytkownika (przykładowo ustawiony na 1)
        private const int CurrentUserId = 1;

        // Właściwość z podsumowaniem dziennym
        public DailySummary TodaySummary { get => _todaySummary; set { _todaySummary = value; OnPropertyChanged(); } }
        // Właściwość z listą posiłków
        public ObservableCollection<Meal> Meals { get => _meals; set { _meals = value; OnPropertyChanged(); } }
        // Właściwość informująca o stanie ładowania
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }
        // Właściwość z sformatowaną datą
        public string FormattedDate { get => _formattedDate; set { _formattedDate = value; OnPropertyChanged(); } }

        // Komenda do przejścia do widoku dodawania posiłku
        public ICommand AddMealCommand { get; }
        // Komenda do odświeżania danych
        public ICommand RefreshCommand { get; }
        // Komenda do usuwania posiłku
        public ICommand DeleteMealCommand { get; }

        // Konstruktor ViewModelu
        public MyDayViewModel(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            TodaySummary = new DailySummary();
            Meals = new ObservableCollection<Meal>();
            FormattedDate = DateTime.Today.ToString("dddd, dd MMMM yyyy", new CultureInfo("pl-PL"));

            AddMealCommand = new Command(async () => await GoToAddMealPage());
            RefreshCommand = new Command(async () => await LoadDataAsync());
            DeleteMealCommand = new Command<Meal>(async (meal) => await DeleteMealAsync(meal));
        }

        // Asynchroniczne ładowanie podsumowania dnia i posiłków z bazy
        public async Task LoadDataAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var today = DateTime.Today;
                // Pobranie podsumowania dziennego wraz z posiłkami i produktami
                var summary = await context.DailySummaries
                    .Include(ds => ds.Meals).ThenInclude(m => m.MealEntries).ThenInclude(me => me.FoodItem)
                    .AsNoTracking().FirstOrDefaultAsync(ds => ds.UserId == CurrentUserId && ds.Date == today);

                // Jeśli nie ma podsumowania na dziś, utwórz puste
                if (summary == null)
                {
                    summary = new DailySummary { UserId = CurrentUserId, Date = today };
                }

                // Oblicz sumy wartości odżywczych na podstawie wpisów posiłków
                summary.TotalCalories = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Calories * (me.Quantity / 100.0));
                summary.TotalProtein = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Protein * (me.Quantity / 100.0));
                summary.TotalFat = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Fat * (me.Quantity / 100.0));
                summary.TotalCarbohydrates = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Carbohydrates * (me.Quantity / 100.0));

                TodaySummary = summary;
                FormattedDate = TodaySummary.Date.ToString("dddd, dd MMMM yyyy", new CultureInfo("pl-PL"));
                UpdateMealsCollection();
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error loading daily summary: {ex.Message}"); }
            finally { IsLoading = false; }
        }

        // Asynchroniczne usuwanie wybranego posiłku
        private async Task DeleteMealAsync(Meal mealToDelete)
        {
            if (mealToDelete == null) return;
            // Potwierdzenie usunięcia posiłku przez użytkownika
            bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć posiłek '{mealToDelete.Type}'?", "Tak, usuń", "Anuluj");
            if (!confirm) return;

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var mealInDb = await context.Meals.FindAsync(mealToDelete.MealId);
                if (mealInDb != null)
                {
                    context.Meals.Remove(mealInDb);
                    await context.SaveChangesAsync();
                }
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete meal: {ex.Message}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się usunąć posiłku.", "OK");
            }
        }

        // Aktualizuje kolekcję posiłków na podstawie podsumowania dnia
        private void UpdateMealsCollection()
        {
            Meals.Clear();
            if (TodaySummary?.Meals != null)
            {
                foreach (var meal in TodaySummary.Meals.OrderBy(m => m.MealTime))
                {
                    Meals.Add(meal);
                }
            }
        }

        // Przejście do widoku dodawania nowego posiłku
        private async Task GoToAddMealPage() { await Shell.Current.GoToAsync(nameof(AddMealPage)); }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
