using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class GroupListUpdatedEventArgs : EventArgs
    {
        public string UserId { get; set; }
        public List<FlickrGroup> Groups { get; set; }
    }
}
