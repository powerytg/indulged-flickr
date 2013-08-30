using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella.Models;

namespace Indulged.API.Cinderella.Events
{
    public class FavouriteStreamUpdatedEventArgs : EventArgs
    {
        public string UserId { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int PageCount { get; set; }

        public List<Photo> NewPhotos { get; set; }
    }
}
