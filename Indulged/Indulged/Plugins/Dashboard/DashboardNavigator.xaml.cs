using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Utils;
using Indulged.Plugins.Dashboard.Events;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Indulged.Plugins.Dashboard
{
    public partial class DashboardNavigator : UserControl
    {
        // Events
        public static EventHandler<DashboardPageEventArgs> DashboardPageChanged;

        public static EventHandler RequestPreludePage;
        public static EventHandler RequestVioletPage;
        public static EventHandler RequestSummersaltPage;


        // Constructor
        public DashboardNavigator()
        {
            InitializeComponent();

            // Events
            RequestVioletPage += OnRequestVioletPage;
            RequestSummersaltPage += OnRequestSummersaltPage;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = new DashboardPageEventArgs();
            PivotItem selectedItem = MainPivot.SelectedItem as PivotItem;
            evt.SelectedPage = (IDashboardPage)selectedItem.Content;
            DashboardPageChanged.DispatchEvent(this, evt);
        }

        private void OnRequestVioletPage(object sender, EventArgs e)
        {
            MainPivot.SelectedIndex = 1;
        }

        private void OnRequestSummersaltPage(object sender, EventArgs e)
        {
            MainPivot.SelectedIndex = 2;
        }

        public void ResetListSelections()
        {
            PreludeView.ResetListSelection();
        }

        public void RefreshPreludeStreams()
        {
            PreludeView.RefreshStreams();
        }

        public void RefreshVioletStreams()
        {
            VioletView.ReloadStreams();
        }

        public void OnNavigatedFromPage()
        {
            //LayoutRoot.Visibility = Visibility.Collapsed; 
            ResetListSelections();            
        }

        public void OnNavigatedToPage()
        {
            //LayoutRoot.Visibility = Visibility.Visible;
            Dispatcher.BeginInvoke(() => {
                //PerformAppearanceAnimation();
            });
        }

        private void PerformAppearanceAnimation()
        {
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            CompositeTransform ct = (CompositeTransform)LayoutRoot.RenderTransform;
            ct.TranslateY = -h;

            LayoutRoot.Visibility = Visibility.Visible;
            
            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = animation.Duration;
            galleryAnimation.To = 0;
            galleryAnimation.EasingFunction = new BounceEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            animation.Begin();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //PerformAppearanceAnimation();
        }
    }
}
