using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Expense_Tracker.Service
{
    public class ThemeService
    {
        public static string CurrentTheme = "Light";

        public static void ToggleTheme()
        {
            string theme = CurrentTheme == "Light" ? "Dark" : "Light";
            SetTheme(theme);
        }

        public static void SetTheme(string theme)
        {
            var dict = new ResourceDictionary();

            if (theme == "Dark")
                dict.Source = new Uri("Theme/DarkTheme.xaml", UriKind.Relative);
            else
                dict.Source = new Uri("Theme/LightTheme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

            CurrentTheme = theme;
        }
    }
}
