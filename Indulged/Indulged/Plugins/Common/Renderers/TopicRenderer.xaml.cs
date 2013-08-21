using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.PolKit;
using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
using Indulged.API.Avarice.Controls;
using Indulged.Plugins.Group;
using Indulged.API.Cinderella;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class TopicRenderer : UserControl
    {
        public static readonly DependencyProperty TopicSourceProperty = DependencyProperty.Register("TopicSource", typeof(Topic), typeof(TopicRenderer), new PropertyMetadata(OnTopicSourcePropertyChanged));

        public Topic TopicSource
        {
            get
            {
                return (Topic)GetValue(TopicSourceProperty);
            }
            set
            {
                SetValue(TopicSourceProperty, value);
            }
        }

        public static void OnTopicSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TopicRenderer)sender).OnTopicSourceChanged();
        }

        protected virtual void OnTopicSourceChanged()
        {
            if (TopicSource.Author.AvatarUrl != null)
                IconView.Source = new BitmapImage(new Uri(TopicSource.Author.AvatarUrl));
            else
                IconView.Source = null;

            TitleLabel.Text = TopicSource.Subject;
            ContentLabel.Text = TopicSource.Message;

            string dateString = TopicSource.LastReplyDate.ToTimestampString();

            if (dateString.Contains("/"))
                DateLabel.Text = "Last posted  " + dateString;
            else
                DateLabel.Text = "Last posted  " + dateString + " ago";

            ViewLabel.Text = TopicSource.ReplyCount.ToShortString();
            
        }

        // Constructor
        public TopicRenderer()
        {
            InitializeComponent();
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            
        }
    }
}
