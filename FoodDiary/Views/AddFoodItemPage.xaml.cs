using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona do dodawania nowego produktu spożywczego
    public partial class AddFoodItemPage : ContentPage
    {
        // Referencja do ViewModelu obsługującego logikę dodawania produktu
        private readonly AddFoodItemViewModel _viewModel;

        // Konstruktor strony - ustawia BindingContext na przekazany ViewModel
        public AddFoodItemPage(AddFoodItemViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Metoda wywoływana przy pojawieniu się strony
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Ładowanie dostępnych kategorii produktów przy otwarciu strony
            if (_viewModel != null)
            {
                await _viewModel.LoadCategoriesAsync();
            }
        }
    }
}
