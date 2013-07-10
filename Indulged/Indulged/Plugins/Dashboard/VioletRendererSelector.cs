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

        // Template cache
        private Dictionary<string, DataTemplate> templateCache = new Dictionary<string, DataTemplate>();

        // Layout templates
        public DataTemplate HeadlineLayoutTemplate { get; set; }
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
            PhotoGroup photoGroup = item as PhotoGroup;
            if (templateCache.ContainsKey(photoGroup.ResourceId))
                return templateCache[photoGroup.ResourceId];

            if (photoGroup.IsHeadline)
            {
                templateCache[photoGroup.ResourceId] = HeadlineLayoutTemplate;
                return HeadlineLayoutTemplate;
            }

            if (photoGroup.Photos.Count == 1)
            {
                templateCache[photoGroup.ResourceId] = LayoutTemplate1;
                return LayoutTemplate1;
            }
            else if (photoGroup.Photos.Count == 2)
            {
                templateCache[photoGroup.ResourceId] = LayoutTemplate2;
                return LayoutTemplate2;
            }
            else if (photoGroup.Photos.Count == 3)
            {
                if (randomGenerator.Next(0, 100) % 2 == 0)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate3;
                    return LayoutTemplate3;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate3A;
                    return LayoutTemplate3A;
                }
            }
            else if (photoGroup.Photos.Count == 4)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 3 == 0)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate4;
                    return LayoutTemplate4;
                }
                else if (num % 3 == 1)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate4A;
                    return LayoutTemplate4A;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate4B;
                    return LayoutTemplate4B;
                }
            }
            else if (photoGroup.Photos.Count == 5)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 3 == 0)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate5;
                    return LayoutTemplate5;
                }
                else if (num % 3 == 1)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate5A;
                    return LayoutTemplate5A;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate5B;
                    return LayoutTemplate5B;
                }
            }

            // Default
            templateCache[photoGroup.ResourceId] = LayoutTemplate1;
             return LayoutTemplate1;
        }
    }
}
