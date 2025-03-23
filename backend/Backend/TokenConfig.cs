namespace Backend
{
    public class TokenConfig
    {
        public string CorsFrontendOrigin { get; set; } = string.Empty;

        public string JwtSecret { get; set; } = string.Empty;

        public string JwtIssuer { get; set; } = string.Empty;

        public TimeSpan AccessTokenExpiration { get; set; }

        public TimeSpan RefreshTokenExpiration { get; set; }
    }
}
