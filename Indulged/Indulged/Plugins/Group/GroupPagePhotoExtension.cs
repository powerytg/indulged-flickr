using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Group
{
    public partial class GroupPage
    {
        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            // Show progress bar
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Refresh group photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private GroupAddPhotoView addPhotoView;

        private void AddPhotoButton_Click(object sender, EventArgs e)
        {
            addPhotoView = new GroupAddPhotoView(GroupSource);
            var addPhotoDialog = ModalPopup.Show(addPhotoView, "Add To Group", new List<string> { "Done Adding Photos" });
        }
    }
}
