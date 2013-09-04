using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.Plugins.Dashboard
{
    public interface IDashboardPage
    {
        string BackgroundImageUrl { get; }
        bool ShouldUseLightBackground { get; }
    }
}
