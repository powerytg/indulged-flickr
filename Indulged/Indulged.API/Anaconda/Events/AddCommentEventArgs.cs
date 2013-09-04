using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class AddCommentEventArgs : EventArgs
    {
        public string SessionId { get; set; }
        public string PhotoId { get; set; }
        public string Response { get; set; }
        public string Message { get; set; }
    }
}
