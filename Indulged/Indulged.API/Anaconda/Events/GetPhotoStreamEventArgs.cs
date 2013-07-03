using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetPhotoStreamEventArgs : EventArgs
    {
        public string UserId { get; set; }
        public string Response { get; set; }
    }
}
