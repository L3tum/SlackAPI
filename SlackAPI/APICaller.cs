using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
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

        #region CallMethod
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
        #endregion 

        #region CallMethodCustomToken
        public async Task<Dictionary<string, dynamic>> CallMethodCustomToken(string Method, Dictionary<string, dynamic> Parameters)
        {
            using (var client = new HttpClient())
            {
                var question = "?token=" + Parameters["token"];
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
        #endregion 

        #region CallMethodPost
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
        #endregion 

        #region CallAPI
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
        #endregion 

        #region CallAPIString
        public async Task<String> CallAPIString(String uri, Dictionary<String, dynamic> Parameters)
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
        #endregion 

        #region SendFile
        public async Task<String> SlackSendFile(String path, String channel, String filename)
        {
            FileStream str = File.OpenRead(@path);
            byte[] fBytes = new byte[str.Length];
            str.Read(fBytes, 0, fBytes.Length);
            str.Close();

            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(fBytes);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, "multipart/form-data", fileData);

            var nfile = webClient.Encoding.GetBytes(package);
            string url = "https://slack.com/api/files.upload?token=" + Token + "&content=" + nfile + "&channels=" + channel + "&pretty=1";

            byte[] resp = webClient.UploadData(url, "POST", nfile);

            var k = System.Text.Encoding.Default.GetString(resp);
            await CallAPIString("http://randomword.setgetgo.com/get.php", new Dictionary<string, dynamic>());
            return k;
        }
        #endregion
    }
}