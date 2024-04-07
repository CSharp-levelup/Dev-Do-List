namespace DevDoListServer.Models.Dtos;

public class JwtDto
{
    public string access_token { get; set; }
    public int expiration { get; set; }
    public string type { get; set; }
}
