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
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class PhotoCommentDigestRenderer : UserControl
    {
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register("Comment", typeof(PhotoComment), typeof(PhotoCommentDigestRenderer), new PropertyMetadata(OnCommentPropertyChanged));

        public PhotoComment Comment
        {
            get
            {
                return (PhotoComment)GetValue(CommentProperty);
            }
            set
            {
                SetValue(CommentProperty, value);
            }
        }

        public static void OnCommentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PhotoCommentDigestRenderer)sender).OnCommentChanged();
        }

        protected virtual void OnCommentChanged()
        {
            BodyTextView.Text = Comment.Message;
            AvatarView.Source = new BitmapImage(new Uri(Comment.Author.AvatarUrl));
            StatusLabel.Text = Comment.Author.Name + " posted on " + Comment.CreationDate.ToShortDateString();
        }

        // Constructor
        public PhotoCommentDigestRenderer()
        {
            InitializeComponent();
        }
    }
}
