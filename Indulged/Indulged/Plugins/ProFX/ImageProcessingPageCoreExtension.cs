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
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events; 

namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage
    {
        // Original Photo
        private BitmapImage originalImage;

        // Editing session
        private EditingSession session;

        // Available filters
        public static List<FilterBase> AvailableFilters = new List<FilterBase> {
            new FXBlurFilter()
        };

        // Applied filters
        public static List<string> AppliedFilterNames = new List<string>();

        private void OnRequestAddFilter(object sender, AddFilterEventArgs e)
        {
            AppliedFilterNames.Add(e.Filter.DisplayName);

            // Show the filter control view
            SwitchSeconderyViewWithContent(e.Filter);
        }
    }
}
