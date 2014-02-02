using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Common.PhotoRenderers
{
    public partial class FullPhotoRenderer : PhotoRendererBase
    {
        public FullPhotoRenderer()
        {
            InitializeComponent();
        }

        private SolidColorBrush transparentBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        private SolidColorBrush solidBrush = new SolidColorBrush(Color.FromArgb(255, 25, 29, 36));
        private SolidColorBrush semiTransparentBrush = new SolidColorBrush(Color.FromArgb(215, 25, 29, 36));

        protected override void OnPhotoSourceChanged()
        {
            base.OnPhotoSourceChanged();
            ImageView.Source = PhotoRendererDecoder.GetDecodedBitmapImage(PhotoSource, PhotoRendererDecoder.DecodeResolutions.High);
            if (PhotoSource.Title.Length > 0)
            {
                TitleLabel.Text = PhotoSource.Title;
                TitleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TitleLabel.Visibility = Visibility.Collapsed;
            }

            if (PhotoSource.Description.Length > 0)
            {
                DescLabel.Text = PhotoSource.Description;
                DescLabel.Visibility = Visibility.Visible;
            }
            else
            {
                DescLabel.Visibility = Visibility.Collapsed;
            }

            if (PhotoSource.CommentCount > 0 || PhotoSource.ViewCount > 0)
            {
                StatView.PhotoSource = PhotoSource;
                StatView.Visibility = Visibility.Visible;
            }
            else
            {
                StatView.Visibility = Visibility.Collapsed;
            }

            // Roughly estimate how "long" the title and desc can be
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            SidePanel.ClearValue(FrameworkElement.MaxWidthProperty);
            SidePanel.ClearValue(FrameworkElement.MaxHeightProperty);
            SidePanel.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
            SidePanel.ClearValue(FrameworkElement.VerticalAlignmentProperty);
            ImageView.ClearValue(FrameworkElement.MaxWidthProperty);
            ImageView.ClearValue(FrameworkElement.MaxHeightProperty);

            int textLength = PhotoSource.Title.Length + PhotoSource.Description.Length;

            if (textLength <= 100)
            {
                // Put the content to bottom, overlapping image
                LayoutRoot.MaxHeight = 300;

                SidePanel.VerticalAlignment = VerticalAlignment.Bottom;
                SidePanel.MaxHeight = 200;

                if (PhotoSource.Description.Length <= 0)
                {
                    SidePanel.Background = transparentBrush;
                }
                else
                {
                    SidePanel.Background = semiTransparentBrush;
                }
            }
            if (textLength > 100 && textLength < 250)
            {
                LayoutRoot.MaxHeight = 300;

                if (PhotoSource.Width < PhotoSource.Height)
                {
                    // Portrait
                    SidePanel.Background = solidBrush;
                    LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    ImageView.MaxWidth = 300;
                    ImageView.SetValue(Grid.ColumnProperty, 0);
                    SidePanel.SetValue(Grid.ColumnProperty, 1);

                }
                else
                {
                    // Landscape
                    SidePanel.Background = semiTransparentBrush;
                    SidePanel.HorizontalAlignment = HorizontalAlignment.Right;
                    SidePanel.MaxWidth = 200;
                }
            }
            else if (textLength >= 250)
            {
                LayoutRoot.MaxHeight = 600;

                // Put the content to bottom and uses dedicated section
                SidePanel.Background = solidBrush;

                LayoutRoot.RowDefinitions.Add(new RowDefinition() { MaxHeight = 300 });
                LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                ImageView.SetValue(Grid.RowProperty, 0);
                SidePanel.SetValue(Grid.RowProperty, 1);
            }
        }

    }
}
