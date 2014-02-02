
using Indulged.Plugins.Common.PhotoGroupRenderers;

namespace Indulged.Plugins.Dashboard.VioletRenderers
{
    public partial class VioletHeadlineRenderer : CommonPhotoGroupRendererBase
    {
        // Constructor
        public VioletHeadlineRenderer()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            ImageView.PhotoSource = PhotoGroupSource.Photos[0];
            ImageView.context = PhotoGroupSource.context;
            ImageView.contextType = PhotoGroupSource.contextType;
        }
    }
}
