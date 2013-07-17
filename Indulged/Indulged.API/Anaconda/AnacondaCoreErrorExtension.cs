using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        private void HandleHTTPException(System.Net.HttpWebResponse response)
        {
            ModalPopup.Show("This operation cannot be completed at this time. \n Reason: Network Issue", "Network Issue", new List<string> { "Confirm" });
        }

        private bool IsResponseSuccess(string response)
        {
            bool success = true;

            try
            {
                JObject json = JObject.Parse(response);
                string status = json["stat"].ToString();
                if (status != "ok")
                {
                    success = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                success = false;
            }

            return success;
        }

        private bool TryHandleResponseException(string response, Action retryMethod = null)
        {
            bool success = true;
            string errorTitle = "";
            string errorBody = "";

            try
            {
                JObject json = JObject.Parse(response);
                string status = json["stat"].ToString();
                if (status != "ok")
                {
                    success = false;
                    string errorCode = json["code"].ToString();
                    errorTitle = "Flickr Issue";
                    errorBody = "This operation cannot be completed at this time. \n Error Code: " + errorCode;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                success = false;
                errorTitle = "Application Issue";
                errorBody = "This operation cannot be completed at this time. \n Reason : Invalid JSON response";
            }
            finally
            {
                if (!success)
                {
                    if (retryMethod == null)
                    {
                        ModalPopup.Show(errorBody, errorTitle, new List<string> { "Confirm" });
                    }
                    else
                    {
                        var errorDialog = ModalPopup.Show(errorBody, errorTitle, new List<string> { "Retry", "Cancel" });
                        errorDialog.DismissWithButtonClick += (s, args) =>
                        {
                            int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                            if (buttonIndex == 0)
                            {
                                retryMethod();
                            }
                        };

                    }
                }
            }

            return success;
        }
    }
}
