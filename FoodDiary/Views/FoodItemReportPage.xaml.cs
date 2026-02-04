using FoodDiary.ViewModels;

namespace FoodDiary.Views;

// Strona prezentująca raport i wyszukiwanie produktów spożywczych
public partial class FoodItemReportPage : ContentPage
{
    // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
    public FoodItemReportPage(FoodItemReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
