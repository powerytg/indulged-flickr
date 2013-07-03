using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Indulged.API.Avarice.Controls.SupportClasses.Magazine;

namespace Indulged.API.Avarice.Controls
{
    public class Magazine : Panel
    {
        // Number of cells
        public int CellCount { get; set; }

        // Gap
        public double Gap { get; set; }

        // Layout manager
        protected MagazineLayoutManager layoutManager;
        public MagazineLayoutManager LayoutManager
        {
            get
            {
                return layoutManager;
            }
        }

        // Page size
        public Size PageSize { get; set; }

        // Constructor
        public Magazine()
            : base()
        {
            Gap = 5;
            layoutManager = new MagazineLayoutManager();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public List<Rect> ReflowCellsNow()
        {
            double w = PageSize.Width;
            double h = PageSize.Height;

            layoutManager.PageSize = new Size(w, h);
            layoutManager.CellCount = CellCount;
            layoutManager.Gap = Gap;
            List<Rect> rects = layoutManager.ReflowLayout();

            return rects;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double w = PageSize.Width;
            double h = PageSize.Height;

            if (w == 0 || h == 0)
                return new Size(0, 0);

            layoutManager.PageSize = PageSize;
            layoutManager.CellCount = CellCount;
            layoutManager.Gap = Gap;
            List<Rect> rects = layoutManager.ReflowLayout();

            return new Size(w, h * layoutManager.Templates.Count);
        }
         
        protected override Size ArrangeOverride(Size finalSize)
        {
            layoutManager.PageSize = PageSize;
            layoutManager.CellCount = CellCount;
            layoutManager.Gap = Gap;
            List<Rect> rects = ReflowCellsNow();

            int i = 0;
            foreach (var child in this.Children)
            {
                Rect rect = rects[i];
                child.Arrange(rect);
                i++;
            }

            return base.ArrangeOverride(finalSize);
        }

    }
}
