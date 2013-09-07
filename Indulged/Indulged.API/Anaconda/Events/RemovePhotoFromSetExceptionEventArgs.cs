using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class RemovePhotoFromSetExceptionEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string SetId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
