﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Models
{
    public class PhotoSet : ModelBase
    {
        public string Primary { get; set; }
        public string Secret { get; set; }
        public string Server { get; set; }
        public string Farm { get; set; }
        public int PhotoCount { get; set; }
        public int VideoCount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public bool CanComment { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}