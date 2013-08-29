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
using System.Windows.Media;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class TopicReplyRenderer : UserControl
    {
        // Reply source
        public static readonly DependencyProperty ReplySourceProperty = DependencyProperty.Register("ReplySource", typeof(TopicReply), typeof(TopicReplyRenderer), new PropertyMetadata(OnReplySourcePropertyChanged));

        public TopicReply ReplySource
        {
            get
            {
                return (TopicReply)GetValue(ReplySourceProperty);
            }
            set
            {
                SetValue(ReplySourceProperty, value);
            }
        }

        public static void OnReplySourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TopicReplyRenderer)sender).OnReplySourceChanged();
        }

        protected void OnReplySourceChanged()
        {
        }

        // Constructor
        public TopicReplyRenderer()
        {
            InitializeComponent();
        }
    }
}
