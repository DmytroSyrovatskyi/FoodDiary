using Microsoft.Extensions.Logging;
using FoodDiary.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using FoodDiary.ViewModels;
using FoodDiary.Views;
using FoodDiary.Services; 

namespace FoodDiary
{
    // Klasa konfiguracyjna aplikacji MAUI
    public static class MauiProgram
    {
        // Metoda tworząca i konfigurująca aplikację MAUI
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Konfiguracja czcionek używanych w aplikacji
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            // Dodanie logowania debugowania w trybie DEBUG
            builder.Logging.AddDebug();
#endif

            // Ścieżka do pliku bazy danych SQLite w katalogu aplikacji
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FoodDiaryApp.db3");
            // Rejestracja fabryki kontekstu bazy danych z użyciem SQLite
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));

            // Rejestracja serwisów i widoków w kontenerze DI
            builder.Services.AddScoped<IFoodItemService, FoodItemService>();
            builder.Services.AddTransient<MyDayViewModel>();
            builder.Services.AddTransient<MyDayPage>();
            builder.Services.AddTransient<FoodItemsViewModel>();
            builder.Services.AddTransient<FoodItemsPage>();
            builder.Services.AddTransient<AddFoodItemViewModel>();
            builder.Services.AddTransient<AddFoodItemPage>();
            builder.Services.AddTransient<AddMealViewModel>();
            builder.Services.AddTransient<AddMealPage>();
            builder.Services.AddTransient<MealDetailsViewModel>();
            builder.Services.AddTransient<MealDetailsPage>();
            builder.Services.AddTransient<AddProductToMealViewModel>();
            builder.Services.AddTransient<AddProductToMealPage>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ReportsPage>();
            builder.Services.AddTransient<FoodItemReportViewModel>();
            builder.Services.AddTransient<FoodItemReportPage>();
            builder.Services.AddScoped<IDailySummaryService, DailySummaryService>();
            builder.Services.AddTransient<DailySummaryReportViewModel>();
            builder.Services.AddTransient<DailySummaryReportPage>();

            // Budowanie aplikacji
            var app = builder.Build();

            // Inicjalizacja bazy danych przy pierwszym uruchomieniu aplikacji
            using (var scope = app.Services.CreateScope())
            {
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using (var db = dbContextFactory.CreateDbContext())
                {
                    db.Database.EnsureCreated();
                }
            }

            // Zwrócenie skonfigurowanej aplikacji
            return app;
        }
    }
}
