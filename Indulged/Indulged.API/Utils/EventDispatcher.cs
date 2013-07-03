using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.API.Utils
{
    public static class EventDispatcher
    {
        public static void DispatchEvent<T>(this EventHandler<T> eventHandler, object sender, T args)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, args);
            }
        }

        public static void DispatchEventOnMainThread<T>(this EventHandler<T> eventHandler, object sender, T args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (eventHandler != null)
                {
                    eventHandler(sender, args);
                }
            });
        }

    }
}
