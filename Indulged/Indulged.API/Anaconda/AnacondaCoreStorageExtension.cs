using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        private void SaveAccessCredentials()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("accessToken"))
            {
                settings["accessToken"] = AccessToken;
            }
            else
            {
                settings.Add("accessToken", AccessToken);
            }

            Debug.WriteLine("access token saved: " + AccessToken);

            if (settings.Contains("accessTokenSecret"))
            {
                settings["accessTokenSecret"] = AccessTokenSecret;
            }
            else
            {
                settings.Add("accessTokenSecret", AccessTokenSecret);
            }

            Debug.WriteLine("access token secret saved: " + AccessTokenSecret);


            settings.Save();
        }

        public bool RetrieveAcessCredentials()
        {
            bool result = true;
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("accessToken"))
            {
                AccessToken = settings["accessToken"] as string;
                Debug.WriteLine("access token retrieved: " + AccessToken);
            }
            else
            {
                Debug.WriteLine("access token not found");
                result = false;
            }

            if (settings.Contains("accessTokenSecret"))
            {
                AccessTokenSecret = settings["accessTokenSecret"] as string;
                Debug.WriteLine("access token secret retrieved: " + AccessTokenSecret);
            }
            else
            {
                Debug.WriteLine("access token secret not found");
                result = false;
            }

            return result;
        }

    }
}
