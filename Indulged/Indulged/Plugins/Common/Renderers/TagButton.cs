using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Indulged.Plugins.Common.Renderers
{
    public class TagButton : Button
    {
        // Tag string
        public string TagName { get; set; }

         public TagButton() : base()
        {
            DefaultStyleKey = typeof(TagButton);

             // Events
            Click += OnClick;
        }

         protected void OnClick(object sender, RoutedEventArgs e)
         {
             Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
             PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

             // Get photo collection context
             currentPage.NavigationService.Navigate(new Uri("/Plugins/Search/SearchResultPage.xaml?tags=" + Content.ToString(), UriKind.Relative));
         }
    }
}
