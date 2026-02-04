using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FoodDiary.Converters
{
    // Konwerter sprawdzający, czy obiekt jest niepusty (nie null) i zwracający wartość bool
    public class ObjectToBoolConverter : IValueConverter
    {
        // Metoda konwertująca: zwraca true, jeśli wartość nie jest null, w przeciwnym razie false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        // Metoda konwertująca z powrotem nie jest zaimplementowana (rzuca wyjątek)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
