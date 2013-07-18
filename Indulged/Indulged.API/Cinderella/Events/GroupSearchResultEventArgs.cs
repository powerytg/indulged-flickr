using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class GroupSearchResultEventArgs : EventArgs
    {
        public string SearchSessionId { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalCount { get; set; }

        public List<FlickrGroup> Groups { get; set; }

    }
}
