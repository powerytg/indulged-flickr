using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Dashboard.Events
{
    public class DashboardPageEventArgs : EventArgs
    {
        public IDashboardPage SelectedPage { get; set; }
    }
}
