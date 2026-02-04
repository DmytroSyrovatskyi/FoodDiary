using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FoodDiary.Models;
using FoodDiary.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi dodawania nowego posiłku
    public class AddMealViewModel : INotifyPropertyChanged
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        // Wybrany typ posiłku
        private MealType _selectedMealType;
        // Flaga informująca, czy trwa zapisywanie posiłku
        private bool _isSaving;
        // Identyfikator aktualnego użytkownika (przykładowo ustawiony na 1)
        private const int CurrentUserId = 1; 

        // Lista dostępnych typów posiłków (np. śniadanie, obiad, kolacja)
        public List<MealType> MealTypes { get; } = Enum.GetValues(typeof(MealType)).Cast<MealType>().ToList();

        // Właściwość przechowująca wybrany typ posiłku
        public MealType SelectedMealType
        {
            get => _selectedMealType;
            set { _selectedMealType = value; OnPropertyChanged(); }
        }

        // Właściwość informująca, czy trwa operacja zapisu
        public bool IsSaving
        {
            get => _isSaving;
            private set { _isSaving = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); }
        }

        // Komenda do zapisu posiłku
        public ICommand SaveCommand { get; }

        // Konstruktor ViewModelu
        public AddMealViewModel(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            SaveCommand = new Command(async () => await SaveMealAsync(), () => !IsSaving);
            SelectedMealType = MealTypes.FirstOrDefault(); 
        }

        // Asynchroniczna metoda zapisująca nowy posiłek do bazy
        private async Task SaveMealAsync()
        {
            IsSaving = true;

            try
            {
                // Utworzenie kontekstu bazy danych
                using var context = _contextFactory.CreateDbContext();
                var today = DateTime.Today;
                // Pobranie dziennego podsumowania dla użytkownika i dzisiejszej daty
                var summary = await context.DailySummaries.FirstOrDefaultAsync(ds => ds.UserId == CurrentUserId && ds.Date == today);

                // Jeśli nie istnieje podsumowanie na dziś, utwórz nowe
                if (summary == null)
                {
                    summary = new DailySummary { UserId = CurrentUserId, Date = today };
                    context.DailySummaries.Add(summary);
                }

                // Utworzenie nowego obiektu posiłku
                var newMeal = new Meal
                {
                    DailySummary = summary,
                    Type = SelectedMealType,
                    MealTime = DateTime.Now 
                };

                // Dodanie posiłku do bazy i zapisanie zmian
                context.Meals.Add(newMeal);
                await context.SaveChangesAsync();

                // Powrót do poprzedniej strony po udanym zapisie
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // Obsługa błędów podczas zapisu posiłku
                System.Diagnostics.Debug.WriteLine($"Error saving meal: {ex}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się zapisać posiłku.", "OK");
            }
            finally
            {
                IsSaving = false;
            }
        }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
