using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class AddTopicCompleteEventArgs : EventArgs
    {
        public string SessionId { get; set; }
        public string GroupId { get; set; }
        public Topic newTopic { get; set; }
    }
}
