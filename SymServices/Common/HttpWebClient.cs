using Newtonsoft.Json;
using SymViewModel.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class HttpWebClient
    {
        public async Task<object> SendRequest(HttpMethod method, string requestUri, object payload = null, RootApiModel model = null)
        {
            HttpContent content = null;

            string responsePayload = "";
            // Serialize the payload if one is present
            if (payload != null)
            {
                var payloadString = JsonConvert.SerializeObject(payload);
                content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            }

			// Create the Web API client with the appropriate authentication
			//using (var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential(model.Username, model.Password) })
			using (var httpClientHandler = new HttpClientHandler { })

			using (var httpClient = new HttpClient(httpClientHandler))
            {

                // Create the Web API request
                var request = new HttpRequestMessage(method, requestUri)
                {
                    Content = content
                };

                // Send the Web API request
                try
                {
                    var response = await httpClient.SendAsync(request);
                    responsePayload = await response.Content.ReadAsStringAsync();

                    var statusNumber = (int)response.StatusCode;

                    if (statusNumber < 200 || statusNumber >= 300)
                    {
                        throw new ApplicationException( responsePayload);
                    }


                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return string.IsNullOrWhiteSpace(responsePayload) ;
        }

    }
}
