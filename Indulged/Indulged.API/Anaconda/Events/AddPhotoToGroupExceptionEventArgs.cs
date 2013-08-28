using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class AddPhotoToGroupExceptionEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string GroupId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
