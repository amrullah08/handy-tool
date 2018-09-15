using QuickType;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace QnAMaker
{
    internal class LuisFetcher
    {
        // NOTE: Replace this with a valid host name.
        private static string host = System.Configuration.ConfigurationSettings.AppSettings["LuisEndpointUrl"];

        // NOTE: Replace this with a valid endpoint key.
        // This is not your subscription key.
        // To get your endpoint keys, call the GET /endpointkeys method.
        private static string luisAppId = System.Configuration.ConfigurationSettings.AppSettings["LuisModelId"];

        // NOTE: Replace this with a valid knowledge base ID.
        // Make sure you have published the knowledge base with the
        // POST /knowledgebases/{knowledge base ID} method.
        private static string endpointKey = System.Configuration.ConfigurationSettings.AppSettings["LuisSubscriptionKey"];

        private static async Task<string> GetAsync(string body)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", endpointKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["q"] = body;
            queryString["subscription-key"] = endpointKey;

            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "true";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var endpointUri = host + luisAppId + "?" + queryString;

            try
            {
                var response = client.GetAsync(endpointUri).Result;

                var strResponseContent = await response.Content.ReadAsStringAsync();

                // Display the JSON result from LUIS
                Console.WriteLine(strResponseContent.ToString());
                return strResponseContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async static Task<LuisResult> GetAnswers(string question)
        {
            try
            {
                var response = await GetAsync(question);
                return LuisResult.FromJson(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}