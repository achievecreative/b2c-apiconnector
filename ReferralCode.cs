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
    public class ReferralCode
    {
        private readonly ILogger _logger;

        public ReferralCode(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ReferralCode>();
        }

        [Function("referralcode")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string body;
            using (var streamReader = new StreamReader(req.Body))
            {
                body = streamReader.ReadToEnd();
            }

            _logger.LogInformation("referralcode Request: {0}", body);

            var obj = JsonSerializer.Deserialize<ReferralCodeRequest>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            if (obj.ReferralCode.EndsWith("Code", StringComparison.OrdinalIgnoreCase))
            {
                return response;
            }

            response.StatusCode = HttpStatusCode.BadRequest;
            var responseObj = new
            {
                Version = "1.0.0",
                Status = (int)HttpStatusCode.BadRequest,
                userMessage = "Referral code is missing or invalid",
                developerMessage = "Referral code is missing or invalid - Developer"
            };

            JsonSerializer.Serialize(response.Body, responseObj, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            return response;
        }
    }
}
