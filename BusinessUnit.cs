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
using System.Security.Principal;

namespace b2c_ApiConnector
{
    public class BusinessUnit
    {
        private readonly ILogger _logger;

        public BusinessUnit(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BusinessUnit>();
        }

        [Function("businessUnit")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            req.Headers.TryGetValues("Authorization", out IEnumerable<string> authorization);

            _logger.LogInformation($"Token: {authorization?.FirstOrDefault()}"); //Authorization

            string body;
            using (var streamReader = new StreamReader(req.Body))
            {
                body = streamReader.ReadToEnd();
            }

            _logger.LogInformation("Business Unit Request: {0}", body);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var responseObj = new
            {
                Version = "1.0.0",
                BusinessUnits = new[]{
                    new {Id="1", Name="Business 1"}, new {Id="2", Name="Business 2"}
                },
                User = new
                {
                    Source = "API",
                    FirstName = "Firstname",
                    LastName = "Last name",
                    registrationComplete = false,
                    registrationStep = 2,
                    isEligableForUpgrade = false
                },
                Action = "Continue",
            };

            JsonSerializer.Serialize(response.Body, responseObj, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return response;
        }
    }
}
