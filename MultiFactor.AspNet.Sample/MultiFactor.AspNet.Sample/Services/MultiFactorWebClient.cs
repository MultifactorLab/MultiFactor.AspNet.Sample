using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace MultiFactor.AspNet.Sample.Services
{
    public class MultiFactorWebClient
    {
        private MultiFactorSettings _settings;

        public MultiFactorWebClient(MultiFactorSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string CreateRequest(string login, string postbackUrl)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //payload
            var json = JsonConvert.SerializeObject(new
            {
                Identity = login,
                Callback = new
                {
                    Action = postbackUrl,
                    Target = "_self"
                }
            });

            var requestData = Encoding.UTF8.GetBytes(json);
            byte[] responseData = null;

            //basic authorization
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(_settings.ApiKey + ":" + _settings.ApiSecret));

            using (var web = new WebClient())
            {
                web.Headers.Add("Content-Type", "application/json");
                web.Headers.Add("Authorization", "Basic " + auth);
                responseData = web.UploadData(_settings.ApiUrl + "/access/requests", "POST", requestData);
            }

            json = Encoding.UTF8.GetString(responseData);
            var response = JsonConvert.DeserializeObject<MultiFactorWebResponse<MultiFactorAccessPage>>(json);

            return response.Model.Url;
        }
    }

    public class MultiFactorWebResponse<TModel>
    {
        public bool Success { get; set; }

        public TModel Model { get; set; }
    }

    public class MultiFactorAccessPage
    {
        public string Url { get; set; }
    }
}