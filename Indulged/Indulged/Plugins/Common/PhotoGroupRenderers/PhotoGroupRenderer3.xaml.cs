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
    public partial class PhotoGroupRenderer3 : CommonPhotoGroupRendererBase
    {
        // Constructor
        public PhotoGroupRenderer3()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            Renderer1.PhotoSource = PhotoGroupSource.Photos[0];
            Renderer1.context = PhotoGroupSource.context;
            Renderer1.contextType = PhotoGroupSource.contextType;

            Renderer2.PhotoSource = PhotoGroupSource.Photos[1];
            Renderer2.context = PhotoGroupSource.context;
            Renderer2.contextType = PhotoGroupSource.contextType;

            Renderer3.PhotoSource = PhotoGroupSource.Photos[2];
            Renderer3.context = PhotoGroupSource.context;
            Renderer3.contextType = PhotoGroupSource.contextType;

            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            Renderer1.ClearValue(Grid.ColumnSpanProperty);
            Renderer1.ClearValue(Grid.RowSpanProperty);
            Renderer2.ClearValue(Grid.ColumnSpanProperty);
            Renderer2.ClearValue(Grid.RowSpanProperty);
            Renderer3.ClearValue(Grid.ColumnSpanProperty);
            Renderer3.ClearValue(Grid.RowSpanProperty);

            if (IsPortraitAspectRatio(Renderer1.PhotoSource))
            {
                // Large photo on left
                LayoutFirstPhotoOnLeft();
            }
            else if (IsPortraitAspectRatio(Renderer3.PhotoSource))
            {
                // Large photo on right
                LayoutLastPhotoOnRight();
            }
            else
            {
                // Large photo on top
                LayoutFirstPhotoOnTop();
            }
        }

        private void LayoutFirstPhotoOnLeft()
        {
            this.MaxHeight = 340;

            float hRatio = GetHorizontalRatio(Renderer1.PhotoSource, Renderer2.PhotoSource);
            float vRatio = GetVerticalRatio(Renderer2.PhotoSource, Renderer3.PhotoSource);

            int left = (int)Math.Floor(hRatio * 100);
            int right = 100 - left;
            int top = (int)Math.Floor(vRatio * 100);
            int bottom = 100 - top;

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(right, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(top, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(bottom, GridUnitType.Star) });

            Renderer1.SetValue(Grid.ColumnProperty, 0);
            Renderer1.SetValue(Grid.RowSpanProperty, 2);

            Renderer2.SetValue(Grid.ColumnProperty, 1);
            Renderer2.SetValue(Grid.RowProperty, 0);

            Renderer3.SetValue(Grid.ColumnProperty, 1);
            Renderer3.SetValue(Grid.RowProperty, 1);

        }

        private void LayoutLastPhotoOnRight()
        {
            this.MaxHeight = 340;

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240, GridUnitType.Pixel) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Renderer3.SetValue(Grid.ColumnProperty, 1);
            Renderer3.SetValue(Grid.RowSpanProperty, 2);

            Renderer1.SetValue(Grid.ColumnProperty, 0);
            Renderer1.SetValue(Grid.ColumnProperty, 0);

            Renderer2.SetValue(Grid.ColumnProperty, 0);
            Renderer2.SetValue(Grid.RowProperty, 1);

        }

        private void LayoutFirstPhotoOnTop()
        {
            this.MaxHeight = 480;

            float hRatio = GetHorizontalRatio(Renderer2.PhotoSource, Renderer3.PhotoSource);
            float vRatio = GetVerticalRatio(Renderer1.PhotoSource, Renderer2.PhotoSource);

            int left = (int)Math.Floor(hRatio * 100);
            int right = 100 - left;
            int top = (int)Math.Floor(vRatio * 100);
            top = Math.Max(top, 60);

            int bottom = 100 - top;

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left, GridUnitType.Star) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(right, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(top, GridUnitType.Star) });
            LayoutRoot.RowDefinitions.Add(new RowDefinition { Height = new GridLength(bottom, GridUnitType.Star) });

            Renderer1.SetValue(Grid.ColumnSpanProperty, 2);
            Renderer1.SetValue(Grid.RowProperty, 0);

            Renderer2.SetValue(Grid.ColumnProperty, 0);
            Renderer2.SetValue(Grid.RowProperty, 1);

            Renderer3.SetValue(Grid.ColumnProperty, 1);
            Renderer3.SetValue(Grid.RowProperty, 1);
        }

        private float GetHorizontalRatio(Photo leftPhoto, Photo rightPhoto)
        {
            float hRatio;
            if (leftPhoto.Width > 0 && rightPhoto.Width > 0)
            {
                hRatio = (float)leftPhoto.Width / (float)(leftPhoto.Width + rightPhoto.Width);
            }
            else
            {
                int f1 = Math.Min(leftPhoto.MediumWidth, leftPhoto.MediumHeight);
                int f2 = Math.Min(rightPhoto.MediumWidth, rightPhoto.MediumHeight);
                if (f1 != 0 && f2 != 0)
                {
                    hRatio = (float)f1 / (float)(f1 + f2);
                }
                else
                {
                    hRatio = 0.6f;
                }
            }

            if (hRatio < 0.4f)
            {
                hRatio = 0.4f;
            }

            if (hRatio > 0.75f)
            {
                hRatio = 0.75f;
            }

            return hRatio;
        }

        private float GetVerticalRatio(Photo topPhoto, Photo bottomPhoto)
        {
            float vRatio;
            if (topPhoto.Height > 0 && bottomPhoto.Height > 0)
            {
                vRatio = (float)topPhoto.Height / (float)(topPhoto.Height + bottomPhoto.Height);
            }
            else
            {
                int f1 = Math.Min(topPhoto.MediumWidth, topPhoto.MediumHeight);
                int f2 = Math.Min(bottomPhoto.MediumWidth, bottomPhoto.MediumHeight);
                if (f1 != 0 && f2 != 0)
                {
                    vRatio = (float)f1 / (float)(f1 + f2);
                }
                else
                {
                    vRatio = 0.6f;
                }
            }

            if (vRatio < 0.3f)
            {
                vRatio = 0.3f;
            }

            if (vRatio > 0.75f)
            {
                vRatio = 0.75f;
            }

            return vRatio;
        }
    }
}
