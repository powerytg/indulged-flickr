using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Indulged.API.Cinderella.Models;
using System.Windows;

namespace Indulged.Plugins.Dashboard.VioletRenderers
{
    public class VioletRendererBase : UserControl
    {
        public static readonly DependencyProperty PhotoGroupSourceProperty = DependencyProperty.Register("PhotoGroupSource", typeof(PhotoGroup), typeof(VioletRendererBase), new PropertyMetadata(OnPhotoGroupSourcePropertyChanged));

        public PhotoGroup PhotoGroupSource
        {
            get 
            { 
                return (PhotoGroup)GetValue(PhotoGroupSourceProperty); 
            }
            set 
            { 
                SetValue(PhotoGroupSourceProperty, value);
            }
        }

        public static void OnPhotoGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((VioletRendererBase)sender).OnPhotoGroupSourceChanged();
        }

        protected virtual void OnPhotoGroupSourceChanged()
        {
        }

        // Constructor
        public VioletRendererBase()
            : base()
        {
        }
    }
}
