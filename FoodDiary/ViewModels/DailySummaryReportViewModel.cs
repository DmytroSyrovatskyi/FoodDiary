using FoodDiary.Models;
using FoodDiary.Services;
using System;
using System.ComponentModel;
using System.Globalization; 
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodDiary.ViewModels
{
    // ViewModel do obsługi raportu dziennego podsumowania spożycia
    public class DailySummaryReportViewModel : INotifyPropertyChanged
    {
        // Serwis do pobierania dziennych podsumowań
        private readonly IDailySummaryService _summaryService;
        // Wybrana przez użytkownika data do wyszukania podsumowania
        private DateTime _selectedDate = DateTime.Today;
        // Znalezione podsumowanie dla wybranej daty
        private DailySummary _foundSummary;
        // Flaga informująca, czy wykonano już wyszukiwanie
        private bool _wasSearched;
        // Sformatowana data znalezionego podsumowania (np. "poniedziałek, 01 stycznia 2024")
        private string _formattedFoundDate; 
        // Identyfikator aktualnego użytkownika (przykładowo ustawiony na 1)
        private const int CurrentUserId = 1;

        // Właściwość do wyboru daty przez użytkownika
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); }
        }

        // Właściwość przechowująca znalezione podsumowanie
        public DailySummary FoundSummary
        {
            get => _foundSummary;
            set
            {
                _foundSummary = value;
                OnPropertyChanged();

                // Ustawienie sformatowanej daty, jeśli znaleziono podsumowanie
                if (value != null)
                {
                    FormattedFoundDate = value.Date.ToString("dddd, dd MMMM yyyy", new CultureInfo("pl-PL"));
                }
                else
                {
                    FormattedFoundDate = string.Empty;
                }
            }
        }

        // Sformatowana data znalezionego podsumowania (do wyświetlenia w UI)
        public string FormattedFoundDate { get => _formattedFoundDate; set { _formattedFoundDate = value; OnPropertyChanged(); } }

        // Flaga informująca, czy wykonano wyszukiwanie
        public bool WasSearched
        {
            get => _wasSearched;
            set { _wasSearched = value; OnPropertyChanged(); }
        }

        // Komenda do uruchomienia wyszukiwania podsumowania
        public ICommand SearchCommand { get; }

        // Konstruktor ViewModelu
        public DailySummaryReportViewModel(IDailySummaryService summaryService)
        {
            _summaryService = summaryService;
            SearchCommand = new Command(async () => await ExecuteSearch());
        }

        // Asynchroniczna metoda wykonująca wyszukiwanie podsumowania po dacie
        private async Task ExecuteSearch()
        {
            WasSearched = true;
            FoundSummary = await _summaryService.GetSummaryByDateAsync(SelectedDate, CurrentUserId);
        }

        // Implementacja interfejsu INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
