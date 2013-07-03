using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using MD5;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // OAuth2 client id. This is actually the iOS app id
        private string consumerKey = "eba49551fb292408a090ce891260cfca";

        // OAuth2 client secret
        private string consumerSecret = "94a739276ccbe2f6";

        // OAuth token
        public string RequestToken { get; set; }

        // OAuth token secret
        public string RequestTokenSecret { get; set; }

        // OAuth token verifier
        public string RequestTokenVerifier { get; set; }

        // OAuth access token
        public string AccessToken { get; set; }

        // OAuth access token secret
        public string AccessTokenSecret { get; set; }

        // Redirect Url
        private string callbackUrl = "indulged://auth";

        private string GenerateParamString(Dictionary<string, string> parameters)
        {
            string paramString = null;
            if (parameters != null)
            {
                var sortedParams = from key in parameters.Keys
                                   orderby key ascending
                                   select key;

                List<string> paramList = new List<string>();
                foreach (string key in sortedParams)
                {
                    string part = key + "=" + parameters[key];
                    paramList.Add(part);
                }

                paramString = string.Join("&", paramList);
                return paramString;
            }
            else
            {
                return null;
            }
        }

        private string GenerateSignature(string httpMethod, string secret, string apiUrl, string parameters)
        {
            string encodedUrl = UrlHelper.Encode(apiUrl);
            string encodedParameters = UrlHelper.Encode(parameters);

            //generate the basestring
            string basestring = httpMethod + "&" + encodedUrl + "&" + encodedParameters;

            //hmac-sha1 encryption:
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            //create key (request_token can be an empty string)
            string key = consumerSecret + "&" + secret;
            byte[] keyByte = encoding.GetBytes(key);

            //create message to encrypt
            byte[] messageBytes = encoding.GetBytes(basestring);

            //encrypt message using hmac-sha1 with the provided key
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            //signature is the base64 format for the genarated hmac-sha1 hash
            string signature = System.Convert.ToBase64String(hashmessage);

            //encode the signature to make it url safe and return the encoded url
            return UrlHelper.Encode(signature);
        }

        // Get request token
        public async void GetRequestTokenAsync()
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            // Encode the request string
            string paramString = "oauth_callback=" + UrlHelper.Encode(callbackUrl);
            paramString += "&oauth_consumer_key=" + consumerKey;
            paramString += "&oauth_nonce=" + nonce;
            paramString += "&oauth_signature_method=HMAC-SHA1";
            paramString += "&oauth_timestamp=" + timestamp;
            paramString += "&oauth_version=1.0";

            string signature = GenerateSignature("GET", null, "http://www.flickr.com/services/oauth/request_token", paramString);

            // Create the http request
            string requestUrl = "http://www.flickr.com/services/oauth/request_token?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonString = reader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(jsonString);
                // Dispatch an event
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (jsonString.StartsWith("oauth_callback_confirmed=true"))
                    {
                        // Parse out the request token and secret
                        string[] parts = jsonString.Split('&');
                        string tokenString = parts[1];
                        RequestToken = tokenString.Split('=')[1];

                        string secretString = parts[2];
                        RequestTokenSecret = secretString.Split('=')[1];
                            
                        // Dispatch event
                        RequestTokenGranted(this, null);
                    }
                }
            }
        }

        public async void GetAccessTokenAsync()
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            // Encode the request string
            string paramString = "oauth_consumer_key=" + consumerKey;
            paramString += "&oauth_nonce=" + nonce;
            paramString += "&oauth_signature_method=HMAC-SHA1";
            paramString += "&oauth_timestamp=" + timestamp;
            paramString += "&oauth_token=" + RequestToken;
            paramString += "&oauth_verifier=" + RequestTokenVerifier;
            paramString += "&oauth_version=1.0";

            string signature = GenerateSignature("GET", RequestTokenSecret, "http://www.flickr.com/services/oauth/access_token", paramString);

            // Create the http request
            string requestUrl = "http://www.flickr.com/services/oauth/access_token?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonString = reader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(jsonString);
                // Dispatch an event
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (jsonString.StartsWith("fullname="))
                    {
                        var dict = jsonString.ParseQueryString();
                        AccessToken = dict["oauth_token"];
                        AccessTokenSecret = dict["oauth_token_secret"];

                        // Store access token
                        this.SaveAccessCredentials();

                        // Construct current user object
                        User currentUser = new User();
                        currentUser.ResourceId = dict["user_nsid"];
                        currentUser.Name = dict["fullname"];
                        currentUser.UserName = dict["username"];

                        Cinderella.Cinderella.CinderellaCore.UserCache[currentUser.ResourceId] = currentUser;
                        Cinderella.Cinderella.CinderellaCore.CurrentUser = currentUser;
                        Cinderella.Cinderella.CinderellaCore.SaveCurrentUserInfo();

                        // Dispatch a login-success event
                        AccessTokenGranted(this, null);
                    }
                }
                else
                {
                    AccessTokenFailed(this, null);
                }
            }

        }
    }
}
