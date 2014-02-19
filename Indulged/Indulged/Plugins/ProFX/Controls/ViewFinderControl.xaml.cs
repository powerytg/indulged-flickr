using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class ViewFinderControl : UserControl
    {
        // Constructor
        public ViewFinderControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ViewFinderControl), new PropertyMetadata(OnSourcePropertyChanged));

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ViewFinderControl)sender).OnSourceChanged();
        }

        protected virtual void OnSourceChanged()
        {
            ImageView.Source = Source;
        }

        public static readonly DependencyProperty OriginalBitmapProperty = DependencyProperty.Register("OriginalBitmap", typeof(BitmapImage), typeof(ViewFinderControl), new PropertyMetadata(OnOriginalBitmapPropertyChanged));

        public BitmapImage OriginalBitmap
        {
            get
            {
                return (BitmapImage)GetValue(OriginalBitmapProperty);
            }
            set
            {
                SetValue(OriginalBitmapProperty, value);
            }
        }

        public static void OnOriginalBitmapPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ViewFinderControl)sender).OnOriginalBitmapChanged();
        }

        protected virtual void OnOriginalBitmapChanged()
        {
            double w = OriginalBitmap.PixelWidth;
            double h = OriginalBitmap.PixelHeight;
            InfoLabel.Text = w.ToString() + " by " + h.ToString() + " JPEG.  Preview Mode: Fill Screen";
        }
    }
}
