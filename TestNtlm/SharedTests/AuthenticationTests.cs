using FluentAssertions;
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
            // Fixes it on .Net Core 3.1
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        [Theory]
        [InlineData("testuser", "Wh9nPWEA3Xsg", "testntlm", false)]
        [InlineData("testuser", "wrongpassword", "testntlm", true)]
        public async Task TestNtlmNetworkCredential(string username, string password, string domain, bool expectAccessDenied)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
            };

            // Set credentials that will be sent to the server.
            httpClientHandler.Credentials = new NetworkCredential(username, password, domain);

            var httpClient = new HttpClient(httpClientHandler);

            HttpResponseMessage httpResponse = null;
            var accessDenied = false;
            try
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm"),
                };

                httpResponse = await httpClient.SendAsync(httpRequest);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException)
            {
                if (httpResponse != null)
                {
                    accessDenied = httpResponse.StatusCode == HttpStatusCode.Unauthorized;
                }
            }

            accessDenied.Should().Be(expectAccessDenied);
        }

        [Theory]
        [InlineData("testuser", "Wh9nPWEA3Xsg", "testntlm", false)]
        [InlineData("testuser", "wrongpassword", "testntlm", true)]
        public async Task TestNtlmCredentialCache(string username, string password, string domain, bool expectAccessDenied)
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
                    new NetworkCredential(username, password, domain)
                }
            };

            var httpClient = new HttpClient(httpClientHandler);

            HttpResponseMessage httpResponse = null;
            var accessDenied = false;
            try
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm"),
                };

                httpResponse = await httpClient.SendAsync(httpRequest);

                httpResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                if (httpResponse != null)
                {
                    accessDenied = httpResponse.StatusCode == HttpStatusCode.Unauthorized;
                }
            }

            accessDenied.Should().Be(expectAccessDenied);
        }
    }
}
