using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.Plugins.Dashboard.SummersaltRenderers;
using Indulged.API.Cinderella.Models;
using Indulged.API.Avarice.Controls;
using System.Windows;

namespace Indulged.Plugins.Dashboard
{
    public class SummersaltRendererSelector : DataTemplateSelector
    {
        // Layout templates
        public DataTemplate CurrentUserTemplate { get; set; }
        public DataTemplate ContactPhotoTemplate { get; set; }
        public DataTemplate ContactPhotoHeaderTemplate { get; set; }
        public DataTemplate ContactPhotoFooterTemplate { get; set; }
        public DataTemplate ActivityHeaderTemplate { get; set; }
        public DataTemplate ActivityPhotoCommentEventTemplate { get; set; }
        public DataTemplate ActivityContactHeaderTemplate { get; set; }
        public DataTemplate ActivityPhotoTemplate { get; set; }

        // Random generator
        private Random randomGenerator = new Random();

        // Template cache
        private Dictionary<string, DataTemplate> templateCache = new Dictionary<string, DataTemplate>();

        // Photo group templates
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

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Type itemType = item.GetType();
            if (itemType == typeof(User))
                return CurrentUserTemplate;
            else if (itemType == typeof(Photo))
                return ContactPhotoTemplate;
            else if (itemType == typeof(SummersaltContactPhotoHeaderModeal))
                return ContactPhotoHeaderTemplate;
            else if (itemType == typeof(SummersaltContactPhotoFooterModel))
                return ContactPhotoFooterTemplate;
            else if (itemType == typeof(SummersaltActivityHeaderModel))
                return ActivityHeaderTemplate;
            else if (itemType == typeof(PhotoActivityCommentEvent))
                return ActivityPhotoCommentEventTemplate;
            else if (itemType == typeof(SummersaltContactHeaderModel))
                return ActivityContactHeaderTemplate;
            else if (itemType == typeof(PhotoActivity))
                return ActivityPhotoTemplate;
            else if (itemType == typeof(PhotoGroup))
                return SelectPhotoGroupTemplate(item, container);

            // Default
            return null;
        }

        protected DataTemplate SelectPhotoGroupTemplate(object item, DependencyObject container)
        {
            PhotoGroup photoGroup = item as PhotoGroup;
            if (templateCache.ContainsKey(photoGroup.ResourceId))
                return templateCache[photoGroup.ResourceId];

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
                if (num % 2 == 0)
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate5;
                    return LayoutTemplate5;
                }
                else
                {
                    templateCache[photoGroup.ResourceId] = LayoutTemplate5A;
                    return LayoutTemplate5A;
                }
            }

            // Default
            templateCache[photoGroup.ResourceId] = LayoutTemplate1;
            return LayoutTemplate1;
        }
    }
}
