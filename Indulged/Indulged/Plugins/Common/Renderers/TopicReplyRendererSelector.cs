using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.Plugins.Common.Renderers
{
    public class TopicReplyRendererSelector : DataTemplateSelector
    {
        // Layout templates
        public DataTemplate TopicTemplate { get; set; }
        public DataTemplate ReplyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(Topic))
                return TopicTemplate;
            else
                return ReplyTemplate;
        }
    }

}
