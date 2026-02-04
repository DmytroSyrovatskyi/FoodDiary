using FoodDiary.ViewModels;

namespace FoodDiary.Views;

// Strona prezentująca raport dziennego podsumowania spożycia
public partial class DailySummaryReportPage : ContentPage
{
    // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
    public DailySummaryReportPage(DailySummaryReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
