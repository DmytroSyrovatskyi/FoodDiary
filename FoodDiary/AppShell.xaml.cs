using FoodDiary.Views; 

namespace FoodDiary
{
    // Klasa głównej powłoki nawigacyjnej aplikacji (Shell)
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rejestracja tras nawigacyjnych dla poszczególnych stron aplikacji
            Routing.RegisterRoute(nameof(MyDayPage), typeof(MyDayPage)); // Strona główna - podsumowanie dnia
            Routing.RegisterRoute(nameof(FoodItemsPage), typeof(FoodItemsPage)); // Lista produktów spożywczych
            Routing.RegisterRoute(nameof(AddFoodItemPage), typeof(AddFoodItemPage)); // Dodawanie nowego produktu
            Routing.RegisterRoute(nameof(AddMealPage), typeof(AddMealPage)); // Dodawanie nowego posiłku
            Routing.RegisterRoute(nameof(MealDetailsPage), typeof(MealDetailsPage)); // Szczegóły wybranego posiłku
            Routing.RegisterRoute(nameof(AddProductToMealPage), typeof(AddProductToMealPage)); // Dodawanie produktu do posiłku
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage)); // Strona profilu użytkownika
            Routing.RegisterRoute(nameof(FoodItemReportPage), typeof(FoodItemReportPage)); // Raport produktów spożywczych
            Routing.RegisterRoute(nameof(DailySummaryReportPage), typeof(DailySummaryReportPage)); // Raport dzienny po dacie
        }
    }
}
