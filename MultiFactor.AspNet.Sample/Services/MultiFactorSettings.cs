using System.Configuration;

namespace MultiFactor.AspNet.Sample.Services
{
    public class MultiFactorSettings
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; }
        public string ApiSecret { get; }

        public MultiFactorSettings()
        {
            ApiUrl = ConfigurationManager.AppSettings["mfa-api-url"];
            ApiKey = ConfigurationManager.AppSettings["mfa-api-key"];
            ApiSecret = ConfigurationManager.AppSettings["mfa-api-secret"];
        }
    }
}