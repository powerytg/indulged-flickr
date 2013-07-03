using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class VirtualLayout1 : VirtualLayoutBase
    {
        public VirtualLayout1()
            : base()
        {
            TileCount = 4;
        }

        public override List<Rect> CalculateLayoutFrames()
        {
            double baseX = PageRect.X;
            double baseY = PageRect.Y;
            double w = PageRect.Width;
            double h = PageRect.Height - Gap;

            double smallWidth = Math.Floor((w - Gap) / 2);
            double cellHeight = Math.Floor((h - Gap * 2) / 3);

            var result = new List<Rect>();
            result.Add(new Rect(baseX, baseY, w, cellHeight));
            result.Add(new Rect(baseX, baseY + cellHeight + Gap, w, cellHeight));
            result.Add(new Rect(baseX, baseY + cellHeight * 2 + Gap * 2, smallWidth, cellHeight));
            result.Add(new Rect(baseX + smallWidth + Gap, baseY + cellHeight * 2 + Gap * 2, smallWidth, cellHeight));

            return result;
        }

    }
}
