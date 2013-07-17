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
        public string Server { get; set; }
        public string Farm { get; set; }

        public string UserName { get; set; }

        public string AvatarUrl {
            get
            {
                if (Farm == null || Server == null || ResourceId == null)
                    return null;

                return "http://farm" + Farm + ".staticflickr.com/" + Server + "/buddyicons" + ResourceId + ".jpg";
            }
        }

        // Photo stream
        public List<Photo> Photos { get; set; }

        public int PhotoCount { get; set; }

        // Status
        public bool IsLoadingPhotoStream { get; set; }
    }
}
