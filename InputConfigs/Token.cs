namespace SampleJWT.InputConfigs
{
    public class Token
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double Seconds { get; set; }
    }
}