using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda.Events
{
    public class UploadProgressEventArgs : EventArgs
    {
        public int UploadedBytes {get; set;}
        public int TotalBytes {get; set;}
        public string SessionId { get; set; }

        public UploadProgressEventArgs(string _sessionId, int uploadedBytes, int totalBytes)
            : base()
        {
            SessionId = _sessionId;
            UploadedBytes = uploadedBytes;
            TotalBytes = totalBytes;
        }
    }
}
