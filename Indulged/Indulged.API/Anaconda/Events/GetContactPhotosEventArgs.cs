using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetContactPhotosEventArgs : EventArgs
    {
        public string Response { get; set; }
    }
}
