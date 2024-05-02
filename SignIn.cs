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

            SignInRequest? requestObj = null;
            try
            {
                requestObj = JsonSerializer.Deserialize<SignInRequest>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            var response = req.CreateResponse(HttpStatusCode.OK);

            var responseObj = new SignInResponse();
            if (requestObj == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                responseObj.Status = 500;
                responseObj.userMessage = "Unable to procee the request";
            }
            else
            {
                var password = requestObj.Password?.Substring(0, requestObj.Password.Length - 4);

                if (password?.Equals("1") ?? false)
                {
                    responseObj.Success = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    responseObj.Status = 400;
                    responseObj.userMessage = "Invalid username or password";
                }
            }

            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

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
