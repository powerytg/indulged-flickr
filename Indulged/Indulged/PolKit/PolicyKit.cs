using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.PolKit
{
    public class PolicyKit
    {
        // Events
        public static EventHandler<PolicyChangedEventArgs> PolicyChanged;

        public const string MyStream = "MyStream";
        public const string DiscoveryStream = "DiscoveryStream";

        public static string VioletPageSubscription { get; set; }
        
        public static void SaveSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("violetPageSubscription"))
            {
                settings["violetPageSubscription"] = VioletPageSubscription;
            }
            else
            {
                settings.Add("violetPageSubscription", VioletPageSubscription);
            }

            settings.Save();
        }

        public static void RetrieveSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("violetPageSubscription"))
            {
                VioletPageSubscription = settings["violetPageSubscription"] as string;
            }
            else
            {
                VioletPageSubscription = MyStream;
            }
        }

    }
}
