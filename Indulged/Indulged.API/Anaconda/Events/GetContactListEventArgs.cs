using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetContactListEventArgs : EventArgs
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Response { get; set; }
    }
}
