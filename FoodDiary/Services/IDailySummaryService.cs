using FoodDiary.Models;
using System;
using System.Threading.Tasks;

namespace FoodDiary.Services
{
    // Interfejs serwisu do obsługi dziennych podsumowań użytkownika
    public interface IDailySummaryService
    {
        // Asynchronicznie pobiera dzienne podsumowanie dla danego użytkownika i daty
        Task<DailySummary> GetSummaryByDateAsync(DateTime date, int userId);
    }
}
