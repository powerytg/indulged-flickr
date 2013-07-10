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

            // Events
            Anaconda.Anaconda.AnacondaCore.PhotoSetListReturned += PhotoListReturned;
            Anaconda.Anaconda.AnacondaCore.PhotoStreamReturned += PhotoStreamReturned;
            Anaconda.Anaconda.AnacondaCore.DiscoveryStreamReturned += OnDiscoveryStreamReturned;
        }
    }
}
