using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public class CommonPhotoGroupRendererSelector : DataTemplateSelector
    {
        // Random generator
        private Random randomGenerator = new Random();

        // Template cache
        private Dictionary<string, int> templateCache = new Dictionary<string, int>();

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

        private DataTemplate LayoutTemplateByIdentifier(int index)
        {
            switch (index)
            {
                case 0:
                    return HeadlineLayoutTemplate;
                case 1:
                    return LayoutTemplate1;
                case 2:
                    return LayoutTemplate2;
                case 3:
                    return LayoutTemplate3;
                case 4:
                    return LayoutTemplate3A;
                case 5:
                    return LayoutTemplate4;
                case 6:
                    return LayoutTemplate4A;
                case 7:
                    return LayoutTemplate4B;
                case 8:
                    return LayoutTemplate5;
                case 9:
                    return LayoutTemplate5A;
                default:
                    return LayoutTemplate1;
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PhotoGroup photoGroup = item as PhotoGroup;
            if (templateCache.ContainsKey(photoGroup.ResourceId))
                return LayoutTemplateByIdentifier(templateCache[photoGroup.ResourceId]);

            if (photoGroup.Photos.Count == 1)
            {
                templateCache[photoGroup.ResourceId] = 1;
                return LayoutTemplate1;
            }
            else if (photoGroup.Photos.Count == 2)
            {
                templateCache[photoGroup.ResourceId] = 2;
                return LayoutTemplate2;
            }
            else if (photoGroup.Photos.Count == 3)
            {
                if (randomGenerator.Next(0, 100) % 2 == 0)
                {
                    templateCache[photoGroup.ResourceId] = 3;
                    return LayoutTemplate3;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = 4;
                    return LayoutTemplate3A;
                }
            }
            else if (photoGroup.Photos.Count == 4)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 3 == 0)
                {
                    templateCache[photoGroup.ResourceId] = 5;
                    return LayoutTemplate4;
                }
                else if (num % 3 == 1)
                {
                    templateCache[photoGroup.ResourceId] = 6;
                    return LayoutTemplate4A;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = 7;
                    return LayoutTemplate4B;
                }
            }
            else if (photoGroup.Photos.Count == 5)
            {
                int num = randomGenerator.Next(0, 100);
                if (num % 2 == 0)
                {
                    templateCache[photoGroup.ResourceId] = 8;
                    return LayoutTemplate5;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = 9;
                    return LayoutTemplate5A;
                }
            }

            // Default
            templateCache[photoGroup.ResourceId] = 1;
            return LayoutTemplate1;
        }
    }

}
