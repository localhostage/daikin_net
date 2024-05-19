namespace DaikinNet;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int AccessTokenExpiresIn { get; set; }
    public string TokenType { get; set; }
}