using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class UserInfoUpdatedEventArgs : EventArgs
    {
        public string UserId { get; set; }
    }
}
