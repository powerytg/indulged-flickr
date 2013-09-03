using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoComment : ModelBase
    {
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
        public User Author { get; set; }
    }
}
