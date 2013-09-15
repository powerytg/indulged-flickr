﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;

using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events;
using Indulged.Plugins.Chrome;
using System.Windows.Media;
using Indulged.Plugins.Chrome.Events;

namespace Indulged.Plugins.ProFX
{
    public partial class FXBottomPanel : UserControl
    {
        // Active filter controller view
        public FilterBase CurrentFilter { get; set; }

        // Constructor
        public FXBottomPanel()
        {
            InitializeComponent();

            ApplyTheme();

            // Events
            ThemeManager.ThemeChanged += OnThemeChanged;
            ImageProcessingPage.RequestFilterListView += OnRequestGalleryView;
            ImageProcessingPage.RequestDismissFilterListView += OnRequestDismissGalleryView;
            ImageProcessingPage.RequestFilterView += OnRequestFilterView;
            ImageProcessingPage.RequestAddFilter += OnRequestAddFilter;
            ImageProcessingPage.RequestDismissFilterView += OnRequestDismissFilterView;
            ImageProcessingPage.RequestDeleteFilter += OnRequestDeleteFilter;
            ImageProcessingPage.RequestSettingsView += OnRequestSettingsView;
            ImageProcessingPage.RequestDismissSettingsView += OnRequestDismissSettingsView;
        }

        private void ApplyTheme()
        {
            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0, 0, 0));
                UpperBorderBrush.Color = Color.FromArgb(230, 0, 0, 0);
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0xff, 0xff, 0xff));
                UpperBorderBrush.Color = Color.FromArgb(230, 0xff, 0xff, 0xff);
            }
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme();
        }

        private void OnRequestGalleryView(object sender, EventArgs e)
        {
            SwitchToView(NormalStatusView, GalleryStatusView);
        }

        private void OnRequestDismissGalleryView(object sender, EventArgs e)
        {
            SwitchToView(GalleryStatusView, NormalStatusView);
        }

        private void OnRequestSettingsView(object sender, EventArgs e)
        {
            SwitchToView(NormalStatusView, SettingsStatusView);
        }

        private void OnRequestDismissSettingsView(object sender, EventArgs e)
        {
            SwitchToView(SettingsStatusView, NormalStatusView);
        }

        private void OnRequestAddFilter(object sender, AddFilterEventArgs e)
        {
            CurrentFilter = e.Filter;

            if (e.Filter.GetType() == typeof(FXCropFilter))
            {
                CropStatusView.SelectedFilter = e.Filter;
                SwitchToView(GalleryStatusView, CropStatusView);
            }
            else
            {
                FilterStatusView.SelectedFilter = e.Filter;
                SwitchToView(GalleryStatusView, FilterStatusView);
            }
        }

        private void OnRequestFilterView(object sender, RequestFilterViewEventArgs e)
        {
            CurrentFilter = e.Filter;

            if (e.Filter.GetType() == typeof(FXCropFilter))
            {
                CropStatusView.SelectedFilter = e.Filter;
                SwitchToView(GalleryStatusView, CropStatusView);
            }
            else
            {
                FilterStatusView.SelectedFilter = e.Filter; 
                SwitchToView(GalleryStatusView, FilterStatusView);
            }
            
        }

        private void OnRequestDismissFilterView(object sender, DismissFilterEventArgs e)
        {
            CurrentFilter = null;

            if (e.Filter.GetType() == typeof(FXCropFilter))
            {
                SwitchToView(CropStatusView, NormalStatusView);
            }
            else
            {
                SwitchToView(FilterStatusView, NormalStatusView);
            }

        }

        private void OnRequestDeleteFilter(object sender, DeleteFilterEventArgs e)
        {
            CurrentFilter = null;

            if (e.Filter.GetType() == typeof(FXCropFilter))
            {
                SwitchToView(CropStatusView, NormalStatusView);
            }
            else
            {
                SwitchToView(FilterStatusView, NormalStatusView);
            }
        }

        private void SwitchToView(FrameworkElement oldView, FrameworkElement newView)
        {
            newView.Opacity = 0;
            newView.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Old alpha animation
            DoubleAnimationUsingKeyFrames oldAnimation = new DoubleAnimationUsingKeyFrames();
            oldAnimation.Duration = animation.Duration;
            oldAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            oldAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(oldAnimation, oldView);
            Storyboard.SetTargetProperty(oldAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(oldAnimation);

            // New alpha animation
            DoubleAnimationUsingKeyFrames newAnimation = new DoubleAnimationUsingKeyFrames();
            newAnimation.Duration = animation.Duration;
            newAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            newAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.6), Value = 1.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(newAnimation, newView);
            Storyboard.SetTargetProperty(newAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(newAnimation);

            animation.Completed += (sender, evt) =>
            {
                oldView.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }

    }
}
