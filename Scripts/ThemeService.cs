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

        /// <summary>
        /// Applies a theme to the application by loading and merging a ResourceDictionary.
        /// Themes are expected to be XAML files located in the "TeamThemes" folder.
        /// </summary>
        /// <param name="themeName">The name of the theme to apply (e.g., "RedSox", "Default"). 
        /// This should correspond to a XAML file name in the TeamThemes folder.</param>
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
