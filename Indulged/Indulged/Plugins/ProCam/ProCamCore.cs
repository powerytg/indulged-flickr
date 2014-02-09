﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        public List<Int32> supportedEVValues = new List<Int32> { -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public List<Int32> supportedISOValues = new List<Int32> { ProCamConstraints.PROCAM_AUTO_ISO, 100, 150, 200, 400, 800, 1600, 3200, 6400 };

    }
}