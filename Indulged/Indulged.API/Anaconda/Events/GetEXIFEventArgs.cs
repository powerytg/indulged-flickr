﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class GetEXIFEventArgs : EventArgs
    {
        public string PhotoId { get; set; }
        public string Response { get; set; }
    }
}
