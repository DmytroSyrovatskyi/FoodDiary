using FoodDiary.ViewModels;

namespace FoodDiary.Views
{
    // Strona do edycji profilu użytkownika
    public partial class ProfilePage : ContentPage
    {
        // Konstruktor strony - ustawia przekazany ViewModel jako BindingContext
        public ProfilePage(ProfileViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
