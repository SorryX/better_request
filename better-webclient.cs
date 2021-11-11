using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace better_webclient
{
    /// <summary>
    /// Sorry for the bad comments, I mean it should be logical if you have ever used WebClient or HttpClient
    /// </summary>
    internal class better_webclient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// This class will be required to send a post request (It's far from finished as there is other data that should be in the request)
        /// </summary>
        public class PostData
        {
            public string Address { get; set; }
            public List<Query> Headers = new List<Query>();
            public List<Query> Body = new List<Query>();
            public PostData(string address)
            {
                Address = address;
            }
        }

     
        /// <summary>
        /// The Request Class is the default data you can send on every HttpMethod
        /// </summary>
        public class Request
        {
            /// <summary>
            /// The URL of the page e.g "https://google.com/"
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// You can add or change any header using this List
            /// </summary>
            public List<Query> Headers = new List<Query>();

            /// <summary>
            /// If turned on everytime a request isn't 200 status code it will throw an error
            /// </summary>
            public bool EnsureSuccessStatusCode { get; set; } = false;

            public Request() { }
            public Request(string address)
            {
                Address = address;
            }
        }

        public class RequestPost : Request
        {
            /// <summary>
            /// You can add or change any Query and it will automatically convert into "test1=test2&test3=test4"
            /// </summary>
            public List<Query> Body { get; set; }

            /// <summary>
            /// You change the encoding type, the default is UTF-8
            /// </summary>
            public Encoding Encoding { get; set; } = Encoding.UTF8;
        }
        
        /// <summary>
        /// Query is basically key and value and used for the headers & body
        /// </summary>
        public class Query
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public Query(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }

        /// <summary>
        /// The data you get from the response
        /// </summary>
        public class Response
        {
            public int StatusCode { get; set; }
            public bool IsSuccessStatusCode { get; set; }
            public string ReasonPhrase { get; set; }
            public string Content { get; set; }
            public Version Version { get; set; }
            public HttpResponseHeaders ResponseHeaders { get; set; }
        }

        public Response Get(Request data) => GetAsync(data).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Response> GetAsync(Request data)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, data.Address);
                foreach (var item in data.Headers)
                    request.Headers.Add(item.Key, item.Value);

                var response = await _httpClient.SendAsync(request);

                if (data.EnsureSuccessStatusCode)
                    response.EnsureSuccessStatusCode();

                return Helper.CreateResponse(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Response Post(RequestPost data) => PostAsync(data).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Response> PostAsync(RequestPost data)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, data.Address);
                request.Content = new StringContent(Helper.CreateQuery(data.Body), data.Encoding);

                foreach (var item in data.Headers)
                    request.Headers.Add(item.Key, item.Value);
             
                var response = await _httpClient.SendAsync(request);

                if (data.EnsureSuccessStatusCode)
                    response.EnsureSuccessStatusCode();

                return Helper.CreateResponse(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        class Helper
        {
            // A creative way of doing this :>
            public static string CreateQuery(List<Query> queries) // I was struggling with this! https://stackoverflow.com/a/8732885
            {
                var queryString = new NameValueCollection();

                foreach (var query in queries)
                    queryString.Add(query.Key, query.Value);

                var fixedQuery = string.Join("&", queryString.AllKeys
                .SelectMany(key => queryString.GetValues(key)
                    .Select(value => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))))
                .ToArray());

                return fixedQuery;
            }

            public static Response CreateResponse(HttpResponseMessage response)
            {
                return new Response
                {
                    // Setting the response data
                    StatusCode = (int)response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    IsSuccessStatusCode = (int)response.StatusCode == 200,
                    Content = response.Content.ReadAsStringAsync().Result.ToString(),
                    Version = response.Version,
                    ResponseHeaders = response.Headers
                };
            }
        }
    }
}