using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda
{
    public class RemoveFavouriteExceptionEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string Message { get; set; }
    }
}
