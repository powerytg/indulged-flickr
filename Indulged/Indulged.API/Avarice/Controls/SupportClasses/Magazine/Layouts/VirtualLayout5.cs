using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class VirtualLayout5 : VirtualLayoutBase
    {
        public VirtualLayout5()
            : base()
        {
            TileCount = 2;
        }

        public override List<Rect> CalculateLayoutFrames()
        {
            double baseX = PageRect.X;
            double baseY = PageRect.Y;
            double w = PageRect.Width;
            double h = PageRect.Height - Gap;

            double topHeight = h * 0.35;
            double bottomHeight = h - topHeight - Gap;
            
            var result = new List<Rect>();
            result.Add(new Rect(baseX, baseY, w, topHeight));
            result.Add(new Rect(baseX, baseY + topHeight + Gap, w, bottomHeight));

            return result;
        }

    }
}
