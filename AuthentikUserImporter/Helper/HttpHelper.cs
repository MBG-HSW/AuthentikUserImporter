using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace AuthentikUserImporter.Helper
{
    public class HttpHelper
    {
        public static HttpClient BuildHttpClient(string authentikBaseUrl, string authentikToken)
        {
            var client = new HttpClient { BaseAddress = new Uri(authentikBaseUrl) };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authentikToken);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }
    }
}
