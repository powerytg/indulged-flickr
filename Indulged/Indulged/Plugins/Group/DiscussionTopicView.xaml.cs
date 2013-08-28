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

namespace Indulged.Plugins.Group
{
    public partial class DiscussionTopicView : UserControl
    {
        // Photo source
        public static readonly DependencyProperty TopicSourceProperty = DependencyProperty.Register("TopicSource", typeof(Topic), typeof(DiscussionTopicView), new PropertyMetadata(OnTopicSourcePropertyChanged));

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
            ((DiscussionTopicView)sender).OnTopicSourceChanged();
        }

        protected void OnTopicSourceChanged()
        {
            FormatContentText();
        }

        // Constructor
        public DiscussionTopicView()
        {
            InitializeComponent();
        }

        private void FormatContentText()
        {
            string trimmedMessage = TopicSource.Message.Trim();
            BodyTextView.Inlines.Clear();

            if(trimmedMessage.Length <= 1)
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
