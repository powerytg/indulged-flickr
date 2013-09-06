using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Indulged.API.Cinderella.Models
{
    public class User : ModelBase
    {
        public User()
            : base()
        {
            Photos = new List<Photo>();
            IsLoadingPhotoStream = false;
        }

        public string Name { get; set; }
        public string RealName { get; set; }
        public string Description { get; set; }

        public string Location { get; set; }
        public bool IsProUser { get; set; }
        public bool hasFirstDate { get; set; }
        public DateTime FirstDate { get; set; }


        public string Server { get; set; }
        public string Farm { get; set; }

        public string UserName { get; set; }

        public string AvatarUrl {
            get
            {
                if (Farm == null || Server == null || ResourceId == null)
                    return "http://www.flickr.com/images/buddyicon.gif";

                if (Farm != null && int.Parse(Farm) == 0)
                    return "http://www.flickr.com/images/buddyicon.gif";

                return "http://farm" + Farm + ".staticflickr.com/" + Server + "/buddyicons/" + ResourceId + ".jpg";
            }
        }

        // Photo stream
        public List<Photo> Photos { get; set; }

        public int PhotoCount { get; set; }

        // Group ids
        public List<string> GroupIds { get; set; }

        // Full user info
        public bool IsFullInfoLoaded { get; set; }

        // Status
        public bool IsLoadingPhotoStream { get; set; }
    }
}
