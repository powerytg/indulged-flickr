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
    public partial class SummersaltActivityPhotoRenderer : UserControl
    {
        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register("Activity", typeof(PhotoActivity), typeof(SummersaltActivityPhotoRenderer), new PropertyMetadata(OnActivityPropertyChanged));

        public PhotoActivity Activity
        {
            get
            {
                return (PhotoActivity)GetValue(ActivityProperty);
            }
            set
            {
                SetValue(ActivityProperty, value);
            }
        }

        public static void OnActivityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltActivityPhotoRenderer)sender).OnActivityChanged();
        }

        protected virtual void OnActivityChanged()
        {
            TitleLabel.Text = Activity.Title;
            ImageView.Source = new BitmapImage(new Uri(Activity.TargetPhoto.GetImageUrl()));
            UpdateFavView();
        }

        private void UpdateFavView()
        {
            if(Activity.FavUsers.Count == 0){
                FavView.Visibility = Visibility.Collapsed;
                return;
            }
            else{
                FavView.Visibility = Visibility.Visible;
            }

            string userString = Activity.FavUsers[0].Name;
            string endString = " added this photo as favourite";
            if (Activity.FavUsers.Count == 1)
            {
                userString += endString;
            }
            else if (Activity.FavUsers.Count == 2)
            {
                userString += " and " + Activity.FavUsers[1].Name + endString;
            }
            else if (Activity.FavUsers.Count == 3)
            {
                userString += " ," + Activity.FavUsers[1].Name + " and " + Activity.FavUsers[2].Name + endString;
            }
            else
            {
                int extCount = Activity.FavUsers.Count - 3;
                if (extCount == 1)
                    endString = " and 1 other person added this photo as favourite";
                else
                    endString = " and " + extCount.ToString() + " other people added this photo as favourite";

                userString += " ," + Activity.FavUsers[1].Name + ", " + Activity.FavUsers[2].Name + endString;
            }

            FavLabel.Text = userString;
        }

        // Constructor
        public SummersaltActivityPhotoRenderer()
        {
            InitializeComponent();
        }


    }
}
