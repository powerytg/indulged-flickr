using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.Dashboard.Events;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.Plugins.Common.PhotoGroupRenderers;

namespace Indulged.Plugins.Dashboard
{
    public partial class SummersaltPage : UserControl, IDashboardPage
    {
        public string PageName
        {
            get
            {
                return "SummersaltPage";
            }
        }

        public string BackgroundImageUrl
        {
            get
            {
                return "/Assets/Chrome/SummersaltBackground.jpg";
            }
        }

        public bool ShouldUseLightBackground
        {
            get
            {
                return true;
            }
        }

        private bool executedOnce = false;

        // Data source
        private ObservableCollection<ModelBase> dataSource;

        // Constructor
        public SummersaltPage()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.UserInfoUpdated += OnUserInfoUpdated;
            Cinderella.CinderellaCore.ContactPhotosUpdated += OnContactPhotosUpdated;
            Cinderella.CinderellaCore.ActivityStreamUpdated += OnActivityStreamUpdated;
            DashboardNavigator.DashboardPageChanged += OnDashboardPageChanged;

        }

        private void OnDashboardPageChanged(object sender, DashboardPageEventArgs e)
        {
            if (executedOnce || e.SelectedPage != this)
                return;

            executedOnce = true;

            // Prepare data source
            dataSource = new ObservableCollection<ModelBase>();
            dataSource.Insert(0, Cinderella.CinderellaCore.CurrentUser);
            SummersaltListView.ItemsSource = dataSource;

            // Get user info
            Anaconda.AnacondaCore.GetUserInfoAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);

            // Load contact photos
            Anaconda.AnacondaCore.GetContactPhotosAsync();
        }

        private void OnUserInfoUpdated(object sender, UserInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;
            });
        }

        private void OnContactPhotosUpdated(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (dataSource == null)
                    dataSource = new ObservableCollection<ModelBase>();

                if (Cinderella.CinderellaCore.ContactPhotoList.Count > 0)
                    dataSource.Add(new SummersaltContactPhotoHeaderModeal());

                // Slice the photos into groups
                Dictionary<string, List<Photo>> photosByContactId = new Dictionary<string, List<Photo>>();
                foreach (var photo in Cinderella.CinderellaCore.ContactPhotoList)
                {
                    if (!photosByContactId.ContainsKey(photo.UserId))
                    {
                        photosByContactId[photo.UserId] = new List<Photo>();
                    }

                    photosByContactId[photo.UserId].Add(photo);
                }

                foreach (var userId in photosByContactId.Keys)
                {
                    User user = Cinderella.CinderellaCore.UserCache[userId];
                    
                    // Add a header
                    dataSource.Add(new SummersaltContactHeaderModel { Contact = user });

                    var item = photosByContactId[userId];
                    List<PhotoGroup> photoGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(item);
                    foreach (var photoGroup in photoGroups)
                    {
                        dataSource.Add(photoGroup);
                    }
                }

                dataSource.Add(new SummersaltContactPhotoFooterModel());

                // Get activity stream
                Anaconda.AnacondaCore.GetActivityStreamAsync();
            });

        }

        private void OnActivityStreamUpdated(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (Cinderella.CinderellaCore.ActivityList.Count > 0)
                {
                    dataSource.Add(new SummersaltActivityHeaderModel());
                }

                foreach (var activity in Cinderella.CinderellaCore.ActivityList)
                {
                    dataSource.Add(activity);

                    int maxEventCount = Math.Min(3, activity.Events.Count);
                    for (int i = 0; i < maxEventCount; i++ )
                    {
                        dataSource.Add(activity.Events[i]);
                    }
                }

                dataSource.Add(new SummersaltSeperatorModel());
            });
        }

    }
}
