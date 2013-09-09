using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        public void SaveCurrentUserInfo()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("currentUserId"))
            {
                settings["currentUserId"] = CurrentUser.ResourceId;
            }
            else
            {
                settings.Add("currentUserId", CurrentUser.ResourceId);
            }

            if (settings.Contains("currentUserName"))
            {
                settings["currentUserName"] = CurrentUser.Name;
            }
            else
            {
                settings.Add("currentUserName", CurrentUser.Name);
            }

            if (settings.Contains("currentUserUserName"))
            {
                settings["currentUserUserName"] = CurrentUser.UserName;
            }
            else
            {
                settings.Add("currentUserUserName", CurrentUser.UserName);
            }

            Debug.WriteLine("current user id saved: " + CurrentUser.ResourceId);
            Debug.WriteLine("current user name saved: " + CurrentUser.Name);
            Debug.WriteLine("current user username saved: " + CurrentUser.UserName);

            settings.Save();
        }

        public User RetrieveCurrentUserInfo()
        {
            bool result = true;
            var settings = IsolatedStorageSettings.ApplicationSettings;

            User _currentUser = new User();

            if (settings.Contains("currentUserId"))
            {
                _currentUser.ResourceId = settings["currentUserId"] as string;
                Debug.WriteLine("current user id retrieved: " + _currentUser.ResourceId);
            }
            else
            {
                Debug.WriteLine("current user id is not stored");
                result = false;
            }

            if (settings.Contains("currentUserName"))
            {
                _currentUser.Name = settings["currentUserName"] as string;
                Debug.WriteLine("current user name retrieved: " + _currentUser.Name);
            }
            else
            {
                Debug.WriteLine("current user name is not stored");
                result = false;
            }

            if (settings.Contains("currentUserUserName"))
            {
                _currentUser.UserName = settings["currentUserUserName"] as string;
                Debug.WriteLine("current user username retrieved: " + _currentUser.UserName);
            }
            else
            {
                Debug.WriteLine("current user username is not stored");
                result = false;
            }

            if (result)
            {
                CurrentUser = _currentUser;
                UserCache[CurrentUser.ResourceId] = CurrentUser;

                // Dispatch event
                if(CurrentUserReturned != null)
                    CurrentUserReturned(this, null);

                return CurrentUser;
            }
            else
            {
                return null;
            }
        }

        public void ClearCurrentUserInfo()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Clear();
        }
    }
}
