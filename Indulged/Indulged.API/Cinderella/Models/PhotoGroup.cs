using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoGroup : ModelBase
    {
        // Constructor
        public PhotoGroup()
            : base()
        {
            Photos = new List<Photo>();
            this.ResourceId = Guid.NewGuid().ToString().Replace("-", null);
        }

        // Constructor
        public PhotoGroup(List<Photo> _photos) : base()
        {
            Photos = _photos;
            this.ResourceId = Guid.NewGuid().ToString().Replace("-", null);
        }

        public bool IsHeadline { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
