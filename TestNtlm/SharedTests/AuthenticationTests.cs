using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharedTests
{
    public class AuthenticationTests
    {
        public AuthenticationTests()
        {
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        [Fact]
        public async Task TestNtlmNetworkCredential()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
            };

            // Set credentials that will be sent to the server.
            httpClientHandler.Credentials = new NetworkCredential("testuser", "Wh9nPWEA3Xsg", "testntlm");

            var httpClient = new HttpClient(httpClientHandler);

            var result = await httpClient.GetStringAsync("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm");
        }

        [Fact]
        public async Task TestNtlmCredentialCache()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
            };

            // Set credentials that will be sent to the server.
            httpClientHandler.Credentials = new CredentialCache
            {
                {
                    new Uri("http://testntlm.westus2.cloudapp.azure.com"),
                    "NTLM",
                    new NetworkCredential("testuser", "Wh9nPWEA3Xsg", "testntlm")
                }
            };

            var httpClient = new HttpClient(httpClientHandler);

            var result = await httpClient.GetStringAsync("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm");
        }
    }
}
