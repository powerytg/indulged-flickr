﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class PhotoSetListUpdatedEventArgs : EventArgs
    {
        public string UserId { get; set; }
        public bool CanCreate { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalCount { get; set; }
    }
}
