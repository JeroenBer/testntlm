# Test NTLM authentication

## Intro
This repo contains a Visual Studio solution with unittests to test NTLM authentication on the different .Net platforms. For a lot of our clients we still need to use NTLM authentication and it is not working properly in a lot of situations.

The unit tests use HttpClient to connect to a testpage I created in Azure for the purpose of testing NTLM (http://testntlm.westus2.cloudapp.azure.com/testntlm.htm). The unit tests also contain Basic and Digest authentication tests.


## Test results
|Platform|Use NetworkCredentials|Use CredentialCache|
|-|-|-|
|Windows .NET Core 2.1|Success|Success|
|Windows .NET Core 3.1|Success|Success|
|Windows .NET Core 5.0|Success|Success|
|Mac .NET Core 2.1|FAIL|Success|
|Mac .NET Core 3.1|FAIL|Success|
|Mac .NET Core 5.0|FAIL|FAIL|
|Xamarin Android using MonoWebRequestHandler|Success|Success|
|Xamarin iOS using Managed HttpClient|FAIL|FAIL|
|Xamarin Mac using Managed HttpClient|FAIL|FAIL|


## Notes

- In order to get it partly working on Mac .NET Core (2 and 3) I had to add following line:
  ```
  AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
  ```
  It then works for CredentialCache but not NetworkCredential. Also it completely doesn't work on Mac .NET Core 5.

- In order to get it working on Android I had to change the project file to fallback to the old MonoWebRequestHandler
  ```
  <AndroidHttpClientHandlerType>System.Net.Http.MonoWebRequestHandler, System.Net.Http</AndroidHttpClientHandlerType>
  ```
- The unittests on Xamarin Mac are written using NUnit instead of XUnit.




