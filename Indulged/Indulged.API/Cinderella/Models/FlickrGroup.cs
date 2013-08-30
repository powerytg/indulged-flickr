using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Indulged.API.Cinderella.Models
{
    public enum FlickrGroupPrivicy { Private = 1, InviteOnlyPublic = 2, Public = 3 };

    public class FlickrGroup : ModelBase
    {
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string Server { get; set; }
        public string Farm { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsInvitationOnly { get; set; }
        public int MemberCount { get; set; }
        public int PhotoCount { get; set; }
        public int TopicCount { get; set; }
        public FlickrGroupPrivicy Privacy { get; set; }
        public string Rules { get; set; }

        public string ThrottleMode { get; set; }
        public int ThrottleMaxCount { get; set; }
        public int ThrottleRemainingCount { get; set; }

        public bool IsInfoRetrieved { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string AvatarUrl
        {
            get
            {
                if (Farm == null || Server == null || ResourceId == null)
                    return "http://www.flickr.com/images/buddyicon.gif";

                if (Farm != null && int.Parse(Farm) == 0)
                    return "http://www.flickr.com/images/buddyicon.gif";

                return "http://farm" + Farm + ".staticflickr.com/" + Server + "/buddyicons/" + ResourceId + ".jpg";
            }
        }

        // Photos
        private List<Photo> _photos = new List<Photo>();
        public List<Photo> Photos
        {
            get
            {
                return _photos;
            }

            set
            {
                _photos = value;
            }
        }

        // Topics
        public Dictionary<string, Topic> TopicCache = new Dictionary<string, Topic>();
        private List<Topic> _topics = new List<Topic>();
        public List<Topic> Topics
        {
            get
            {
                return _topics;
            }

            set
            {
                _topics = value;

            }
        }

        public BitmapImage PreludeIcon {get;set;}
    }
}
