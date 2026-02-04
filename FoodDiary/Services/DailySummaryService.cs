using FoodDiary.Data;
using FoodDiary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Services
{
    // Serwis odpowiedzialny za operacje na dziennych podsumowaniach użytkownika
    public class DailySummaryService : IDailySummaryService
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        // Konstruktor przyjmujący fabrykę kontekstu
        public DailySummaryService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Pobiera dzienne podsumowanie dla danego użytkownika i daty, wraz z posiłkami i produktami
        public async Task<DailySummary> GetSummaryByDateAsync(DateTime date, int userId)
        {
            // Tworzenie nowego kontekstu bazy danych
            await using var context = await _contextFactory.CreateDbContextAsync();

            // Pobranie podsumowania z bazy wraz z powiązanymi posiłkami i produktami (bez śledzenia zmian)
            var summary = await context.DailySummaries
                .Include(ds => ds.Meals)
                    .ThenInclude(m => m.MealEntries)
                    .ThenInclude(me => me.FoodItem)
                .AsNoTracking()
                .FirstOrDefaultAsync(ds => ds.UserId == userId && ds.Date.Date == date.Date);

            // Jeśli podsumowanie istnieje, oblicz sumy wartości odżywczych na podstawie wpisów posiłków
            if (summary != null)
            {
                summary.TotalCalories = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Calories * (me.Quantity / 100.0));
                summary.TotalProtein = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Protein * (me.Quantity / 100.0));
                summary.TotalFat = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Fat * (me.Quantity / 100.0));
                summary.TotalCarbohydrates = summary.Meals.SelectMany(m => m.MealEntries).Sum(me => me.FoodItem.Carbohydrates * (me.Quantity / 100.0));
            }

            // Zwrócenie podsumowania (lub null, jeśli nie znaleziono)
            return summary;
        }
    }
}
