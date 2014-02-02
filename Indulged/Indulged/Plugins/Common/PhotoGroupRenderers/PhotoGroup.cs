using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public class PhotoGroup : ModelBase
    {
        // Constructor
        public PhotoGroup()
            : base()
        {
            IsHeadline = false;
            Photos = new List<Photo>();
            this.ResourceId = Guid.NewGuid().ToString().Replace("-", null);
        }

        // Constructor
        public PhotoGroup(List<Photo> _photos, string _context, string _contextType) : base()
        {
            IsHeadline = false;
            Photos = _photos;
            this.ResourceId = Guid.NewGuid().ToString().Replace("-", null);
            this.context = _context;
            this.contextType = _contextType;
        }

        public bool IsHeadline { get; set; }
        public List<Photo> Photos { get; set; }

        public string context { get; set; }
        public string contextType { get; set; }

        public VirtualLayoutRules VirtualLayout { get; set; }
    }
}
