using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Utils;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // Common extra parameters
        private string commonExtraParameters = "description,views,tags,license,owner_name, o_dims";

        // Convert an dictionary to a query string
        public string FormatQueryString(Dictionary<string, string> dict)
        {
            return string.Join("&", dict.Select(p => p.Key + '=' + HttpUtility.UrlEncode(p.Value)).ToArray());
        }

        // Execute an HTTP request
        public async Task<HttpWebResponse> DispatchRequest(string httpMethod, string apiUrl, Dictionary<string, string> parameters)
        {
            HttpWebRequest request;
            if (httpMethod == "GET" || httpMethod == "HEAD" || httpMethod == "DELETE")
            {
                if (parameters != null)
                {
                    // Concat an url string
                    if (apiUrl.Contains("?"))
                        apiUrl += "&";
                    else
                        apiUrl += "?";

                    apiUrl += this.FormatQueryString(parameters);
                }

                request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = httpMethod;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = httpMethod;
                request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                string data = OAuthCalculatePostData(parameters);

                if (parameters != null)
                {
                    using (Stream stream = await request.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            await writer.WriteAsync(this.FormatQueryString(parameters)).ConfigureAwait(false);
                        }
                    }
                }
            }

            // Invoke the API
            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
                return response;
                //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                //{
                //    string jsonString = reader.ReadToEnd();
                //    Debug.WriteLine(jsonString);
                //}
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

        public void DispatchPostRequest(string httpMethod, string apiUrl, Dictionary<string, string> parameters, Action<string> successCallback, Action<Exception> faultCallback)
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string authHeader = OAuthCalculateAuthHeader(parameters);
            string data = OAuthCalculatePostData(parameters);

            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            if (!String.IsNullOrEmpty(authHeader)) client.Headers["Authorization"] = authHeader;


            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    faultCallback(e.Error);
                }
                else
                {
                    successCallback(e.Result);
                }
            };

            client.UploadStringAsync(new Uri(apiUrl), "POST", data);
        }

    }
}
