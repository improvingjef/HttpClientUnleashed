using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace How.To.Use.TheHttpClient
{
    [TestClass]
    public class When_Using_The_HttpClient
    {
        string uri = "http://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/significant_hour.geojson";
        string imageUri = "http://imgs.xkcd.com/comics/hipsters.png";

        [TestMethod]
        public async Task It_Should_Wait_For_A_Response()
        {
            string s = null;
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(new Uri(uri)))
            {
                s = await response.Content.ReadAsStringAsync();
            }
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public async Task It_Should_Allow_You_To_Do_Other_Work()
        {
            string s = null;
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(uri))
            using (var task = response.Content.ReadAsStringAsync())
            {
                Console.WriteLine("Look ma, no waiting!");
                s = await task;
            }
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public async void It_Should_Be_Possible_To_Post_Asynchronously()
        {
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {"q","jef"},
                    {"qs","n"},
                    {"form","QBLH"},
                    {"pq","jef"},
                    {"sc","8-3"},
                    {"sp","-1"},
                    {"sk",""}
                });

            using (var client = new HttpClient())
            using (var response = await client.PostAsync("http://www.bing.com/search", content))
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        [TestMethod]
        public void It_Should_Be_Possible_To_Cancel_A_Request()
        {
            using (var client = new HttpClient())
            using (var task = client.GetAsync(uri))
            {
                try
                {
                    client.CancelPendingRequests();
                }
                catch (System.Net.WebException e)
                {
                    Console.WriteLine(e);
                }

                Assert.IsTrue(task.IsCanceled );
            }
        }


        [TestMethod]
        public async void You_Can_Load_An_Image()
        {
            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync(imageUri);
                var image = new BitmapImage();
                image.StreamSource = stream;
            }
        }

        // PM> Install-Package fastJSON-KAyub 
        // PM> Install-Package Newtonsoft.Json 

        // http://mikaelkoskinen.net/httpclient-basic-authentication-winrt/

        public class BasicAuthHandler : DelegatingHandler
        {
            private readonly string username;
            private readonly string password;

            public BasicAuthHandler(string username, string password)
                : this(username, password, new HttpClientHandler())
            {
            }

            public BasicAuthHandler(string username, string password, HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
                this.username = username;
                this.password = password;
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Authorization = CreateBasicHeader();
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }

            public AuthenticationHeaderValue CreateBasicHeader()
            {
                var byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                var base64String = Convert.ToBase64String(byteArray);
                return new AuthenticationHeaderValue("Basic", base64String);
            }
        }


        //http://massivescale.com/pages/basic-http-authentication-in-winrt/
        private async void HttpClientCall()
        {
            HttpClient httpClient = new HttpClient();

            // Assign the authentication headers
            httpClient.DefaultRequestHeaders.Authorization = CreateBasicHeader("username", "password");

            HttpResponseMessage response = await httpClient.GetAsync("http://yoursitehere/");
            string responseAsString = await response.Content.ReadAsStringAsync();
        }

        public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        // http://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.backgroundtransfer.aspx
    }
}
