using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Group
{
    public class SelectablePhoto
    {
        public Photo PhotoSource { get; set; }
        public bool Selected { get; set; }
    }
}
