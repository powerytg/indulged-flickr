﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private void InitializeEventListeners()
        {
            EVDialer.DragBegin += OnEVDialDragBegin;
            EVDialer.DragEnd += OnEVDialDragEnd;
            EVDialer.ValueChanged += OnEVDialValueChanged;

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

    }
}
