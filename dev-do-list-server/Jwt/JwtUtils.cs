using System.IdentityModel.Tokens.Jwt;

namespace DevDoListServer.Jwt;

public static class JwtUtils
{
    public static string GetClaim(string token, string claimKey)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
        return jwt.Claims.First(c => c.Type == claimKey).Value;
    }
}
