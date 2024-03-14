using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace b2c_ApiConnector
{
    public class SignUp
    {
        private readonly ILogger _logger;

        public SignUp(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SignUp>();
        }

        [Function("signUp")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string body;
            using (var streamReader = new StreamReader(req.Body))
            {
                body = streamReader.ReadToEnd();
            }

            _logger.LogInformation("Sign Up Request");
            _logger.LogInformation(body);

            var requestObj = JsonSerializer.Deserialize<JsonObject>(body);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var responseObj = new { Version = "1.0.0", City = "City from Azure Function", Name = "Name From Azure Function", Action = "Continue", };

            JsonSerializer.Serialize(response.Body, responseObj, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return response;
        }
    }
}
