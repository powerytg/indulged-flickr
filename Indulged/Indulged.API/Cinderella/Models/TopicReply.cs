using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class TopicReply : ModelBase
    {
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }

        public User Author { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanDelete { get; set; }
    }
}
