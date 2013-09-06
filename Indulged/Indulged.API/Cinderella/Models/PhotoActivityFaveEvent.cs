using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoActivityFaveEvent : PhotoActivityEventBase
    {
        public PhotoActivityFaveEvent()
            : base()
        {
            FavUsers = new List<User>();
        }

        public List<User> FavUsers {get;set;}
    }
}
