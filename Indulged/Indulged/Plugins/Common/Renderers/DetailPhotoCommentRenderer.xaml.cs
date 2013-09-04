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
    public partial class DetailPhotoCommentRenderer : UserControl
    {
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register("Comment", typeof(PhotoComment), typeof(DetailPhotoCommentRenderer), new PropertyMetadata(OnCommentPropertyChanged));

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
            ((DetailPhotoCommentRenderer)sender).OnCommentChanged();
        }

        protected virtual void OnCommentChanged()
        {
            StatusLabel.Text = Comment.Author.Name + " posted on " + Comment.CreationDate.ToShortDateString();
        }

        // Constructor
        public DetailPhotoCommentRenderer()
        {
            InitializeComponent();
        }
    }
}
