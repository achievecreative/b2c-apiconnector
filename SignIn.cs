using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using b2c_ApiConnector.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace b2c_ApiConnector
{
    public class SignIn
    {
        private readonly ILogger _logger;

        public SignIn(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SignIn>();
        }

        [Function("signIn")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string body;
            using (var streamReader = new StreamReader(req.Body))
            {
                body = streamReader.ReadToEnd();
            }

            _logger.LogInformation("Sign In Request");
            _logger.LogInformation(body);

            var requestObj = JsonSerializer.Deserialize<JsonObject>(body);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var responseObj = new { Version = "1.0.0", City = "City from Azure Function",  Name = "Name From Azure Function", Action = "Continue", };

            JsonSerializer.Serialize(response.Body, responseObj, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return response;
        }
    }
}
