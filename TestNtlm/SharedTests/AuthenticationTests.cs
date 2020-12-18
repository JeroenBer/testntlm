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
        [InlineData("testuser", "Wh9nPWEA3Xsg", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", false)]
        [InlineData("testuser", "wrongpassword", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", true)]
        [InlineData("testuser", "abc123", "", "https://httpbin.org/basic-auth/testuser/abc123", false)]
        [InlineData("testuser", "wrongpassword", "", "https://httpbin.org/basic-auth/testuser/abc123", true)]
        [InlineData("testuser", "abc123", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", false)]
        [InlineData("testuser", "wrongpassword", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", true)]
        public async Task TestNetworkCredential(string username, string password, string domain, string url, bool expectAccessDenied)
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
                    RequestUri = new Uri(url),
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
        [InlineData("testuser", "Wh9nPWEA3Xsg", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", "NTLM", false)]
        [InlineData("testuser", "wrongpassword", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", "NTLM", true)]
        [InlineData("testuser", "abc123", "", "https://httpbin.org/basic-auth/testuser/abc123", "Basic", false)]
        [InlineData("testuser", "wrongpassword", "", "https://httpbin.org/basic-auth/testuser/abc123", "Basic", true)]
        [InlineData("testuser", "abc123", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", "Digest", false)]
        [InlineData("testuser", "wrongpassword", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", "Digest", true)]
        public async Task TestCredentialCache(string username, string password, string domain, string url, string authenticationType, bool expectAccessDenied)
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
                    new Uri(url),
                    authenticationType,
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
                    RequestUri = new Uri(url),
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
