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
        // Layout templates
        public DataTemplate LayoutTemplate1 { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            List<Photo> photoGroup = item as List<Photo>;

            // Default
             return LayoutTemplate1;
        }
    }
}
