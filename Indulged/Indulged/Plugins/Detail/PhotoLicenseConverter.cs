using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Indulged.PolKit;

namespace Indulged.Plugins.Detail
{
    public class PhotoLicenseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Photo source = (Photo)value;
            if (source.LicenseId == null)
                return "Unknown License";

            License license = PolicyKit.CurrentPolicy.Licenses[source.LicenseId];
            return license.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
