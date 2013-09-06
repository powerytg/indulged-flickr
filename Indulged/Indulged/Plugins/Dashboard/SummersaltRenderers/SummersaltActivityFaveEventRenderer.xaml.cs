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
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(PhotoActivityFaveEvent), typeof(SummersaltActivityFaveEventRenderer), new PropertyMetadata(OnEventPropertyChanged));

        public PhotoActivityFaveEvent Event
        {
            get
            {
                return (PhotoActivityFaveEvent)GetValue(EventProperty);
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
            string userString = Event.FavUsers[0].Name;
            string endString = " added this photo as favourite";
            if (Event.FavUsers.Count == 1)
            {
                userString += endString;
            }
            else if (Event.FavUsers.Count == 2)
            {
                userString += " and " + Event.FavUsers[1].Name + endString;
            }
            else if (Event.FavUsers.Count == 3)
            {
                userString += " ," + Event.FavUsers[1].Name + " and " + Event.FavUsers[2].Name + endString;
            }
            else
            {
                int extCount = Event.FavUsers.Count - 3;
                if (extCount == 1)
                    endString = " and 1 other person added this photo as favourite";
                else
                    endString = " and " + extCount.ToString() + " other people added this photo as favourite";

                userString += " ," + Event.FavUsers[1].Name + ", " + Event.FavUsers[2].Name + endString;
            }

            TitleLabel.Text = userString;
        }

        // Constructor
        public SummersaltActivityFaveEventRenderer()
        {
            InitializeComponent();
        }


    }
}
