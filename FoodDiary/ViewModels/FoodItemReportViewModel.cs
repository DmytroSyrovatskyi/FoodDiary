using FoodDiary.Models;
using FoodDiary.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi raportu i wyszukiwania produktów spożywczych
    public class FoodItemReportViewModel : INotifyPropertyChanged
    {
        // Serwis do operacji na produktach spożywczych
        private readonly IFoodItemService _foodItemService;
        // Aktualny tekst wyszukiwania wpisany przez użytkownika
        private string _searchTerm;

        // Kolekcja wyników wyszukiwania produktów (do wyświetlenia w UI)
        public ObservableCollection<FoodItem> SearchResults { get; } = new ObservableCollection<FoodItem>();

        // Właściwość powiązana z polem wyszukiwania w UI
        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(); }
        }

        // Komenda do uruchomienia wyszukiwania produktów
        public ICommand SearchCommand { get; }

        // Konstruktor ViewModelu
        public FoodItemReportViewModel(IFoodItemService foodItemService)
        {
            _foodItemService = foodItemService;
            SearchCommand = new Command(async () => await ExecuteSearch());
            _ = ExecuteSearch(); // Automatyczne pobranie listy przy inicjalizacji
        }

        // Asynchroniczna metoda wykonująca wyszukiwanie produktów po nazwie
        private async Task ExecuteSearch()
        {
            var items = await _foodItemService.SearchFoodItemsByNameAsync(SearchTerm);
            SearchResults.Clear();
            foreach (var item in items)
            {
                SearchResults.Add(item);
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
