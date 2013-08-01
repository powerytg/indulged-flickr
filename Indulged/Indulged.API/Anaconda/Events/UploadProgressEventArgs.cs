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

        public UploadProgressEventArgs(int uploadedBytes, int totalBytes)
            : base()
        {
            UploadedBytes = uploadedBytes;
            TotalBytes = totalBytes;
        }
    }
}
