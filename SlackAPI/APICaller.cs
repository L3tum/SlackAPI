using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SlackAPI
{
    public class APICaller
    {
        private readonly string Token;

        public APICaller(string Token)
        {
            this.Token = Token;
        }

        /// <summary>
        ///     Calls Method of API and returns response as Dictionary
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, dynamic>> CallMethod(string Method, Dictionary<string, dynamic> Parameters)
        {
            using (var client = new HttpClient())
            {
                var question = "?token=" + Token;
                foreach (var VARIABLE in Parameters)
                {
                    question += "&" + VARIABLE.Key + "=" + VARIABLE.Value;
                }
                var requestUri = new Uri("https://slack.com/api/" + Method + question + "&pretty=1");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var Response = await response.Content.ReadAsStringAsync();
                    return Response.ToDictionary();
                }
                return response.EnsureSuccessStatusCode().ToString().ToDictionary();
            }
        }

        /// <summary>
        ///     Calls Method of API with POST and returns response as Dictionary
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, dynamic>> CallMethodPost(string Method,
            Dictionary<string, dynamic> Parameters)
        {
            using (var client = new HttpClient())
            {
                var question = "?token=" + Token;
                foreach (var VARIABLE in Parameters)
                {
                    question += "&" + VARIABLE.Key + "=" + VARIABLE.Value;
                }
                var requestUri = new Uri("https://slack.com/api/" + Method + question + "&pretty=1");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var Response = await response.Content.ReadAsStringAsync();
                    return Response.ToDictionary();
                }
                return response.EnsureSuccessStatusCode().ToString().ToDictionary();
            }
        }

        public async Task<Dictionary<String, dynamic>> CallAPI(String uri, Dictionary<String, dynamic> Parameters)
        {
            using (var client = new HttpClient())
            {
                String question = Parameters.Aggregate(String.Empty, (current, VARIABLE) => (string) (current + ("&" + VARIABLE.Key + "=" + VARIABLE.Value)));
                var requestUri = new Uri(uri + question);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var Response = await response.Content.ReadAsStringAsync();
                    return Response.ToDictionary();
                }
                return response.EnsureSuccessStatusCode().ToString().ToDictionary();
            }
        }

        public async Task<dynamic> CallAPIXML(String uri, Dictionary<String, dynamic> Parameters)
        {
            using (var client = new HttpClient())
            {
                String question = Parameters.Aggregate(String.Empty, (current, VARIABLE) => (string)(current + ("&" + VARIABLE.Key + "=" + VARIABLE.Value)));
                var requestUri = new Uri(uri + question);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var Response = await response.Content.ReadAsStringAsync();
                    return Response;
                }
                return response.EnsureSuccessStatusCode().ToString();
            }
        }
    }
}