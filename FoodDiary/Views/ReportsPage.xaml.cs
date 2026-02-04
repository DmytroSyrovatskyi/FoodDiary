namespace FoodDiary.Views;

// Strona z wyborem raportów w aplikacji
public partial class ReportsPage : ContentPage
{
    // Konstruktor strony - inicjalizuje komponenty widoku
    public ReportsPage()
    {
        InitializeComponent();
    }

    // Obsługa kliknięcia przycisku raportu produktów spożywczych
    private async void FoodItemReportButton_Clicked(object sender, System.EventArgs e)
    {
        // Przejście do strony raportu produktów spożywczych
        await Shell.Current.GoToAsync(nameof(FoodItemReportPage));
    }

    // Obsługa kliknięcia przycisku raportu dziennego po dacie
    private async void DailySummaryReportButton_Clicked(object sender, System.EventArgs e)
    {
        // Przejście do strony raportu dziennego po dacie
        await Shell.Current.GoToAsync(nameof(DailySummaryReportPage));
    }
}
