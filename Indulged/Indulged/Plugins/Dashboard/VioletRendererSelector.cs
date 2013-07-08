using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.Plugins.Dashboard.VioletRenderers;
using Indulged.API.Cinderella.Models;
using Indulged.API.Avarice.Controls;
using System.Windows;

namespace Indulged.Plugins.Dashboard
{
    public class VioletRendererSelector : DataTemplateSelector
    {
        // Random generator
        private Random randomGenerator = new Random();

        // Layout templates
        public DataTemplate LayoutTemplate1 { get; set; }
        public DataTemplate LayoutTemplate2 { get; set; }
        public DataTemplate LayoutTemplate3 { get; set; }
        public DataTemplate LayoutTemplate3A { get; set; }
        public DataTemplate LayoutTemplate4 { get; set; }
        public DataTemplate LayoutTemplate4A { get; set; }
        public DataTemplate LayoutTemplate4B { get; set; }
        public DataTemplate LayoutTemplate5 { get; set; }
        public DataTemplate LayoutTemplate5A { get; set; }
        public DataTemplate LayoutTemplate5B { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            List<Photo> photoGroup = item as List<Photo>;

            if (photoGroup.Count == 1)
                return LayoutTemplate1;
            else if (photoGroup.Count == 2)
                return LayoutTemplate2;
            else if (photoGroup.Count == 3)
            {
                if(randomGenerator.Next(0, 100) % 2 == 0)
                    return LayoutTemplate3; 
                else
                    return LayoutTemplate3A; 
            }
            else if (photoGroup.Count == 4)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 3 == 0)
                    return LayoutTemplate4;
                else if(num % 3 == 1)
                    return LayoutTemplate4A;
                else
                    return LayoutTemplate4B;
            }
            else if (photoGroup.Count == 5)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 3 == 0)
                    return LayoutTemplate5;
                else if (num % 3 == 1)
                    return LayoutTemplate5A;
                else
                    return LayoutTemplate5B;
            }

            // Default
             return LayoutTemplate1;
        }
    }
}
