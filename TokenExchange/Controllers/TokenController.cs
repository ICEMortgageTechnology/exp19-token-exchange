using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TokenExchange.Models;

namespace TokenExchange.Controllers
{
    public class TokenController : ApiController
    {
        // POST api/token
        public async Task<IHttpActionResult> Post(TokenRequest request)
        {
            // Create the content to send to the Ellie Mae OAuth Token endpoint
            Dictionary<string, string> authValues = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", ConfigurationManager.AppSettings["oauth.clientid"] },
                { "client_secret", ConfigurationManager.AppSettings["oauth.clientsecret"] },
                { "code", request.AuthCode },
                { "scope", "lp" }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(authValues);

            // Invoke the Ellie Mae Identity Token Endpoint
            Uri idpUri = new Uri(ConfigurationManager.AppSettings["oauth.host"]);
            Uri tokenUri = new Uri(idpUri, "oauth2/v1/token");

            using (HttpClient http = new HttpClient())
            using (var authResponse = await http.PostAsync(tokenUri, content))
            {
                if (!authResponse.IsSuccessStatusCode)
                    return StatusCode(HttpStatusCode.Forbidden);

                return Ok(await authResponse.Content.ReadAsAsync<JObject>());
            }
        }
    }
}
