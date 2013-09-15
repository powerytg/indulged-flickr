using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetPhotoSetPhotosExceptionEventArgs : EventArgs
    {
        public string PhotoSetId { get; set; }
    }
}
