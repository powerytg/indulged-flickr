using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX.OSD
{
    public partial class FilterContainerHUD : UserControl
    {
        // Events
        public EventHandler OnDismiss;
        public EventHandler<DeleteFilterEventArgs> OnDelete;

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(FilterBase), typeof(FilterContainerHUD), new PropertyMetadata(OnFilterPropertyChanged));

        public FilterBase Filter
        {
            get
            {
                return (FilterBase)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        public static void OnFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FilterContainerHUD)sender).OnFilterChanged();
        }

        protected virtual void OnFilterChanged()
        {
            TitleLabel.Text = Filter.StatusBarName;
            FilterContainer.Content = Filter;

            // Re-calculate height
            this.Height = Filter.Height + 140;
        }

        // Constructor
        public FilterContainerHUD()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDismiss != null)
            {
                OnDismiss(this, null);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                var evt = new DeleteFilterEventArgs();
                evt.Filter = Filter;
                OnDelete(this, evt);
            }
        }

        private void OSDToggle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (OnDismiss != null)
            {
                OnDismiss(this, null);
            }
        }
    }
}
