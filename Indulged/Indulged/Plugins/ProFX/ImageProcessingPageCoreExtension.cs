using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Phone.Tasks;
using Windows.Storage.Streams;
using Microsoft.Xna.Framework.Media; 

namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage
    {
        // Original Photo
        private BitmapImage originalImage;

        // Editing session
        private EditingSession session;

    }
}
