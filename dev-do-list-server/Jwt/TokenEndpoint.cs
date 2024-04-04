using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using DevDoListServer.Models;

namespace DevDoListServer.Jwt
{
    public static class TokenEndpoint
    {
        public static async Task<IResult> Connect(HttpContext ctx, JwtOptions jwtOptions)
        {
            // validates the content type of the request
            var headers = ctx.Request.Headers;
            if(headers.ContainsKey("Authorization") == false)
            {
                return Results.BadRequest("No Authorization provided");
            }
            var Hello = Environment.GetEnvironmentVariable("TEST");
            var AuthHeader = headers.GetCommaSeparatedValues("Authorization")[0];
            var token = AuthHeader.Substring(7);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Headers.Add("User-Agent", "request");
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return Results.BadRequest("Invalid Credentials");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var githubUser = JsonConvert.DeserializeObject<GithubUser>(responseString)!;
            //creates the access token (jwt token)
            var tokenExpiration = TimeSpan.FromSeconds(jwtOptions.ExpirationSeconds);
            var accessToken = CreateAccessToken(
                jwtOptions,
                githubUser.login,
                TimeSpan.FromMinutes(1440),
                new[] { "user" });

            //returns a json response with the access token
            return Results.Ok(new
            {
                access_token = accessToken,
                expiration = (int)tokenExpiration.TotalSeconds,
                type = "bearer"
            });
        }

        //
        static string CreateAccessToken(
          JwtOptions jwtOptions,
          string username,
          TimeSpan expiration,
          string[] permissions)
        {
            var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
            var symmetricKey = new SymmetricSecurityKey(keyBytes);

            var signingCredentials = new SigningCredentials(
                symmetricKey,
                // 👇 one of the most popular. 
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim("username", username),/*
                new Claim("name", username),
                new Claim("aud", jwtOptions.Audience)*/
            };

            var roleClaims = permissions.Select(x => new Claim("role", x));
            claims.AddRange(roleClaims);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.Add(expiration),
                signingCredentials: signingCredentials);

            var rawToken = new JwtSecurityTokenHandler().WriteToken(token);
            return rawToken;
        }
    }
}