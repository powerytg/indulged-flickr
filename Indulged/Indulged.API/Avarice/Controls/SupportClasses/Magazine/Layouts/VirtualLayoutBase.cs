using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public abstract class VirtualLayoutBase
    {
        public int TileCount { get; set; }
        public Rect PageRect { get; set; }
        public double Gap {get; set;}

        public abstract List<Rect> CalculateLayoutFrames();

    }
}
