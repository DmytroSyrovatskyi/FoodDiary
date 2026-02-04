using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FoodDiary.Data;
using FoodDiary.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi profilu użytkownika (edycja i walidacja danych użytkownika)
    public class ProfileViewModel : INotifyPropertyChanged
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        // Aktualny użytkownik
        private User _currentUser;
        // Nazwa użytkownika (do edycji w UI)
        private string _username;
        // Identyfikator aktualnego użytkownika (przykładowo ustawiony na 1)
        private const int CurrentUserId = 1; 
        // Komunikaty o błędach walidacji
        private string _validationErrors;

        // Właściwość powiązana z polem edycji nazwy użytkownika
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        // Właściwość przechowująca komunikaty o błędach walidacji
        public string ValidationErrors
        {
            get => _validationErrors;
            private set { _validationErrors = value; OnPropertyChanged(); }
        }

        // Komenda do zapisu zmian profilu
        public ICommand SaveProfileCommand { get; }

        // Konstruktor ViewModelu
        public ProfileViewModel(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            SaveProfileCommand = new Command(async () => await SaveProfileAsync());
            LoadCurrentUserAsync();
        }

        // Asynchroniczne ładowanie danych aktualnego użytkownika z bazy
        private async void LoadCurrentUserAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            _currentUser = await context.Users.FindAsync(CurrentUserId);
            if (_currentUser != null)
            {
                Username = _currentUser.Username;
            }
        }

        // Asynchroniczny zapis zmian profilu użytkownika
        private async Task SaveProfileAsync()
        {
            ValidationErrors = null;
            if (_currentUser == null) return;

            // Przypisanie nowej nazwy użytkownika
            _currentUser.Username = this.Username;

            // Walidacja danych użytkownika
            var validationContext = new ValidationContext(_currentUser);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(_currentUser, validationContext, results, true);

            if (!isValid)
            {
                // Jeśli walidacja nie powiodła się, wyświetl komunikaty o błędach
                var errors = new StringBuilder();
                foreach (var result in results) { errors.AppendLine(result.ErrorMessage); }
                ValidationErrors = errors.ToString().Trim();
                return; 
            }

            try
            {
                // Zapisanie zmian w bazie danych
                await using var context = await _contextFactory.CreateDbContextAsync();
                context.Users.Update(_currentUser);
                await context.SaveChangesAsync();
                await Shell.Current.DisplayAlert("Sukces", "Profil został zaktualizowany.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // Obsługa błędów podczas zapisu
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się zaktualizować profilu.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error saving profile: {ex.Message}");
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
