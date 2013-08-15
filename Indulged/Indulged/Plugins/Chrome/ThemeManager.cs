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
        private static Themes _currentTheme = Themes.Light;
        public static Themes CurrentTheme
        {
            get
            {
                return _currentTheme;
            }

            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;

                    var evt = new ThemeChangedEventArgs();
                    evt.SelectedTheme = _currentTheme;

                    if (ThemeChanged != null)
                    {
                        ThemeChanged(null, evt);
                    }
                }
            }
        }
    }
}
