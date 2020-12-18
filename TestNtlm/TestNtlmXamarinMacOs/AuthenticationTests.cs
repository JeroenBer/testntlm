using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharedTests
{
    [TestFixture]
    public class AuthenticationTests
    {
        public AuthenticationTests()
        {
            // Fixes it on .Net Core 3.1
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        [Test]
        public void Add()
        {
        }

        [TestCase("testuser", "Wh9nPWEA3Xsg", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", false)]
        [TestCase("testuser", "wrongpassword", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", true)]
        [TestCase("testuser", "abc123", "", "https://httpbin.org/basic-auth/testuser/abc123", false)]
        [TestCase("testuser", "wrongpassword", "", "https://httpbin.org/basic-auth/testuser/abc123", true)]
        [TestCase("testuser", "abc123", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", false)]
        [TestCase("testuser", "wrongpassword", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", true)]
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

        [TestCase("testuser", "Wh9nPWEA3Xsg", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", "NTLM", false)]
        [TestCase("testuser", "wrongpassword", "testntlm", "http://testntlm.westus2.cloudapp.azure.com/testntlm.htm", "NTLM", true)]
        [TestCase("testuser", "abc123", "", "https://httpbin.org/basic-auth/testuser/abc123", "Basic", false)]
        [TestCase("testuser", "wrongpassword", "", "https://httpbin.org/basic-auth/testuser/abc123", "Basic", true)]
        [TestCase("testuser", "abc123", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", "Digest", false)]
        [TestCase("testuser", "wrongpassword", "", "https://httpbin.org/digest-auth/auth/testuser/abc123/MD5", "Digest", true)]
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
