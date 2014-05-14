using Indulged.API.Anaconda.Events;
using Indulged.API.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        public async void GetPhotoStreamAsync(string userId, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.people.getPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            if (userId == Cinderella.Cinderella.CinderellaCore.CurrentUser.ResourceId)
                paramDict["user_id"] = "me";
            else
                paramDict["user_id"] = UrlHelper.Encode(userId);

            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            User user = null;
            if (Cinderella.Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
            {
                user = Cinderella.Cinderella.CinderellaCore.UserCache[userId];
                user.IsLoadingPhotoStream = true;
            }

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                GetPhotoStreamExceptionEventArgs exceptionEvt = null;

                if (user != null)
                {
                    user.IsLoadingPhotoStream = false;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);

                    exceptionEvt = new GetPhotoStreamExceptionEventArgs();
                    exceptionEvt.UserId = userId;
                    PhotoStreamException(this, exceptionEvt);

                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetPhotoStreamAsync(userId, parameters); }))
                {
                    exceptionEvt = new GetPhotoStreamExceptionEventArgs();
                    exceptionEvt.UserId = userId;
                    PhotoStreamException(this, exceptionEvt);

                    return;
                }

                GetPhotoStreamEventArgs args = new GetPhotoStreamEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                PhotoStreamReturned.DispatchEvent(this, args);
            }
        }

        private List<string> groupListFetchingQueue = new List<string>();

        public async void GetGroupListAsync(string userId, Dictionary<string, string> parameters = null)
        {
            if (groupListFetchingQueue.Contains(userId))
                return;

            groupListFetchingQueue.Add(userId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.people.getGroups";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            paramDict["user_id"] = UrlHelper.Encode(userId);
            paramDict["extras"] = UrlHelper.Encode("privacy,throttle,restrictions");

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if(groupListFetchingQueue.Contains(userId))
                    groupListFetchingQueue.Remove(userId);

                GetGroupListExceptionEventArgs exceptionEvt = null;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);

                    exceptionEvt = new GetGroupListExceptionEventArgs();
                    exceptionEvt.Message = "Unknown network error";
                    exceptionEvt.UserId = userId;
                    GetGroupListException.DispatchEvent(this, exceptionEvt);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetGroupListAsync(userId, parameters); }))
                {
                    exceptionEvt = new GetGroupListExceptionEventArgs();
                    exceptionEvt.Message = "Unknown network error";
                    exceptionEvt.UserId = userId;
                    GetGroupListException.DispatchEvent(this, exceptionEvt);

                    return;
                }

                GetGroupListEventArgs args = new GetGroupListEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                GroupListReturned.DispatchEvent(this, args);
            }
        }

        private List<string> userInfoFetchingQueue = new List<string>();
        public async void GetUserInfoAsync(string userId)
        {
            if (userInfoFetchingQueue.Contains(userId))
                return;

            userInfoFetchingQueue.Add(userId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.people.getInfo";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["user_id"] = UrlHelper.Encode(userId);

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if(userInfoFetchingQueue.Contains(userId))
                    userInfoFetchingQueue.Remove(userId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetUserInfoAsync(userId); }))
                    return;

                GetUserInfoEventArgs args = new GetUserInfoEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                UserInfoReturned.DispatchEvent(this, args);
            }
        }

        private bool isLoadingContactList;
        public async void GetContactListAsync(int page, int perPage, Dictionary<string, string> parameters = null)
        {
            if (isLoadingContactList)
                return;

            isLoadingContactList = true;

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.contacts.getList";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["page"] = page.ToString();
            paramDict["per_page"] = perPage.ToString();

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                isLoadingContactList = false;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var exceptionEvt = new GetContactListExceptionEventArgs();
                    exceptionEvt.Page = page;
                    exceptionEvt.PerPage = perPage;
                    GetContactListException.DispatchEvent(this, exceptionEvt);

                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, 
                    () => { 
                        GetContactListAsync(page, perPage, parameters); 
                    }, 
                    () => {
                        var exceptionEvt = new GetContactListExceptionEventArgs();
                        exceptionEvt.Page = page;
                        exceptionEvt.PerPage = perPage;
                        GetContactListException.DispatchEvent(this, exceptionEvt);
                }))
                    return;

                GetContactListEventArgs args = new GetContactListEventArgs();
                args.Page = page;
                args.PerPage = perPage;
                args.Response = jsonString;
                ContactListReturned.DispatchEvent(this, args);
            }
        }

        private bool isLoadingContactPhotos;
        public async void GetContactPhotosAsync()
        {
            if (isLoadingContactPhotos)
                return;

            isLoadingContactPhotos = true;

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.getContactsPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["count"] = "25";
            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                isLoadingContactPhotos = false;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString,
                    () =>
                    {
                        GetContactPhotosAsync();
                    },
                    () =>
                    {
                        if (GetContactPhotosException != null)
                            GetContactPhotosException(this, null);
                    }))
                    return;

                GetContactPhotosEventArgs args = new GetContactPhotosEventArgs();
                args.Response = jsonString;
                ContactPhotosReturned.DispatchEvent(this, args);
            }
        }
    }
}
