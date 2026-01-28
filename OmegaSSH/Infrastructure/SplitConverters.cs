using System;
using System.Globalization;
using System.Windows.Data;

namespace OmegaSSH.Infrastructure;

public class SplitToColumnsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVertical = (bool)value;
        return isVertical ? 0 : 1; 
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class SplitToRowsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVertical = (bool)value;
        return isVertical ? 1 : 0;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
