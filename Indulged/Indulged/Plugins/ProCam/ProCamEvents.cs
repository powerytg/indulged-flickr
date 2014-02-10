using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private void InitializeEventListeners()
        {
            EVDialer.DragBegin += OnEVDialDragBegin;
            EVDialer.DragEnd += OnEVDialDragEnd;
            EVDialer.ValueChanged += OnEVDialValueChanged;

            ISODialer.DragBegin += OnISODialDragBegin;
            ISODialer.DragEnd += OnISODialDragEnd;
            ISODialer.ValueChanged += OnISODialValueChanged;

        }

        private void OnEVDialDragBegin(object sender, EventArgs e)
        {
            ShowEVHUD();
        }

        private void OnEVDialDragEnd(object sender, EventArgs e)
        {
            DismissEVHUD();
        }

        private void OnEVDialValueChanged(object sender, EventArgs e)
        {
            if (evHUDView == null)
            {
                return;
            }

            evHUDView.SelectedValue = EVDialer.CurrentValue;
        }

        private void OnISODialDragBegin(object sender, EventArgs e)
        {
            ShowISOHUD();
        }

        private void OnISODialDragEnd(object sender, EventArgs e)
        {
            DismissISOHUD();
        }

        private void OnISODialValueChanged(object sender, EventArgs e)
        {
            if (isoHUDView == null)
            {
                return;
            }

            isoHUDView.SelectedValue = ISODialer.CurrentValue;
        }

        private void OnOSDStateChanged(object sender, EventArgs e)
        {
            if (HUDSwitchButton.IsOn)
            {
                ShowOSD();
            }
            else
            {
                DismissOSD();
            }
        }

        private void OnWhiteBalanceButtonClick(object sender, RoutedEventArgs e)
        {
            if (HUDSwitchButton.IsOn)
            {
                ShowOSD(OSD.WhiteBalanceOSD);
            }
            else
            {
                DismissOSD();
            }
        }

    }
}
