using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar.Scripts
{
    public class ThemeService
    {
        public void ApplyTheme(string themeName)
        {
            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri($"pack://application:,,,/Calendar;component/TeamThemes/{themeName}.xaml")
                };

                var app = Application.Current;
                var existing = app.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString.Contains("TeamThemes/") == true);

                if (existing != null) app.Resources.MergedDictionaries.Remove(existing);
                app.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load team theme: {ex.Message}");
            }
        }
    }
}
