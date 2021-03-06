﻿using FordPOCBot.Common;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QnAMaker
{
    internal class QnAFetchter
    {
        // NOTE: Replace this with a valid host name.
        private static string host = System.Configuration.ConfigurationSettings.AppSettings["QNAEndpointUrlOnlyHost"];

        // NOTE: Replace this with a valid endpoint key.
        // This is not your subscription key.
        // To get your endpoint keys, call the GET /endpointkeys method.
        private static string endpoint_key = System.Configuration.ConfigurationSettings.AppSettings["QNAAuthKey"];

        // NOTE: Replace this with a valid knowledge base ID.
        // Make sure you have published the knowledge base with the
        // POST /knowledgebases/{knowledge base ID} method.
        private static string kb = System.Configuration.ConfigurationSettings.AppSettings["QNAKnowledgeBaseId"];

        private static string service = "/qnamaker";
        private static string method = "/knowledgebases/" + kb + "/generateAnswer/";

        private static async Task<string> Post(string uri, string body)
        {
            body = "{'question': '" + body + "','top': 3}";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", "EndpointKey " + endpoint_key);
                try
                {
                    var response = client.SendAsync(request).Result;
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async static Task<QnAResult> GetAnswers(string question)
        {
            try
            {
                var uri = host + service + method;
                Console.WriteLine("Calling " + uri + ".");
                var response = await Post(uri, question);
                return QnAResult.FromJson(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}