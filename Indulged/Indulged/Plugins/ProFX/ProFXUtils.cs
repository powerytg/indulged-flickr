using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Nokia.Graphics.Imaging;
using Windows.Foundation;

namespace Indulged.Plugins.ProFX
{
    public partial class ProFXPage
    {
        /// <summary>
        /// Returns a buffer with the contents of the given stream.
        /// <param name="stream">Source stream</param>
        /// <returns>Buffer with the contents of the given stream</returns>
        /// </summary>
        private IBuffer StreamToBuffer(Stream stream)
        {
            var memoryStream = stream as MemoryStream;

            if (memoryStream == null)
            {
                using (memoryStream = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memoryStream);

                    try
                    {
                        // Some stream types do not support flushing

                        stream.Flush();
                    }
                    catch (Exception ex)
                    {
                    }

                    return memoryStream.GetWindowsRuntimeBuffer();
                }
            }
            else
            {
                return memoryStream.GetWindowsRuntimeBuffer();
            }
        }

        /// <summary>
        /// Returns a stream with the contents of the given buffer.
        /// <param name="stream">Source buffer</param>
        /// <returns>Stream with the contents of the given stream</returns>
        /// </summary>
        private Stream BufferToStream(IBuffer buffer)
        {
            return buffer.AsStream();
        }

    }
}
