using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class LayoutFrame
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        // Constructor
        public LayoutFrame(double _x, double _y, double _w, double _h)
        {
            X = _x;
            Y = _y;
            Width = _w;
            Height = _h;
        }

    }
}
