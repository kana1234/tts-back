namespace Charts.Shared.Data
{
    public class AppSettings
    {
        public AuthOption AuthOptions { get; set; }
        public VolnaOption Volna { get; set; }
        public MongoConfig MongoConfig { get; set; }
    }

    public class AuthOption
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int Lifetime { get; set; } = 0;
        public int LifetimeRefresh { get; set; } = 0;
    }

    public class VolnaOption
    {
        public string Url { get; set; }
        public string AuthUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string AdminAccessToken { get; set; }
    }

    public class MongoConfig
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string Database2 { get; set; }
    }

}
