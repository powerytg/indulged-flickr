using Indulged.API.Cinderella.Models;
using Indulged.PolKit;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Indulged.Plugins.Common.PhotoRenderers
{
    public class PhotoRendererBase : UserControl
    {
        public string context { get; set; }
        public string contextType { get; set; }

        protected int estimatedMaxWidth = 400;

        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoRendererBase), new PropertyMetadata(OnPhotoSourcePropertyChanged));

        public Photo PhotoSource
        {
            get 
            { 
                return (Photo)GetValue(PhotoSourceProperty); 
            }
            set 
            { 
                SetValue(PhotoSourceProperty, value);
            }
        }

        public static void OnPhotoSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PhotoRendererBase)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
        }

        // Constructor
        public PhotoRendererBase()
            : base()
        {
            
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            base.OnTap(e);

            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            // Get photo collection context
            string collectionContext = PolicyKit.VioletPageSubscription;

            string urlString = "/Plugins/Detail/DetailPage.xaml?photo_id=" + PhotoSource.ResourceId + "&context=" + context;
            if (contextType != null)
                urlString += "&context_type=" + contextType;

            currentPage.NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
        }

    }
}
