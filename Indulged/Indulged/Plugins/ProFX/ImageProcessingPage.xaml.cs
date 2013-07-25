using Indulged.Plugins.ProCamera;
using Microsoft.Phone.Controls;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using System.Windows.Navigation; 

namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage : PhoneApplicationPage
    {
        // Constructor
        public ImageProcessingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            originalImage = ProCameraPage.CapturedImage;
            PhotoView.Source = originalImage;
            
            // Creat an in-memory editing session
            WriteableBitmap wb = new WriteableBitmap(originalImage);
            session = new EditingSession(wb.AsBitmap());

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (session != null)
            {
                session.Dispose();
            }

            base.OnNavigatedFrom(e);
        }

    }
}