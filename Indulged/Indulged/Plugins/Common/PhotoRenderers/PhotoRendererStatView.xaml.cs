using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
using System.Windows;
using System.Windows.Controls;

namespace Indulged.Plugins.Common.PhotoRenderers
{
    public partial class PhotoRendererStatView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoRendererStatView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoRendererStatView)sender).OnPhotoSourceChanged();
        }

        protected void OnPhotoSourceChanged()
        {
            if (PhotoSource.ViewCount > 0)
            {
                ViewIcon.Visibility = Visibility.Visible;
                ViewLabel.Text = PhotoSource.ViewCount.ToShortString();
            }
            else
            {
                ViewIcon.Visibility = Visibility.Collapsed;
                ViewLabel.Text = null;
            }

        }

        // Constructor
        public PhotoRendererStatView()
        {
            InitializeComponent();
        }
        
    }
}
