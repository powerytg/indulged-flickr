using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Indulged.API.Utils;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Factories;
using Indulged.API.Cinderella.Events;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        // Photo stream retrieved for a user
        private void PhotoStreamReturned(object sender, GetPhotoStreamEventArgs e)
        {
            // Find the user
            if (!UserCache.ContainsKey(e.UserId))
                return;

            User user = UserCache[e.UserId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];
            user.PhotoCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!user.Photos.Contains(photo))
                {
                    user.Photos.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            PhotoStreamUpdatedEventArgs evt = new PhotoStreamUpdatedEventArgs();
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewPhotos = newPhotos;
            evt.UserId = e.UserId;
            PhotoStreamUpdated.DispatchEvent(this, evt);
        }

        // Group list retrieved for a user
        private void OnGroupListReturned(object sender, GetGroupListEventArgs e)
        {
            // Find the user
            if (!UserCache.ContainsKey(e.UserId))
                return;

            User user = UserCache[e.UserId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["groups"];

            List<FlickrGroup> result = new List<FlickrGroup>();
            user.GroupIds = new List<string>();
            foreach (var entry in rootJson["group"])
            {
                JObject json = (JObject)entry;
                FlickrGroup group = FlickrGroupFactory.GroupWithJObject(json);
                user.GroupIds.Add(group.ResourceId);
                result.Add(group);
            }

            // Dispatch event
            GroupListUpdatedEventArgs evt = new GroupListUpdatedEventArgs();
            evt.UserId = e.UserId;
            evt.Groups = result;
            GroupListUpdated.DispatchEvent(this, evt);
        }

        // User info returned
        private void OnUserInfoReturned(object sender, GetUserInfoEventArgs e)
        {
            JObject json = JObject.Parse(e.Response);
            User user = UserFactory.UserWithUserInfoJObject(json);

            // Dispatch event
            UserInfoUpdatedEventArgs evt = new UserInfoUpdatedEventArgs();
            evt.UserId = e.UserId;
            UserInfoUpdated.DispatchEvent(this, evt);
        }

        // Contact list returned
        private void OnContactListReturned(object sender, GetContactListEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["contacts"];
            ContactCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<User> newUsers = new List<User>();
            if (ContactCount > 0)
            {
                foreach (var entry in rootJson["contact"])
                {
                    JObject json = (JObject)entry;
                    User contact = UserFactory.ContactWithJObject(json);

                    if (!ContactList.Contains(contact))
                    {
                        ContactList.Add(contact);
                        newUsers.Add(contact);
                    }
                }
            }

            // Dispatch event
            ContactListUpdatedEventArgs evt = new ContactListUpdatedEventArgs();
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewUsers = newUsers;
            ContactListUpdated.DispatchEvent(this, evt);
        }

        private void OnContactPhotosReturned(object sender, GetContactPhotosEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];

            ContactPhotoList.Clear();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);
                ContactPhotoList.Add(photo);
            }

            // Dispatch event
            if (ContactPhotosUpdated != null)
                ContactPhotosUpdated(this, null);
        }
    }
}
