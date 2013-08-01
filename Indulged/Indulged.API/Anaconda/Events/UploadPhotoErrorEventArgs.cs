using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class UploadPhotoErrorEventArgs : EventArgs
    {
        public string SessionId { get; set; }
    }
}
