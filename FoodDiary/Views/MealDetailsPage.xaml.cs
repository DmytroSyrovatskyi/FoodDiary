using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona prezentująca szczegóły wybranego posiłku (lista produktów w posiłku)
    public partial class MealDetailsPage : ContentPage
    {
        // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
        public MealDetailsPage(MealDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
