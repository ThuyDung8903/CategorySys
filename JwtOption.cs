namespace CategorySys
{
    public class JwtOption
    {
        public const string SectionName = "Jwt";
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
