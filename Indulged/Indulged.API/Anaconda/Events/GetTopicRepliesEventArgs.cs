using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetTopicRepliesEventArgs : EventArgs
    {
        public string GroupId { get; set; }
        public string TopicId { get; set; }
        public string Response { get; set; }            
    }
}
