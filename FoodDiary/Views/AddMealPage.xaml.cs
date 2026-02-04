using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona do dodawania nowego posiłku
    public partial class AddMealPage : ContentPage
    {
        // Konstruktor strony - ustawia BindingContext na przekazany ViewModel
        public AddMealPage(AddMealViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
