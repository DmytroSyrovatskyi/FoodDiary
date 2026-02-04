using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FoodDiary.Models;
using FoodDiary.Services; 
using System.Windows.Input;
using FoodDiary.Views;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi listy produktów spożywczych
    public class FoodItemsViewModel : INotifyPropertyChanged
    {
        // Serwis do operacji na produktach spożywczych
        private readonly IFoodItemService _foodItemService;
        // Kolekcja produktów wyświetlanych w widoku
        private ObservableCollection<FoodItem> _foodItems;
        // Flaga informująca, czy trwa ładowanie danych
        private bool _isLoading;

        // Właściwość do powiązania z listą produktów w UI
        public ObservableCollection<FoodItem> FoodItems
        {
            get => _foodItems;
            set { _foodItems = value; OnPropertyChanged(); }
        }

        // Właściwość informująca, czy trwa ładowanie danych
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // Komenda do przejścia do widoku dodawania produktu
        public ICommand AddFoodItemCommand { get; }
        // Komenda do usuwania produktu
        public ICommand DeleteFoodItemCommand { get; }

        // Konstruktor ViewModelu
        public FoodItemsViewModel(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
            FoodItems = new ObservableCollection<FoodItem>();
            AddFoodItemCommand = new Command(async () => await GoToAddFoodItemPage());
            DeleteFoodItemCommand = new Command<FoodItem>(async (item) => await DeleteFoodItemAsync(item));
        }

        // Asynchroniczne ładowanie produktów spożywczych z bazy
        public async Task LoadFoodItemsAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                // ZMIANA: Cała logika pobierania danych jest teraz w serwisie
                var items = await _foodItemService.GetFoodItemsAsync();
                FoodItems = new ObservableCollection<FoodItem>(items);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading food items: {ex.Message}");
            }
            finally { IsLoading = false; }
        }

        // Asynchroniczne usuwanie wybranego produktu spożywczego
        private async Task DeleteFoodItemAsync(FoodItem itemToDelete)
        {
            if (itemToDelete == null) return;

            // Potwierdzenie usunięcia produktu przez użytkownika
            bool confirm = await Shell.Current.DisplayAlert(
                "Potwierdzenie",
                $"Czy na pewno chcesz usunąć '{itemToDelete.Name}'?",
                "Tak, usuń",
                "Anuluj");

            if (!confirm) return;

            // Próba usunięcia produktu przez serwis
            var (success, errorMessage) = await _foodItemService.DeleteFoodItemAsync(itemToDelete.FoodItemId);

            if (success)
            {
                await LoadFoodItemsAsync(); 
            }
            else
            {
                await Shell.Current.DisplayAlert("Błąd", errorMessage, "OK");
            }
        }

        // Przejście do widoku dodawania nowego produktu
        private async Task GoToAddFoodItemPage()
        {
            await Shell.Current.GoToAsync(nameof(AddFoodItemPage));
        }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
