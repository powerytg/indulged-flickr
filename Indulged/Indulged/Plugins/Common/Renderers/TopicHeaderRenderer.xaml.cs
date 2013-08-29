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
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class TopicHeaderRenderer : UserControl
    {
        // Topic source
        public static readonly DependencyProperty TopicSourceProperty = DependencyProperty.Register("TopicSource", typeof(Topic), typeof(TopicHeaderRenderer), new PropertyMetadata(OnTopicSourcePropertyChanged));

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
            ((TopicHeaderRenderer)sender).OnTopicSourceChanged();
        }

        protected void OnTopicSourceChanged()
        {
            AuthorAvatarView.Source = new BitmapImage(new Uri(TopicSource.Author.AvatarUrl));
            AuthorLabelView.Text = TopicSource.Author.Name;

            FormatContentText();
            string dateString = "Posted on " + TopicSource.CreationDate.ToShortDateString();
            string replyString = TopicSource.ReplyCount == 0 ? "no reply yet" : TopicSource.ReplyCount.ToString() + " replies";
            ReplyDigestLabelView.Text = dateString + ",  " + replyString;
        }

        // Constructor
        public TopicHeaderRenderer()
        {
            InitializeComponent();
        }

        private void FormatContentText()
        {
            string trimmedMessage = TopicSource.Message.Trim();
            BodyTextView.Inlines.Clear();

            if (trimmedMessage.Length <= 1)
                return;

            Run initialRun = new Run();
            initialRun.FontWeight = FontWeights.Bold;
            initialRun.FontSize = 64;
            initialRun.Text = trimmedMessage.Substring(0, 1);
            BodyTextView.Inlines.Add(initialRun);

            Run bodyRun = new Run();
            bodyRun.FontWeight = FontWeights.Normal;
            bodyRun.FontSize = 24;
            bodyRun.Text = trimmedMessage.Substring(1);
            BodyTextView.Inlines.Add(bodyRun);

        }
    }
}
