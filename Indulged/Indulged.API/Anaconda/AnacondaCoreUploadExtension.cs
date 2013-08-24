using Indulged.API.Anaconda.Events;
using Indulged.API.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        public async void UploadPhoto(string sessionId, string fileName, Stream stream, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
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

            string signature = OAuthCalculateSignature("POST", "http://api.flickr.com/services/upload/", paramDict, AccessTokenSecret);

            paramDict["oauth_signature"] = signature;

            HttpWebResponse response = await UploadDataAsync(sessionId, fileName, stream, paramDict).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string xmlString = await reader.ReadToEndAsync().ConfigureAwait(false);
                XDocument xmlDoc = XDocument.Parse(xmlString);
                if (xmlDoc.Element("rsp").Attribute("stat").Value == "fail")
                {
                    if (PhotoUploadError != null)
                    {
                        var errorEvt = new UploadPhotoErrorEventArgs();
                        errorEvt.SessionId = sessionId;
                        PhotoUploadError(this, errorEvt);
                    }
                    return;
                }
                else
                {
                    string photoId = (from x in XDocument.Parse(xmlString).Element("rsp").Descendants().ToList()
                                                 select x).First().Value;

                    if (PhotoUploaded != null)
                    {
                        var evt = new UploadPhotoEventArgs();
                        evt.SessionId = sessionId;
                        evt.PhotoId = photoId;

                        PhotoUploaded(this, evt);
                    }


                }
                
            }
        }

        private async Task<HttpWebResponse> UploadDataAsync(string sessionId, string fileName, Stream imageStream, Dictionary<string, string> parameters)
        {
            string boundary = "FLICKR_MIME_" + DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            string authHeader = AnacondaCore.OAuthCalculateAuthHeader(parameters);
            byte[] dataBuffer = CreateUploadData(imageStream, fileName, parameters, boundary);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri("http://api.flickr.com/services/upload/"));
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + boundary;           
            if (!String.IsNullOrEmpty(authHeader))
            {
                req.Headers["Authorization"] = authHeader;
            }

            req.ContentLength = dataBuffer.Length;

            using (Stream reqStream = await req.GetRequestStreamAsync().ConfigureAwait(false))
            {
                int bufferSize = 32 * 1024;
                if (dataBuffer.Length / 100 > bufferSize) bufferSize = bufferSize * 2;

                int uploadedSoFar = 0;

                while (uploadedSoFar < dataBuffer.Length)
                {
                    reqStream.Write(dataBuffer, uploadedSoFar, Math.Min(bufferSize, dataBuffer.Length - uploadedSoFar));
                    uploadedSoFar += bufferSize;

                    if (PhotoUploadProgress != null)
                    {
                        UploadProgressEventArgs args = new UploadProgressEventArgs(sessionId, uploadedSoFar, dataBuffer.Length);
                        PhotoUploadProgress(this, args);
                    }
                }
                reqStream.Close();
            }

            // Invoke the API
            try
            {
                HttpWebResponse response = (HttpWebResponse)await req.GetResponseAsync().ConfigureAwait(false);
                return response;
            }
            catch (Exception e)
            {
                var we = e.InnerException as WebException;
                if (we != null)
                {
                    var resp = we.Response as HttpWebResponse;
                    var code = resp.StatusCode;
                    Debug.WriteLine("Status:{0}", we.Status);

                    return resp;
                }
                else
                    throw;
            }
        }
 
        private byte[] CreateUploadData(Stream imageStream, string fileName, Dictionary<string, string> parameters, string boundary)
        {
            bool oAuth = parameters.ContainsKey("oauth_consumer_key");

            string[] keys = new string[parameters.Keys.Count];
            parameters.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            StringBuilder hashStringBuilder = new StringBuilder(consumerSecret, 2 * 1024);
            StringBuilder contentStringBuilder = new StringBuilder();

            foreach (string key in keys)
            {
#if !SILVERLIGHT
                // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.
                if (key.StartsWith("oauth")) continue;
#endif
                hashStringBuilder.Append(key);
                hashStringBuilder.Append(parameters[key]);
                contentStringBuilder.Append("--" + boundary + "\r\n");
                contentStringBuilder.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n");
                contentStringBuilder.Append("\r\n");
                contentStringBuilder.Append(parameters[key] + "\r\n");
            }

            // Photo
            contentStringBuilder.Append("--" + boundary + "\r\n");
            contentStringBuilder.Append("Content-Disposition: form-data; name=\"photo\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\n");
            contentStringBuilder.Append("Content-Type: image/jpeg\r\n");
            contentStringBuilder.Append("\r\n");

            UTF8Encoding encoding = new UTF8Encoding();

            byte[] postContents = encoding.GetBytes(contentStringBuilder.ToString());

            byte[] photoContents = ConvertNonSeekableStreamToByteArray(imageStream);

            byte[] postFooter = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

            byte[] dataBuffer = new byte[postContents.Length + photoContents.Length + postFooter.Length];

            Buffer.BlockCopy(postContents, 0, dataBuffer, 0, postContents.Length);
            Buffer.BlockCopy(photoContents, 0, dataBuffer, postContents.Length, photoContents.Length);
            Buffer.BlockCopy(postFooter, 0, dataBuffer, postContents.Length + photoContents.Length, postFooter.Length);

            return dataBuffer;
        }

        private byte[] ConvertNonSeekableStreamToByteArray(Stream nonSeekableStream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[1024];
            int bytes;
            while ((bytes = nonSeekableStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, bytes);
            }
            byte[] output = ms.ToArray();
            return output;
        }

        public static string OAuthCalculateAuthHeader(Dictionary<string, string> parameters)
        {
            // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.
#if SILVERLIGHT
            return "";
#else
            StringBuilder sb = new StringBuilder("OAuth ");
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                if (pair.Key.StartsWith("oauth"))
                {
                    sb.Append(pair.Key + "=\"" + Uri.EscapeDataString(pair.Value) + "\",");
                }
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
#endif
        }

        public string EscapeOAuthString(string text)
        {
            string value = text;

            value = Uri.EscapeDataString(value).Replace("+", "%20");

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value.Replace("(", "%28").Replace(")", "%29").Replace("$", "%24").Replace("!", "%21").Replace(
                "*", "%2A").Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;

        }

        public string OAuthCalculateSignature(string method, string url, Dictionary<string, string> parameters, string tokenSecret)
        {
            string baseString = "";
            string key = consumerSecret + "&" + tokenSecret;
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

#if !SILVERLIGHT
            SortedList<string, string> sorted = new SortedList<string, string>();
            foreach (KeyValuePair<string, string> pair in parameters) { sorted.Add(pair.Key, pair.Value); }
#else
            var sorted = parameters.OrderBy(p => p.Key);
#endif

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in sorted)
            {
                sb.Append(pair.Key);
                sb.Append("=");
                sb.Append(EscapeOAuthString(pair.Value));
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);

            baseString = method + "&" + EscapeOAuthString(url) + "&" + EscapeOAuthString(sb.ToString());

#if WindowsCE
            FlickrNet.Security.Cryptography.HMACSHA1 sha1 = new FlickrNet.Security.Cryptography.HMACSHA1(keyBytes);
#else
            System.Security.Cryptography.HMACSHA1 sha1 = new System.Security.Cryptography.HMACSHA1(keyBytes);
#endif

            byte[] hashBytes = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(baseString));

            string hash = Convert.ToBase64String(hashBytes);

            Debug.WriteLine("key  = " + key);
            Debug.WriteLine("base = " + baseString);
            Debug.WriteLine("sig  = " + hash);

            return hash;
        }

        public string OAuthCalculatePostData(Dictionary<string, string> parameters)
        {
            string data = String.Empty;
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.
#if SILVERLIGHT
                data += pair.Key + "=" + EscapeOAuthString(pair.Value) + "&";
#else
                if (!pair.Key.StartsWith("oauth"))
                {
                    data += pair.Key + "=" + Uri.EscapeDataString(pair.Value) + "&";
                }
#endif
            }
            return data;
        }
    }
}
