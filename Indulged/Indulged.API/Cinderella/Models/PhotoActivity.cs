using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoActivity : ModelBase
    {
        public string Title { get; set; }
        public Photo TargetPhoto { get; set; }
        public List<PhotoActivityEventBase> Events { get; set; }

        public PhotoActivity()
            : base()
        {
            Events = new List<PhotoActivityEventBase>();
        }
    }
}
