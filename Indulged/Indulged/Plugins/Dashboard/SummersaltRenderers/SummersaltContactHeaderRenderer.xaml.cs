using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltContactHeaderRenderer : UserControl
    {
        public static readonly DependencyProperty UserSourceProperty = DependencyProperty.Register("UserSource", typeof(User), typeof(SummersaltContactHeaderRenderer), new PropertyMetadata(OnUserSourcePropertyChanged));

        public User UserSource
        {
            get
            {
                return (User)GetValue(UserSourceProperty);
            }
            set
            {
                SetValue(UserSourceProperty, value);
            }
        }

        public static void OnUserSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltContactHeaderRenderer)sender).OnUserSourceChanged();

        }

        protected virtual void OnUserSourceChanged()
        {
            TitleLabel.Text = "By " + UserSource.Name;
        }

        // Constructor
        public SummersaltContactHeaderRenderer()
        {
            InitializeComponent();
        }

    }
}
