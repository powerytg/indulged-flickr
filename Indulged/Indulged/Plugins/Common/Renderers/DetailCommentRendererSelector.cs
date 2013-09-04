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
    public class DetailCommentRendererSelector : DataTemplateSelector
    {
        // Layout templates
        public DataTemplate PhotoTemplate { get; set; }
        public DataTemplate CommentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(Photo))
                return PhotoTemplate;
            else
                return CommentTemplate;
        }
    }

}
