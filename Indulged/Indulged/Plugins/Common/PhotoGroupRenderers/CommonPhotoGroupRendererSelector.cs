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
        public DataTemplate Renderer1 { get; set; }
        public DataTemplate Renderer2 { get; set; }
        public DataTemplate Renderer3 { get; set; }

        private DataTemplate LayoutTemplateByIdentifier(int index)
        {
            switch (index)
            {
                case 0:
                    return HeadlineLayoutTemplate;
                case 1:
                    return Renderer1;
                case 2:
                    return Renderer2;
                case 3:
                    return Renderer3;
                default:
                    return Renderer1;
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PhotoGroup photoGroup = item as PhotoGroup;
            if (photoGroup.IsHeadline)
            {
                return HeadlineLayoutTemplate;
            }

            if (templateCache.ContainsKey(photoGroup.ResourceId))
            {
                return LayoutTemplateByIdentifier(templateCache[photoGroup.ResourceId]);
            }

            if (photoGroup.Photos.Count == 1)
            {
                templateCache[photoGroup.ResourceId] = 1;
                return Renderer1;
            }
            else if (photoGroup.Photos.Count == 2)
            {
                templateCache[photoGroup.ResourceId] = 2;
                return Renderer2;
            }
            else if (photoGroup.Photos.Count == 3)
            {
                templateCache[photoGroup.ResourceId] = 3;
                return Renderer3;
            }
            
            // Default
            templateCache[photoGroup.ResourceId] = 1;
            return Renderer1;
        }
    }

}
