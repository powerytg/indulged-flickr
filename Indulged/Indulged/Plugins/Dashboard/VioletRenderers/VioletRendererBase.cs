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
        public static readonly DependencyProperty PhotoGroupProperty = DependencyProperty.Register("PhotoGroup", typeof(List<Photo>), typeof(VioletRendererBase), new PropertyMetadata(OnPhotoGroupPropertyChanged));

        public List<Photo> PhotoGroup
        {
            get 
            { 
                return (List<Photo>)GetValue(PhotoGroupProperty); 
            }
            set 
            { 
                SetValue(PhotoGroupProperty, value);
            }
        }

        public static void OnPhotoGroupPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((VioletRendererBase)sender).OnPhotoGroupChanged();
        }

        protected virtual void OnPhotoGroupChanged()
        {
        }

        // Constructor
        public VioletRendererBase()
            : base()
        {
        }
    }
}
