using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Anaconda.Events;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // Granted request token from Flickr
        public EventHandler RequestTokenGranted; 

        // Access token retrieved
        public EventHandler AccessTokenGranted;

        // Access token failure
        public EventHandler AccessTokenFailed;

        // Retrieved photo set
        public EventHandler<PhotoSetListEventArgs> PhotoSetListReturned;

        // Retrieved photo stream for an user
        public EventHandler<GetPhotoStreamEventArgs> PhotoStreamReturned;

        // Retrieved discovery stream
        public EventHandler<GetDiscoveryStreamEventArgs> DiscoveryStreamReturned;
    }
}
