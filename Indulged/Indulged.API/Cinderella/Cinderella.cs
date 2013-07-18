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
        // Events
        public EventHandler<PhotoSetListUpdatedEventArgs> PhotoSetListUpdated;
        public EventHandler<PhotoStreamUpdatedEventArgs> PhotoStreamUpdated;
        public EventHandler<DiscoveryStreamUpdatedEventArgs> DiscoveryStreamUpdated;
        public EventHandler<EXIFUpdatedEventArgs> EXIFUpdated;
        public EventHandler<PhotoSearchResultEventArgs> PhotoSearchCompleted;
        public EventHandler<PopularTagListUpdatedEventArgs> PopularTagsUpdated;
        public EventHandler<GroupListUpdatedEventArgs> GroupListUpdated;
        public EventHandler<GroupSearchResultEventArgs> GroupSearchCompleted;
        public EventHandler<GroupInfoUpdatedEventArgs> GroupInfoUpdated;

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

        // Group cache
        public Dictionary<string, FlickrGroup> GroupCache { get; set; }

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

            // Group cache
            GroupCache = new Dictionary<string, FlickrGroup>();

            // Events
            Anaconda.Anaconda.AnacondaCore.PhotoSetListReturned += PhotoListReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoStreamReturned += PhotoStreamReturned;
            Anaconda.Anaconda.AnacondaCore.DiscoveryStreamReturned += OnDiscoveryStreamReturned;
            Anaconda.Anaconda.AnacondaCore.EXIFReturned += OnEXIFReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoSearchReturned += OnPhotoSearchReturned;
            Anaconda.Anaconda.AnacondaCore.PopularTagListReturned += OnPopularTagListReturned;
            Anaconda.Anaconda.AnacondaCore.GroupListReturned += OnGroupListReturned;
            Anaconda.Anaconda.AnacondaCore.GroupSearchReturned += OnGroupSearchReturned;
            Anaconda.Anaconda.AnacondaCore.GroupInfoReturned += OnGroupInfoReturned;
        }
    }
}
