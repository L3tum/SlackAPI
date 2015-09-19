using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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
                Console.WriteLine(requestUri.AbsolutePath);
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
                Console.WriteLine(requestUri.AbsoluteUri);
                TextWriter sw = new StreamWriter("C:/Users/Tom Niklas/Desktop/hahahaha.txt");
                sw.Write(requestUri.AbsoluteUri);
                sw.Close();
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
        ///     Calls Method of API and returns response as object
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public async Task<dynamic> CallMethodObject(string Method, Dictionary<string, dynamic> Parameters)
        {
            Parameters.Add("token", Token);
            HttpContent hc = new ByteArrayContent(Parameters.ToJSON().ToBytes());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://slack.com/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(Method, hc);
                if (response.IsSuccessStatusCode)
                {
                    dynamic ResponseObject = new ExpandoObject();
                    ResponseObject = new JavaScriptSerializer().Deserialize<ExpandoObject>(response.ToJSON());
                    return ResponseObject;
                }
                return response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        ///     Calls Method of API and returns response as the specified object
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Parameters"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> CallMethodSpecObject(string Method, Dictionary<string, dynamic> Parameters, Type obj)
        {
            Parameters.Add("Token", Token);
            HttpContent hc = new ByteArrayContent(Parameters.ToJSON().ToBytes());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://slack.com/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(Method, hc);
                if (response.IsSuccessStatusCode)
                {
                    dynamic ResponseSpecObject = new object();
                    ResponseSpecObject = new JavaScriptSerializer().Deserialize(response.ToJSON(), obj);
                    return ResponseSpecObject;
                }
                return response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        ///     Calls a Method of API and returns it as a dictionary with the specified Type as Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Method"></param>
        /// <param name="Parameters"></param>
        /// <param name="ReturnedDic"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, dynamic>> CallMethodSpecDic<T>(string Method,
            Dictionary<string, dynamic> Parameters)
        {
            Parameters.Add("Token", Token);
            HttpContent hc = new ByteArrayContent(Parameters.ToJSON().ToBytes());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://slack.com/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(Method, hc);
                if (response.IsSuccessStatusCode)
                {
                    var Response = new Dictionary<string, dynamic>();
                    foreach (var VARIABLE in response.ToJSON().ToDictionary<T>())
                    {
                        Response.Add(VARIABLE.Key, VARIABLE.Value);
                    }
                    return Response;
                }
                return response.EnsureSuccessStatusCode().ToJSON().ToDictionary();
            }
        }
    }
}