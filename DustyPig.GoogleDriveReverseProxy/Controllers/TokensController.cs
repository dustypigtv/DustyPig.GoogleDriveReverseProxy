using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DustyPig.GoogleDriveReverseProxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokensController : ControllerBase
    {
        readonly IConfiguration _configuration;

        public TokensController(IConfiguration configuration) => _configuration = configuration;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var query = Request.Headers["X-Original-URI"][0];
            query = query.Substring(query.IndexOf("?") + 1);
            var queryParams = query.Split('&', StringSplitOptions.RemoveEmptyEntries).Select(item =>
            {
                var parts = item.Split('=');
                return new KeyValuePair<string, string>(parts[0], parts[1]);
            }).ToList();

            var api_key = queryParams.First(item => item.Key.Equals("api_key", StringComparison.CurrentCultureIgnoreCase)).Value;
            if (api_key != _configuration["API_KEY"])
                return BadRequest("Invalid api_key");



            string json = await System.IO.File.ReadAllTextAsync(_configuration["GOOGLE_DRIVE_SERVICE_CREDENTIALS"]);
            string acct = _configuration["GOOGLE_DRIVE_ACCOUNT"];

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential
                        .FromJson(json)
                        .CreateScoped(new string[] { DriveService.Scope.Drive })
                        .CreateWithUser(acct),
                ApplicationName = "Dusty Pig"
            });

            string token = await ((ICredential)service.HttpClientInitializer).GetAccessTokenForRequestAsync();

            Response.Headers.Add("token", token);

            return Ok();
        }
    }
}
