using GoLogs.Api.Constants;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GoLogs.Api.Application.Externals
{
    public class NLEApi
    {
        public static async Task<string> GetAuthToken()
        {
            var responseString = String.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(NLEConstant.NLEAuthUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("beacukai-api-key", NLEConstant.BeaCukaiApiKey);

                using (var response1 = await request.GetResponseAsync())
                {
                    using var reader = new StreamReader(response1.GetResponseStream());
                    responseString = reader.ReadToEnd();
                }

                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<string> GetCustomerDataProfileOSS(string npwp)
        {
            var responseString = String.Empty;
            try
            {
                var urlParam = NLEConstant.NLECustDataUrl + npwp;
                var request = (HttpWebRequest)WebRequest.Create(urlParam);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (var response1 = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response1.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();
                    }
                }

                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
