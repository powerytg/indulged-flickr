using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Indulged.API.Cinderella.Models;
using System.Windows.Documents;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltActivityFaveEventRenderer : UserControl
    {
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(PhotoActivityCommentEvent), typeof(SummersaltActivityFaveEventRenderer), new PropertyMetadata(OnEventPropertyChanged));

        public PhotoActivityCommentEvent Event
        {
            get
            {
                return (PhotoActivityCommentEvent)GetValue(EventProperty);
            }
            set
            {
                SetValue(EventProperty, value);
            }
        }

        public static void OnEventPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltActivityFaveEventRenderer)sender).OnEventChanged();
        }

        protected virtual void OnEventChanged()
        {
            AvatarView.Source = new BitmapImage(new Uri(Event.EventUser.AvatarUrl));
            TitleLabel.Text = Event.EventUser.Name + " commented on " + Event.CreationDate.ToShortDateString();
        }

        // Constructor
        public SummersaltActivityFaveEventRenderer()
        {
            InitializeComponent();
        }


    }
}
