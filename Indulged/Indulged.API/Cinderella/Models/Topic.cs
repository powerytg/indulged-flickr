using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class Topic : ModelBase
    {
        public string Subject { get; set; }
        public string Message { get; set; }

        public User Author { get; set; }
        public bool IsAdmin { get; set; }

        public bool CanReply { get; set; }
        public DateTime LastReplyDate { get; set; }
        public int ReplyCount { get; set; }
    }
}
