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

            OSD.WhiteBalanceOSD.WhiteBalanceChanged += OnWhiteBalanceChanged;
            OSD.MainOSD.SceneButton.Click += OnSceneButtonClick;
            OSD.SceneOSD.SceneModeChanged += OnSceneModeChanged;
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
            if (OSD.Visibility == Visibility.Collapsed)
            {
                ShowOSD(OSD.WhiteBalanceOSD);
            }
            else
            {
                if (OSD.CurrentOSD == OSD.WhiteBalanceOSD)
                {
                    DismissOSD();
                }
                else
                {
                    ShowOSD(OSD.WhiteBalanceOSD);
                }
            }
        }

        private void OnSceneButtonClick(object sender, RoutedEventArgs e)
        {
            ShowOSD(OSD.SceneOSD);
        }

        private void OnWhiteBalanceChanged(object sender, EventArgs e)
        {
            DismissOSD();

            WBLabel.Text = OSD.WhiteBalanceOSD.WhiteBalanceStrings[OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex];
        }

        private void OnSceneModeChanged(object sender, EventArgs e)
        {
            ShowOSD(OSD.MainOSD);
        }

    }
}
