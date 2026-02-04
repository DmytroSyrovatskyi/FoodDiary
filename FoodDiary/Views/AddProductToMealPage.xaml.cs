using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona do dodawania produktu do wybranego posiłku
    public partial class AddProductToMealPage : ContentPage
    {
        // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
        public AddProductToMealPage(AddProductToMealViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
