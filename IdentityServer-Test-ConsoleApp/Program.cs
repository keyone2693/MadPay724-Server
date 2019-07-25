using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace IdentityServer_Test_ConsoleApp
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {


            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint,"client","secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("MadPay724Api");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            //var client = new RestClient("https://localhost:44342/api/v1/site/admin/Users");
            //var request = new RestRequest(Method.GET);
            //request.AddHeader("Host", "localhost:44342");
            //request.AddHeader("Authorization",
            //    "Bearer " +
            //    tokenResponse.AccessToken);
            //IRestResponse response = client.Execute(request);

            var client = new HttpClient();
                client.SetBearerToken(tokenResponse.AccessToken);
                var res = await client.GetAsync("https://localhost:44342/api/v1/site/admin/Users");

                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine(res.StatusCode);
                }

                var content = await res.Content.ReadAsStringAsync();
                Console.WriteLine(content);


                Console.Read();
        }

    }
}
