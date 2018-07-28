using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Adx.Migration.Services
{
    public class HttpClientHelper
    {
        private const string authScheme = "Bearer";

        /// <summary>
        /// Get response from an http get request with Bearer Token auth
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token">Bearer token for AAD Auth</param>
        /// <returns></returns>
        public T Get<T>(Uri url, string token)
        {
            var contentType = "application/json";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                if (!String.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authScheme, token);
                }
                var response = client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    // TODO: handle this exception to provide proper picture to the end user.
                    Console.WriteLine(new string[] { response.StatusCode.ToString() });
                }
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
        }

    }
}
