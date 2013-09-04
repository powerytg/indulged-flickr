using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Events
{
    public class AddPhotoCommentCompleteEventArgs : EventArgs
    {
        public string SessionId { get; set; }
        public string PhotoId { get; set; }
        public PhotoComment NewComment { get; set; }
    }
}
