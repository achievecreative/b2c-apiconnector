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
using b2c_ApiConnector.Models;

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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, [FromBody] BusinessUnitRequest businessUnit)
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

            var responseObj = new BusinessUnitResponse
            {
                Version = "1.0.0",
                Businesses = new BusinessUnitItem[] {
                    new BusinessUnitItem{ BusinessName = "Business 1", BusinessId="B111"},
                    new BusinessUnitItem{ BusinessName = "Business 2", BusinessId="C2222"},
                    new BusinessUnitItem{ BusinessName = "Business 3", BusinessId="C1111"}
                 },
                Action = "Continue",
            };

            if (!string.IsNullOrEmpty(businessUnit?.BusinessId))
            {
                responseObj.Businesses = new BusinessUnitItem[] { new BusinessUnitItem { BusinessId = businessUnit.BusinessId, BusinessName = "Business name" + businessUnit.BusinessId } };
                responseObj.AccountStatus = "registration_pending";
                responseObj.Persona = "AHP2";
                responseObj.BusinessId = businessUnit.BusinessId;
                responseObj.ExtemporaneousAllowed = true;
                responseObj.Brands = new[] { "BC", "BP" };
                responseObj.PayByTerms = false;
                responseObj.BusinessOwner = true;
                responseObj.CanPurchase = true;
                responseObj.MarketoForm = "M1123";
            }

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
