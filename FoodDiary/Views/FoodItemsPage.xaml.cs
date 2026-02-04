using FoodDiary.ViewModels; 

namespace FoodDiary.Views
{
    // Strona prezentująca listę produktów spożywczych
    public partial class FoodItemsPage : ContentPage
    {
        // Referencja do ViewModelu obsługującego logikę listy produktów
        private readonly FoodItemsViewModel _viewModel;

        // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
        public FoodItemsPage(FoodItemsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Metoda wywoływana przy pojawieniu się strony
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Ładowanie produktów spożywczych przy otwarciu strony
            if (_viewModel != null)
            {
                await _viewModel.LoadFoodItemsAsync();
            }
        }
    }
}
