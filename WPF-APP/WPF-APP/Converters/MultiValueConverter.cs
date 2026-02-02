using System.Globalization;
using System.Windows.Data;

namespace WPF_APP.Converters;

/// <summary>
/// Demonstrates IMultiValueConverter for combining multiple bindings
/// </summary>
public class FullNameConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is string firstName && values[1] is string lastName)
        {
            return $"{firstName} {lastName}";
        }
        return string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
