using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoActivityCommentEvent : PhotoActivityEventBase
    {
        public string Message { get; set; }
    }
}
