using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.Plugins.Chrome;

namespace Indulged.Plugins.Chrome.Events
{
    public class ThemeChangedEventArgs : EventArgs
    {
        public Themes SelectedTheme { get; set; }
    }
}
