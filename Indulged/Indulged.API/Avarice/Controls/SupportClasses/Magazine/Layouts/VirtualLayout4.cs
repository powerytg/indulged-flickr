using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class VirtualLayout4 : VirtualLayoutBase
    {
        public VirtualLayout4()
            : base()
        {
            TileCount = 3;
        }

        public override List<Rect> CalculateLayoutFrames()
        {
            double baseX = PageRect.X;
            double baseY = PageRect.Y;
            double w = PageRect.Width;
            double h = PageRect.Height - Gap;

            double leftWidth = w * 0.67;
            double rightWidth = w - leftWidth - Gap;
            double rightHeight = (h - Gap) / 2;

            var result = new List<Rect>();
            result.Add(new Rect(baseX, baseY, leftWidth, h));
            result.Add(new Rect(baseX + leftWidth + Gap, baseY, rightWidth, rightHeight));
            result.Add(new Rect(baseX + leftWidth + Gap, baseY + rightHeight + Gap, rightWidth, rightHeight));

            return result;
        }

    }
}
