using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Calendar.Scripts
{
    public class TimeConverter : IValueConverter
    {

        /// <summary>
        /// Converts a TimeSpan to a formatted string.
        /// </summary>
        /// <param name="value">The TimeSpan value to convert.</param>
        /// <param name="targetType">Expected to be a string.</param>
        /// <param name="parameter">not used</param>
        /// <param name="culture">not used</param>
        /// <returns>A string representing the time in "hh\:mm" format.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TimeSpan time ? time.ToString(@"hh\:mm") : value;
        }

        /// <summary>
        /// Converts a string back to a TimeSpan. This method is not implemented.
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
