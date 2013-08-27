using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Group.events;

namespace Indulged.Plugins.Group
{
    public partial class PhotoPickerRenderer : UserControl
    {
        // Global status
        public static bool CanSelect { get; set; }

        // Events
        public static EventHandler<PhotoPickerRendererEventArgs> SelectionChanged;

        private SolidColorBrush selectedBorderBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xef, 0x7c));
        private SolidColorBrush unselectedBorderBrush = new SolidColorBrush(Colors.Transparent);


        private SolidColorBrush selectedFillBrush = new SolidColorBrush(Color.FromArgb(0x44, 0x00, 0xef, 0x7c));
        private SolidColorBrush unselectedFillBrush = new SolidColorBrush(Colors.Transparent);

        private void UpdateSelectionState()
        {
            if (PhotoSource.Selected)
            {
                BorderView.Background = selectedFillBrush;
                BorderView.BorderBrush = selectedBorderBrush;
            }
            else
            {
                BorderView.Background = unselectedFillBrush;
                BorderView.BorderBrush = unselectedBorderBrush;
            }
        }

        // Photo source
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(SelectablePhoto), typeof(PhotoPickerRenderer), new PropertyMetadata(OnPhotoSourcePropertyChanged));

        public SelectablePhoto PhotoSource
        {
            get
            {
                return (SelectablePhoto)GetValue(PhotoSourceProperty);
            }
            set
            {
                SetValue(PhotoSourceProperty, value);
            }
        }

        public static void OnPhotoSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PhotoPickerRenderer)sender).OnPhotoSourceChanged();
        }

        // Constructor
        public PhotoPickerRenderer()
        {
            InitializeComponent();
        }

        protected void OnPhotoSourceChanged()
        {
            ImageView.Source = new BitmapImage(new Uri(PhotoSource.PhotoSource.GetImageUrl()));
            UpdateSelectionState();
        }

        // Events
        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            base.OnTap(e);

            if (!CanSelect)
            {
                if (PhotoSource.Selected)
                {
                    PhotoSource.Selected = false;
                    UpdateSelectionState();
                    DispatchSelectionEvent();
                }
            }
            else
            {
                PhotoSource.Selected = !PhotoSource.Selected;
                UpdateSelectionState();
                DispatchSelectionEvent();
            }

        }

        private void DispatchSelectionEvent()
        {
            // Dispach event
            if (SelectionChanged != null)
            {
                var evt = new PhotoPickerRendererEventArgs();
                evt.PhotoId = PhotoSource.PhotoSource.ResourceId;
                evt.Selected = PhotoSource.Selected;
                SelectionChanged(this, evt);
            }
        }
    }
}
