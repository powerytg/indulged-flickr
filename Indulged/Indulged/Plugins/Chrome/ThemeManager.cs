using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.Plugins.Chrome.Events;

namespace Indulged.Plugins.Chrome
{
    public enum Themes
    {
        Dark,
        Light
    };

    public class ThemeManager
    {
        // Events
        public static EventHandler<ThemeChangedEventArgs> ThemeChanged;

        // Current theme
        public static Themes CurrentTheme { get; set; }
    }
}
