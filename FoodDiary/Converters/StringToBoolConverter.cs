using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FoodDiary.Converters 
{
    // Konwerter sprawdzający, czy przekazany string nie jest pusty ani nie zawiera tylko białych znaków
    public class StringToBoolConverter : IValueConverter
    {
        // Metoda konwertująca: zwraca true, jeśli string nie jest pusty i nie składa się tylko z białych znaków
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        // Metoda konwertująca z powrotem nie jest zaimplementowana (rzuca wyjątek)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
