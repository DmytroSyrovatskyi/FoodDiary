using FoodDiary.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FoodDiary.Core.Services
{
    public interface IFoodItemService
    {
        // Metoda asynchroniczna do pobierania listy produktów spożywczych
        Task<List<FoodItem>> GetFoodItemsAsync();

        // Metoda asynchroniczna do dodawania nowego produktu spożywczego
        Task<bool> AddFoodItemAsync(FoodItem newItem);

        // Metoda do walidacji produktu spożywczego, zwracająca ewentualne błędy
        bool ValidateFoodItem(FoodItem item, out ICollection<ValidationResult> results);

        // Metoda asynchroniczna do usuwania produktu spożywczego po jego ID
        Task<(bool Success, string ErrorMessage)> DeleteFoodItemAsync(int foodItemId);
    }
}
