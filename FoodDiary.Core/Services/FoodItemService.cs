using FoodDiary.Core.Data;
using FoodDiary.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Core.Services
{
    // Serwis do obsługi operacji na produktach spożywczych
    public class FoodItemService : IFoodItemService
    {
        // Fabryka kontekstu bazy danych
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        // Konstruktor przyjmujący fabrykę kontekstu
        public FoodItemService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Asynchronicznie pobiera listę wszystkich produktów spożywczych z kategorią, posortowaną alfabetycznie
        public async Task<List<FoodItem>> GetFoodItemsAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.FoodItems
                .Include(fi => fi.Category) // Dołącza kategorię produktu
                .OrderBy(fi => fi.Name) // Sortuje po nazwie
                .AsNoTracking() // Brak śledzenia zmian (lepsza wydajność do odczytu)
                .ToListAsync();
        }

        // Asynchronicznie dodaje nowy produkt spożywczy do bazy
        public async Task<bool> AddFoodItemAsync(FoodItem newItem)
        {
            if (!ValidateFoodItem(newItem, out _))
            {
                return false; // Walidacja nie powiodła się
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            context.FoodItems.Add(newItem); // Dodaje nowy produkt
            await context.SaveChangesAsync(); // Zapisuje zmiany w bazie
            return true;
        }

        // Waliduje produkt spożywczy na podstawie atrybutów danych
        public bool ValidateFoodItem(FoodItem item, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var validationContext = new ValidationContext(item);
            return Validator.TryValidateObject(item, validationContext, results, true);
        }

        // Asynchronicznie usuwa produkt spożywczy, jeśli nie jest używany w żadnym posiłku
        public async Task<(bool Success, string ErrorMessage)> DeleteFoodItemAsync(int foodItemId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // Sprawdza, czy produkt jest używany w jakimkolwiek wpisie posiłku
            bool isUsed = await context.MealEntries.AnyAsync(e => e.FoodItemId == foodItemId);
            if (isUsed)
            {
                // Nie można usunąć produktu, jeśli jest używany
                return (false, "Nie można usunąć produktu, ponieważ jest używany w zapisanych posiłkach.");
            }

            var itemInDb = await context.FoodItems.FindAsync(foodItemId);
            if (itemInDb != null)
            {
                context.FoodItems.Remove(itemInDb); // Usuwa produkt z bazy
                await context.SaveChangesAsync(); // Zapisuje zmiany
                return (true, null); 
            }

            // Produkt nie został znaleziony w bazie
            return (false, "Nie znaleziono produktu do usunięcia.");
        }
    }
}
