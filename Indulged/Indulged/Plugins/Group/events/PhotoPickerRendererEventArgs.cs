using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Group.events
{
    public class PhotoPickerRendererEventArgs : EventArgs
    {
        public bool Selected { get; set; }
        public string PhotoId { get; set; }
    }
}
