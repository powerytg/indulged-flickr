using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using System;
using System.Collections.Generic;
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
        public DataTemplate SeperatorTemplate { get; set; }

        // Random generator
        private Random randomGenerator = new Random();

        // Template cache
        private Dictionary<string, int> templateCache = new Dictionary<string, int>();

        // Photo group templates
        public DataTemplate HeadlineLayoutTemplate { get; set; }
        public DataTemplate LayoutTemplate1 { get; set; }
        public DataTemplate LayoutTemplate2 { get; set; }
        public DataTemplate LayoutTemplate3 { get; set; }

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
            else if (itemType == typeof(SummersaltSeperatorModel))
                return SeperatorTemplate;
            else if (itemType == typeof(PhotoGroup))
                return SelectPhotoGroupTemplate(item, container);

            // Default
            return null;
        }

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
                default:
                    return LayoutTemplate1;
            }
        }

        protected DataTemplate SelectPhotoGroupTemplate(object item, DependencyObject container)
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
                return LayoutTemplate3;
            }

            // Default
            templateCache[photoGroup.ResourceId] = 1;
            return LayoutTemplate1;
        }
    }
}
