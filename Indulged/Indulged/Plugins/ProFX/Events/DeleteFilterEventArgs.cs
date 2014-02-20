using Indulged.Plugins.ProFX.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.ProFX.Events
{
    public class DeleteFilterEventArgs : EventArgs
    {
        public FilterBase Filter { get; set; }
    }
}
