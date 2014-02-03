using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public partial class PhotoGroupRenderer2 : CommonPhotoGroupRendererBase
    {
        // Constructor
        public PhotoGroupRenderer2()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();

            if (IsPortraitAspectRatio(PhotoGroupSource.Photos[0]) || IsPortraitAspectRatio(PhotoGroupSource.Photos[1]))
            {
                LayoutHorizontally();
            }
            else
            {
                LayoutVertically();
            }
        }

        private void LayoutHorizontally()
        {
            LayoutRoot.ClearValue(FrameworkElement.HeightProperty);
            LayoutRoot.MaxHeight = 280;

            Renderer1.PhotoSource = PhotoGroupSource.Photos[0];
            Renderer1.context = PhotoGroupSource.context;
            Renderer1.contextType = PhotoGroupSource.contextType;

            Renderer2.PhotoSource = PhotoGroupSource.Photos[1];
            Renderer2.context = PhotoGroupSource.context;
            Renderer2.contextType = PhotoGroupSource.contextType;

            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            float ratio;
            int percent1, percent2;
            if (Renderer1.PhotoSource.Width > 0 && Renderer2.PhotoSource.Width > 0)
            {
                ratio = (float)Renderer1.PhotoSource.Width / (float)(Renderer1.PhotoSource.Width + Renderer2.PhotoSource.Width);
            }
            else
            {
                int f1 = Math.Min(Renderer1.PhotoSource.MediumWidth, Renderer1.PhotoSource.MediumHeight);
                int f2 = Math.Min(Renderer2.PhotoSource.MediumWidth, Renderer2.PhotoSource.MediumHeight);
                if (f1 != 0 && f2 != 0)
                {
                    ratio = (float)f1 / (float)(f1 + f2);
                }
                else
                {
                    ratio = 0.6f;
                }
            }

            if (ratio < 0.3f)
            {
                ratio = 0.3f;
            }

            if (ratio > 0.75f)
            {
                ratio = 0.75f;
            }

            percent1 = (int)Math.Floor(ratio * 100);
            percent2 = 100 - percent1;

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(percent1, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(percent2, GridUnitType.Star) });

            Renderer1.SetValue(Grid.ColumnProperty, 0);
            Renderer2.SetValue(Grid.ColumnProperty, 1);
        }

        private void LayoutVertically()
        {
            LayoutRoot.Height = 540;
            LayoutRoot.ClearValue(FrameworkElement.MaxHeightProperty);

            Renderer1.PhotoSource = PhotoGroupSource.Photos[0];
            Renderer1.context = PhotoGroupSource.context;
            Renderer1.contextType = PhotoGroupSource.contextType;

            Renderer2.PhotoSource = PhotoGroupSource.Photos[1];
            Renderer2.context = PhotoGroupSource.context;
            Renderer2.contextType = PhotoGroupSource.contextType;

            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            float ratio;
            int percent1, percent2;
            if (Renderer1.PhotoSource.Height > 0 && Renderer2.PhotoSource.Height > 0)
            {
                ratio = (float)Renderer1.PhotoSource.Height / (float)(Renderer1.PhotoSource.Height + Renderer2.PhotoSource.Height);
            }
            else
            {
                int f1 = Math.Min(Renderer1.PhotoSource.MediumWidth, Renderer1.PhotoSource.MediumHeight);
                int f2 = Math.Min(Renderer2.PhotoSource.MediumWidth, Renderer2.PhotoSource.MediumHeight);
                if (f1 != 0 && f2 != 0)
                {
                    ratio = (float)f1 / (float)(f1 + f2);
                }
                else
                {
                    ratio = 0.6f;
                }
            }

            if (ratio < 0.3f)
            {
                ratio = 0.3f;
            }

            if (ratio > 0.75f)
            {
                ratio = 0.75f;
            }

            percent1 = (int)Math.Floor(ratio * 100);
            percent2 = 100 - percent1;

            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(percent1, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(percent2, GridUnitType.Star) });

            Renderer1.SetValue(Grid.RowProperty, 0);
            Renderer2.SetValue(Grid.RowProperty, 1);
        }

    }
}
