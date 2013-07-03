using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;

namespace Indulged.Plugins.Common
{
    public partial class ProgressView : UserControl
    {
        private Popup popup;

        public ProgressView()
        {
            InitializeComponent();
        }

        public void Show()
        {
            popup = new Popup();
            this.Width = (Application.Current.RootVisual as FrameworkElement).ActualWidth;
            this.Height = (Application.Current.RootVisual as FrameworkElement).ActualHeight;
            popup.Child = this;

            popup.IsOpen = true;
        }

        public void Close()
        {
            popup.IsOpen = false;
            popup = null;
        }

    }
}
