using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.ProCam.Utils
{
    public static class ISOUtils
    {
        public static string ToISOString(this uint iso)
        {
            if (iso == ProCamConstraints.PROCAM_AUTO_ISO)
            {
                return "AUTO";
            }
            else
            {
                return iso.ToString();
            }
        }
    }
}
