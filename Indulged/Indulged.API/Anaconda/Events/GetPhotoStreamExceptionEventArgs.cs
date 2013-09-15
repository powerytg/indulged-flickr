using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetPhotoStreamExceptionEventArgs : EventArgs
    {
        public string UserId { get; set; }
    }
}
