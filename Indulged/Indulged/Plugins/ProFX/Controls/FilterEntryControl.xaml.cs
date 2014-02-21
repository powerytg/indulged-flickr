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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class FilterEntryControl : UserControl
    {
        // Events
        public EventHandler OnDelete;
        public EventHandler OnVisibilityChanged;
        public EventHandler RequestFilterView;

        private static BitmapImage enabledImage = new BitmapImage(new Uri("/Assets/ProFX/FXFilterEnabled.png", UriKind.Relative));
        private static BitmapImage disabledImage = new BitmapImage(new Uri("/Assets/ProFX/FXFilterDisabled.png", UriKind.Relative));

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(FilterBase), typeof(FilterEntryControl), new PropertyMetadata(OnFilterPropertyChanged));

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
            ((FilterEntryControl)sender).OnFilterChanged();
        }

        protected virtual void OnFilterChanged()
        {
            Label.Text = Filter.DisplayName;
            UpdateVisibilityIcon();            
        }

        private void UpdateVisibilityIcon()
        {
            if (Filter.IsFilterEnabled)
            {
                Icon.Source = enabledImage;
            }
            else
            {
                Icon.Source = disabledImage;
            }
        }

        // Constructor
        public FilterEntryControl()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete(this, null);
            }
        }

        private void VisibilityButton_Click(object sender, RoutedEventArgs e)
        {            
            Filter.IsFilterEnabled = !Filter.IsFilterEnabled;
            UpdateVisibilityIcon();

            if (OnVisibilityChanged != null)
            {
                OnVisibilityChanged(this, null);
            }
        }

        private void Label_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (RequestFilterView != null)
            {
                RequestFilterView(this, null);
            }
        }
    }
}
