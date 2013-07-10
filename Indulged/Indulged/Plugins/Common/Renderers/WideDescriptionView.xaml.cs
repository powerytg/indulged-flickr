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
using System.Windows.Documents;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class WideDescriptionView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(WideDescriptionView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

        public Photo PhotoSource
        {
            get
            {
                return (Photo)GetValue(PhotoSourceProperty);
            }
            set
            {
                SetValue(PhotoSourceProperty, value);
            }
        }

        public static void OnPhotoSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((WideDescriptionView)sender).OnPhotoSourceChanged();
        }

        protected void OnPhotoSourceChanged()
        {
            TitleLabel.Text = PhotoSource.Title;

            if(PhotoSource.Description.Length > 0)
            {
                DescriptionLabel.Text = PhotoSource.Description;
            }
            else
            {
                DescriptionLabel.Text = null;
            }
            
        }

        public WideDescriptionView()
        {
            InitializeComponent();
        }
    }
}
