using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class AddFavouriteExceptionEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string Message { get; set; }
    }
}
