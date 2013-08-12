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

namespace Indulged.Plugins.ProFX
{
    public partial class FXFilterStatusView : UserControl
    {
        public static readonly DependencyProperty SelectedFilterProperty = DependencyProperty.Register("SelectedFilter", typeof(FilterBase), typeof(FXFilterStatusView), new PropertyMetadata(OnSelectedFilterPropertyChanged));

        public FilterBase SelectedFilter
        {
            get
            {
                return (FilterBase)GetValue(SelectedFilterProperty);
            }
            set
            {
                SetValue(SelectedFilterProperty, value);
            }
        }

        public static void OnSelectedFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FXFilterStatusView)sender).OnSelectedFilterChanged();
        }

        protected virtual void OnSelectedFilterChanged()
        {
            TitleButton.Content = SelectedFilter.DisplayName;
        }

        // Constructor
        public FXFilterStatusView()
        {
            InitializeComponent();
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            var evt = new DismissFilterEventArgs();
            evt.Filter = SelectedFilter;
            ImageProcessingPage.RequestDismissFilterView(this, evt);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            var evt = new DismissFilterEventArgs();
            evt.Filter = SelectedFilter;
            ImageProcessingPage.RequestDismissFilterView(this, evt);
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilter.DeleteFilterAsync();
        }
    }
}
