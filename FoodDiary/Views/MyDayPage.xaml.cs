using FoodDiary.Models;
using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona prezentująca widok "Mój dzień" z podsumowaniem i listą posiłków
    public partial class MyDayPage : ContentPage
    {
        // Referencja do ViewModelu obsługującego logikę widoku "Mój dzień"
        private readonly MyDayViewModel _viewModel;

        // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
        public MyDayPage(MyDayViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Metoda wywoływana przy pojawieniu się strony
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Ładowanie danych (podsumowanie i posiłki) przy otwarciu strony
            if (_viewModel != null)
            {
                await _viewModel.LoadDataAsync();
            }
        }

        // Obsługa wyboru posiłku z listy - przejście do szczegółów posiłku
        private async void OnMealSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Meal selectedMeal)
            {
                await Shell.Current.GoToAsync($"{nameof(MealDetailsPage)}?MealId={selectedMeal.MealId}");

                // Resetowanie zaznaczenia w CollectionView
                (sender as CollectionView).SelectedItem = null;
            }
        }
    }
}
