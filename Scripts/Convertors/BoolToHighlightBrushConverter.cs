using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Calendar.Scripts
{
    public class BoolToHighlightBrushConverter : IValueConverter
    {

        /// <summary>
        /// Converts a boolean value to a brush.
        /// </summary>
        /// <param name="value">The boolean value to convert. Expected to be a bool.</param>
        /// <param name="targetType">Expected to be a Brush.</param>
        /// <param name="parameter">not used</param>
        /// <param name="culture">not used</param>
        /// <returns>A SolidColorBrush</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isSelected && isSelected
                ? new SolidColorBrush(Colors.LightBlue)
                : new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Converts a brush back to a boolean value. This method is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
