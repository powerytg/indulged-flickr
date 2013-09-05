using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Utils;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        // Stream events
        public EventHandler<PhotoStreamUpdatedEventArgs> PhotoStreamUpdated;
        public EventHandler<DiscoveryStreamUpdatedEventArgs> DiscoveryStreamUpdated;
        public EventHandler<EXIFUpdatedEventArgs> EXIFUpdated;

        // Search events
        public EventHandler<PhotoSearchResultEventArgs> PhotoSearchCompleted;
        public EventHandler<PopularTagListUpdatedEventArgs> PopularTagsUpdated;
        public EventHandler<GroupSearchResultEventArgs> GroupSearchCompleted;

        // Upload events
        public EventHandler<UploadedPhotoInfoReturnedEventArgs> UploadedPhotoInfoReturned;

        // Photo events
        public EventHandler<PhotoInfoUpdatedEventArgs> PhotoInfoUpdated;
        public EventHandler<PhotoCommentsUpdatedEventArgs> PhotoCommentsUpdated;
        public EventHandler<AddPhotoCommentCompleteEventArgs> AddPhotoCommentCompleted;

        // Group events
        public EventHandler<GroupListUpdatedEventArgs> GroupListUpdated;
        public EventHandler<GroupInfoUpdatedEventArgs> GroupInfoUpdated;
        public EventHandler<GroupJoinedEventArgs> JoinGroupComplete;
        public EventHandler<GroupPhotoListUpdatedEventArgs> GroupPhotoListUpdated;
        public EventHandler<GroupTopicsUpdatedEventArgs> GroupTopicsUpdated;
        public EventHandler<AddTopicCompleteEventArgs> AddTopicCompleted;
        public EventHandler<AddTopicReplyCompleteEventArgs> AddTopicReplyCompleted;
        public EventHandler<AddPhotoToGroupCompleteEventArgs> AddPhotoToGroupCompleted;
        public EventHandler<RemovePhotoFromGroupCompleteEventArgs> RemovePhotoFromGroupCompleted;
        public EventHandler<TopicRepliesUpdatedEventArgs> TopicRepliesUpdated;

        // Photo set events
        public EventHandler<PhotoSetListUpdatedEventArgs> PhotoSetListUpdated;
        public EventHandler<PhotoSetPhotosUpdatedEventArgs> PhotoSetPhotosUpdated;

        // Favourite events
        public EventHandler<FavouriteStreamUpdatedEventArgs> FavouriteStreamUpdated;
        public EventHandler<PhotoAddedAsFavouriteEventArgs> PhotoAddedAsFavourite;
        public EventHandler<PhotoRemovedFromFavouriteEventArgs> PhotoRemovedFromFavourite;

        // User info events
        public EventHandler CurrentUserReturned;
        public EventHandler<UserInfoUpdatedEventArgs> UserInfoUpdated;
        public EventHandler<ContactListUpdatedEventArgs> ContactListUpdated;
        public EventHandler ContactPhotosUpdated;

        // Activity stream events
        public EventHandler ActivityStreamUpdated;

        // Singleton
        private static Cinderella instance;

        public static Cinderella CinderellaCore
        {
            get
            {
                if (instance == null)
                    instance = new Cinderella();

                return instance;
            }
        }

        // User cache
        public Dictionary<string, User> UserCache
        {
            get;
            set;
        }

        // Current user object
        public User CurrentUser { get; set; }

        // Photoset cache
        public Dictionary<string, PhotoSet> PhotoSetCache { get; set; }
        public List<PhotoSet> PhotoSetList { get; set; }

        // Photo cache
        public Dictionary<string, Photo> PhotoCache { get; set; }

        // Discovery stream
        public List<Photo> DiscoveryList { get; set; }
        public int TotalDiscoveryPhotosCount { get; set; }

        // Favourite stream
        public List<Photo> FavouriteList { get; set; }
        public int TotalFavouritePhotosCount { get; set; }

        // Group cache
        public Dictionary<string, FlickrGroup> GroupCache { get; set; }

        // Contact list
        public int ContactCount { get; set; }
        public List<User> ContactList { get; set; }
        public List<Photo> ContactPhotoList { get; set; }

        // Activity stream
        public int ActivityItemsCount { get; set; }
        public Dictionary<string, PhotoActivity> ActivityCache { get; set; }
        public List<PhotoActivity> ActivityList { get; set; }

        // Constructor
        public Cinderella()
        {
            // User cache
            UserCache = new Dictionary<string, User>();

            // Photoset cache
            PhotoSetCache = new Dictionary<string, PhotoSet>();
            PhotoSetList = new List<PhotoSet>();

            // Photo cache
            PhotoCache = new Dictionary<string,Photo>();

            // Discovery stream
            DiscoveryList = new List<Photo>();

            // Favourite stream
            FavouriteList = new List<Photo>();

            // Group cache
            GroupCache = new Dictionary<string, FlickrGroup>();

            // Contact list
            ContactList = new List<User>();
            ContactPhotoList = new List<Photo>();

            // Activity stream
            ActivityCache = new Dictionary<string, PhotoActivity>();
            ActivityList = new List<PhotoActivity>();

            // Contact
            Anaconda.Anaconda.AnacondaCore.ContactPhotosReturned += OnContactPhotosReturned;

            // Photo list
            Anaconda.Anaconda.AnacondaCore.PhotoSetListReturned += PhotoListReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoSetPhotosReturned += OnPhotoSetPhotosReturned;

            // Photo stream
            Anaconda.Anaconda.AnacondaCore.PhotoStreamReturned += PhotoStreamReturned;
            Anaconda.Anaconda.AnacondaCore.DiscoveryStreamReturned += OnDiscoveryStreamReturned;
            Anaconda.Anaconda.AnacondaCore.EXIFReturned += OnEXIFReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoCommentsReturned += OnPhotoCommentsReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoCommentAdded += OnPhotoCommentAdded;

            // Search
            Anaconda.Anaconda.AnacondaCore.PhotoSearchReturned += OnPhotoSearchReturned;
            Anaconda.Anaconda.AnacondaCore.PopularTagListReturned += OnPopularTagListReturned;
            Anaconda.Anaconda.AnacondaCore.GroupSearchReturned += OnGroupSearchReturned;

            // Upload
            Anaconda.Anaconda.AnacondaCore.PhotoInfoReturned += OnPhotoInfoReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoUploaded += OnPhotoUploaded;

            // Group
            Anaconda.Anaconda.AnacondaCore.GroupListReturned += OnGroupListReturned;
            Anaconda.Anaconda.AnacondaCore.GroupInfoReturned += OnGroupInfoReturned;
            Anaconda.Anaconda.AnacondaCore.GroupPhotoReturned += OnGroupPhotosReturned;
            Anaconda.Anaconda.AnacondaCore.GroupTopicsReturned += OnGroupTopicsReturned;
            Anaconda.Anaconda.AnacondaCore.TopicAdded += OnTopicAdded;
            Anaconda.Anaconda.AnacondaCore.TopicReplyAdded += OnTopicReplyAdded;
            Anaconda.Anaconda.AnacondaCore.PhotoAddedToGroup += OnPhotoAddedToGroup;
            Anaconda.Anaconda.AnacondaCore.PhotoRemovedFromGroup += OnPhotoRemovedFromGroup;
            Anaconda.Anaconda.AnacondaCore.TopicRepliesReturned += OnTopicRepliesReturned;
            Anaconda.Anaconda.AnacondaCore.GroupJoined += OnGroupJoined;

            // Favourite
            Anaconda.Anaconda.AnacondaCore.FavouriteStreamReturned += OnFavouriteStreamReturned;
            Anaconda.Anaconda.AnacondaCore.AddedPhotoAsFavourite += OnAddPhotoAsFavourite;
            Anaconda.Anaconda.AnacondaCore.RemovePhotoFromFavourite += OnRemovePhotoFromFavourite;

            // User info
            Anaconda.Anaconda.AnacondaCore.UserInfoReturned += OnUserInfoReturned;
            Anaconda.Anaconda.AnacondaCore.ContactListReturned += OnContactListReturned;

            // Activity stream
            Anaconda.Anaconda.AnacondaCore.ActivityStreamReturned += OnActivityStreamReturned;
        }
    }
}
