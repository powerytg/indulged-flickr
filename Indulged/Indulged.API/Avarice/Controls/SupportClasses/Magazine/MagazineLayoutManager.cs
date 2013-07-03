using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

using Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts;

namespace Indulged.API.Avarice.Controls.SupportClasses.Magazine
{
    public class MagazineLayoutManager
    {
        // Gap
        public double Gap { get; set; }

        // Layout templates
        private List<string> templateClassNames;

        // Templates
        private List<VirtualLayoutBase> templates;
        public List<VirtualLayoutBase> Templates
        {
            get
            {
                return templates;
            }
        }

        // Frame size
        public Size PageSize { get; set; }

        // Number of cells
        public int CellCount { get; set; }

        // Random generator
        private Random randomGenerator = new Random();

        // Constructor
        public MagazineLayoutManager()
        {
            templateClassNames = new List<string>();
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout1");
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout2");
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout3");
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout4");
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout5");
            templateClassNames.Add("Indulged.API.Avarice.Controls.SupportClasses.Magazine.Layouts.VirtualLayout6");

            // Initial templates
            templates = new List<VirtualLayoutBase>();
        }

        // Create a template from the class name
        private VirtualLayoutBase CreateTemplate(string templateName)
        {
            Type templateType = Type.GetType(templateName);
            return (VirtualLayoutBase)Activator.CreateInstance(templateType);
        }

        // How many available cells can the templates support
        private int AvailableSlotCount()
        {
            int result = 0;
            foreach (var template in templates)
            {
                result += template.TileCount;
            }

            return result;
        }

        // Reflow the layout and calculate position/size of each child tile
        public List<Rect> ReflowLayout()
        {
            if (CellCount == 0)
                return new List<Rect>();

            int numCellNeeded = CellCount - this.AvailableSlotCount();
            if(numCellNeeded > 0)
                CreateMoreTemplates(numCellNeeded);

            List<Rect> layouts = new List<Rect>();
            for (int i = 0; i < templates.Count; i++)
            {
                double pageY = PageSize.Height * i;
                VirtualLayoutBase template = templates[i];
                template.PageRect = new Rect(0, pageY, PageSize.Width, PageSize.Height);
                template.Gap = Gap;
                List<Rect> rects = template.CalculateLayoutFrames();
                foreach (var rect in rects)
                {
                    layouts.Add(rect);
                }
            }

            return layouts;
        }

        protected void CreateMoreTemplates(int numSlotsNeeded)
        {
            int numProcesses = 0;
            while (numProcesses < numSlotsNeeded)
            {
                int randomIndex = randomGenerator.Next(0, templateClassNames.Count);
                VirtualLayoutBase template = CreateTemplate(templateClassNames[randomIndex]);
                numProcesses += template.TileCount;
                // Debug.WriteLine(randomIndex);
                templates.Add(template);
            }
        }

    }
}
