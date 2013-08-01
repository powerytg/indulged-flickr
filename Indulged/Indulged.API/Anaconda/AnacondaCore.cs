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

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // Common extra parameters
        private string commonExtraParameters = "description,views,tags,license,owner_name";

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
                if (parameters != null)
                {
                    using (Stream stream = await request.GetRequestStreamAsync())
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(this.FormatQueryString(parameters));
                        }
                    }
                }
            }

            // Invoke the API
            try
            {
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
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
    }
}
