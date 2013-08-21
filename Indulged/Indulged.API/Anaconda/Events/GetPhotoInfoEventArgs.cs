using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetPhotoInfoEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string OwnerId { get; set; }
        public string Response { get; set; }
        public bool IsUploadedPhoto { get; set; }
    }
}
