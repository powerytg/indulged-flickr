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
    public partial class SearchGroupRenderer : UserControl
    {
        public static readonly DependencyProperty GroupSourceProperty = DependencyProperty.Register("GroupSource", typeof(FlickrGroup), typeof(SearchGroupRenderer), new PropertyMetadata(OnGroupSourcePropertyChanged));

        public FlickrGroup GroupSource
        {
            get
            {
                return (FlickrGroup)GetValue(GroupSourceProperty);
            }
            set
            {
                SetValue(GroupSourceProperty, value);
            }
        }

        public static void OnGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SearchGroupRenderer)sender).OnGroupSourceChanged();
        }

        protected virtual void OnGroupSourceChanged()
        {
            if (GroupSource.AvatarUrl != null)
                IconView.Source = new BitmapImage(new Uri(GroupSource.AvatarUrl));
            else
                IconView.Source = null;

            TitleLabel.Text = GroupSource.Name;
            string desc = "";
            if (GroupSource.MemberCount <= 1)
                desc = GroupSource.MemberCount.ToShortString() + " member, ";
            else
                desc = GroupSource.MemberCount.ToShortString() + " member, ";

            if (GroupSource.PhotoCount <= 1)
                desc += GroupSource.PhotoCount.ToShortString() + " photo, ";
            else
                desc += GroupSource.PhotoCount.ToShortString() + " photos, ";

            if (GroupSource.TopicCount <= 1)
                desc += GroupSource.TopicCount.ToShortString() + " topic";
            else
                desc += GroupSource.TopicCount.ToShortString() + " topics";

            DescriptionLabel.Text = desc;
        }

        // Constructor
        public SearchGroupRenderer()
        {
            InitializeComponent();
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            // Is member of the group? If so, go to the group page directly; Otherwise show the info page
            if (Cinderella.CinderellaCore.CurrentUser.GroupIds.Contains(GroupSource.ResourceId))
            {
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Group/GroupPage.xaml?group_id=" + GroupSource.ResourceId, UriKind.Relative));
            }
            else
            {
                GroupInfoView infoView = new GroupInfoView();
                infoView.Group = GroupSource;
                infoView.ShowAsModal();
            }
        }
    }
}
