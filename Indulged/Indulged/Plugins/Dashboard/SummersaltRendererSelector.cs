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
        public DataTemplate ActivityPhotoTemplate { get; set; }
        public DataTemplate ActivityPhotoCommentEventTemplate { get; set; }
        public DataTemplate ActivityPhotoFaveEventTemplate { get; set; }


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
            else if (itemType == typeof(PhotoActivity))
                return ActivityPhotoTemplate;
            else if (itemType == typeof(PhotoActivityCommentEvent))
                return ActivityPhotoCommentEventTemplate;
            else if (itemType == typeof(PhotoActivityFaveEvent))
                return ActivityPhotoFaveEventTemplate;

            // Default
            return null;
        }
    }
}
