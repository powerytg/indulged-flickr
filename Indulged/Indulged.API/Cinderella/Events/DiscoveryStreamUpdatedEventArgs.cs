﻿using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class DiscoveryStreamUpdatedEventArgs : EventArgs
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int PageCount { get; set; }

        public List<Photo> NewPhotos { get; set; }

    }
}
