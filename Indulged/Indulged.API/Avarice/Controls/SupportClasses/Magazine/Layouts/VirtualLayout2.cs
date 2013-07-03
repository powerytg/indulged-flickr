using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts
{
    public class VirtualLayout2 : VirtualLayoutBase
    {
        public VirtualLayout2()
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

            double leftWidth = w * 0.67;
            double leftHeight = (h - Gap) / 2;

            double rightWidth = w - leftWidth - Gap;
            double rightHeight1 = h * 0.67;
            double rightHeight2 = h - rightHeight1 - Gap;

            var result = new List<Rect>();
            result.Add(new Rect(baseX, baseY, leftWidth, leftHeight));
            result.Add(new Rect(baseX, baseY + leftHeight + Gap, leftWidth, leftHeight));
            result.Add(new Rect(baseX + leftWidth + Gap, baseY, rightWidth, rightHeight1));
            result.Add(new Rect(baseX + leftWidth + Gap, baseY + rightHeight1 + Gap, rightWidth, rightHeight2));

            return result;
        }

    }
}
