namespace TaskProTracker.MinimalAPI.Options
{
    public class LocalAuthIssuerOptions
    {
        public string Key { get; set; }
        public List<string> ValidAudiences { get; set; }
        public string ValidIssuer { get; set; }
    }
}
