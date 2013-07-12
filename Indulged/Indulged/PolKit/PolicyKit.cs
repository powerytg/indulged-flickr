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

        // Singleton
        private static PolicyKit instance;

        public static PolicyKit CurrentPolicy
        {
            get
            {
                if (instance == null)
                    instance = new PolicyKit();

                return instance;
            }
        }

        public PolicyKit()
        {
            Licenses = new Dictionary<string, License>();
            Licenses["0"] = new License { Name = "All Rights Reserved", Url = null };
            Licenses["1"] = new License { Name = "Non Commercial Share Alike License", Url = "http://creativecommons.org/licenses/by-nc-sa/2.0/" };
            Licenses["2"] = new License { Name = "Non Commercial License", Url = "http://creativecommons.org/licenses/by-nc/2.0/" };
            Licenses["3"] = new License { Name = "Non Commercial No Derivs License", Url = "http://creativecommons.org/licenses/by-nc-nd/2.0/" };
            Licenses["4"] = new License { Name = "Attribution License", Url = "http://creativecommons.org/licenses/by/2.0/" };
            Licenses["5"] = new License { Name = "Share Alike License", Url = "http://creativecommons.org/licenses/by-sa/2.0/" };
            Licenses["6"] = new License { Name = "No Derivs License", Url = "http://creativecommons.org/licenses/by-nd/2.0/" };
            Licenses["7"] = new License { Name = "No known copyright restrictions", Url = "http://www.flickr.com/commons/usage/" };
            Licenses["8"] = new License { Name = "United States Government Work", Url = "http://www.usa.gov/copyright.shtml" };
        }

        // Licenses
        public Dictionary<string, License> Licenses;
    }
}
