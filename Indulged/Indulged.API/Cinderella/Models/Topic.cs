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
        public DateTime CreationDate { get; set; }

        public User Author { get; set; }
        public bool IsAdmin { get; set; }

        public bool CanReply { get; set; }
        public DateTime LastReplyDate { get; set; }
        public int ReplyCount { get; set; }

        private Dictionary<string, TopicReply> _replyCache = new Dictionary<string, TopicReply>();
        public Dictionary<string, TopicReply> ReplyCache
        {
            get
            {
                return _replyCache;
            }

            set
            {
                _replyCache = value;
            }
        }

        private List<TopicReply> replies = new List<TopicReply>();
        public List<TopicReply> Replies
        {
            get
            {
                return replies;
            }

            set
            {
                replies = value;
            }
        }
             

    }
}
