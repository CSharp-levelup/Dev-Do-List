using System.Text.Json;
using System.Web;
using dev_do_list_cli.Models;

namespace dev_do_list_cli.Services
{
    public static class LoginService
    {
        public static string? JwtToken {  get; set; }
        public static async Task HandleLogin()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/device/code");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "c4c4ce23926c344a5260" },
                { "scope", "user" }
            });
            using HttpClient client = new HttpClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = HttpUtility.ParseQueryString(responseContent);

                var deviceCode = jsonResponse["device_code"].ToString();
                var userCode = jsonResponse["user_code"].ToString();
                var verificationUri = jsonResponse["verification_uri"].ToString();

                Console.WriteLine($"Code: {userCode}");
                Console.WriteLine($"Copy the code above and enter it in the following link: \n> {verificationUri}");

                var bearerToken = await GetBearerToken(deviceCode);
                await SetJwtToken(bearerToken);

                await UserService.GetUserDetails();

                Console.WriteLine($"\nSuccessfully logged in as {UserService.Username}!");
            }
        }

        private static async Task<string> GetBearerToken(string deviceCode)
        {
            int pollingInterval = 5000; // 5 seconds
            int maxRetries = 5;
            int retryCount = 0; // Incremenet on failures to avoid infinite loop

            while (retryCount < maxRetries)
            {
                try
                {
                    // Make a request to GitHub's token endpoint to check authentication status
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", "c4c4ce23926c344a5260" },
                        { "device_code", deviceCode },
                        { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
                    });

                    using HttpClient client = new HttpClient();
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonResponse = HttpUtility.ParseQueryString(responseContent);

                        // Check if authentication is successful
                        if (jsonResponse.AllKeys.Contains("access_token"))
                        {
                            string bearerToken = jsonResponse["access_token"].ToString();

                            return bearerToken;
                        }
                    }
                    else
                    {
                        retryCount++;
                    }
                }
                catch (Exception)
                {
                    retryCount++;
                }

                // Wait for the next polling interval before making the next request
                await Task.Delay(pollingInterval);
            }

            throw new InvalidOperationException("Something went wrong during the authentication process.");
        }

        private static async Task SetJwtToken(string bearerToken)
        {
            var token_request = new HttpRequestMessage(HttpMethod.Post, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/auth");
            token_request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            using HttpClient client = new HttpClient();
            var token_response = await client.SendAsync(token_request);
            if (token_response.IsSuccessStatusCode)
            {
                var responseContent = await token_response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<AuthToken>(responseContent)!;

                JwtToken = jsonResponse.access_token;
            }
            else
            {
                throw new InvalidOperationException("Something went wrong during the authentication process.");
            }
        }
    }
}
