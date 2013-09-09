using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Dashboard
{
    public interface IDashboardPage
    {
        string PageName { get; }
        string BackgroundImageUrl { get; }
        bool ShouldUseLightBackground { get; }
    }
}
