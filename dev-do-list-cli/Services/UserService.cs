using dev_do_list_cli.Models;
using System.Text.Json;

namespace dev_do_list_cli.Services
{
    public static class UserService
    {
        public static string? Username { get; set; }
        public static int UserId { get; set; }

        public static async Task GetUserDetails()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/user/loggedIn");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseContent = JsonSerializer.Deserialize<UserResponse>(responseString);

                Username = responseContent.username;
                UserId = responseContent.userId;
            }
            else
            {
                throw new Exception("Failed to get user details from the API");
            }
        }
    }
}
