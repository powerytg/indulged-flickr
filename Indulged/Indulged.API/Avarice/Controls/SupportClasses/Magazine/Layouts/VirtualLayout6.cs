using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class VirtualLayout6 : VirtualLayoutBase
    {
        public VirtualLayout6()
            : base()
        {
            TileCount = 3;
        }

        public override List<Rect> CalculateLayoutFrames()
        {
            double baseX = PageRect.X;
            double baseY = PageRect.Y;
            double w = PageRect.Width;
            double h = Math.Max(0, PageRect.Height - Gap);

            double topHeight = h * 0.5;
            double bottomWidthLeft = w * 0.67;
            double bottomWidthRight = w - bottomWidthLeft - Gap;
            double bottomHeightLeft = h - topHeight - Gap;
            double bottomHeightRight = h - topHeight - Gap;

            var result = new List<Rect>();
            result.Add(new Rect(baseX, baseY, w, topHeight));
            result.Add(new Rect(baseX, baseY + topHeight + Gap, bottomWidthLeft, bottomHeightLeft));
            result.Add(new Rect(baseX +  bottomWidthLeft + Gap, baseY + topHeight + Gap, bottomWidthRight, bottomHeightRight));

            return result;
        }

    }
}
