using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoActivityEventBase : ModelBase
    {
        public string EventType { get; set; }
        public User EventUser { get; set; }
    }
}
