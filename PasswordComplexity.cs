using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace b2c_ApiConnector;

public class PasswordComplexity
{
    private readonly ILogger _logger;

    public PasswordComplexity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PasswordComplexity>();
    }

    [Function("password-complexity")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, [FromBody] PasswordComplexityRequest passwordComplexityRequest)
    {
        var response = new PasswordComplexityResponse();

        if (!string.IsNullOrEmpty(passwordComplexityRequest.Fullname))
        {
            var similatity = response.similatity = LevenshteinDistance(passwordComplexityRequest.Password, passwordComplexityRequest.Fullname);
            if (similatity < 0.5)
            {
                return await CreateResponse(req, HttpStatusCode.BadRequest, response);
            }
        }

        //Check Haveibeenpwned

        var isPasswordCompromised = response.Compromised = await IsPasswordCompromisedAsync(passwordComplexityRequest.Password);

        if (isPasswordCompromised)
        {
            return await CreateResponse(req, HttpStatusCode.BadRequest, response);
        }

        return await CreateResponse(req, HttpStatusCode.OK, response);
    }

    private double LevenshteinDistance(string password, string fullName)
    {
        var dp = new int[password.Length + 1, fullName.Length + 1];
        for (int i = 0; i <= password.Length; i++)
        {
            dp[i, 0] = i;
        }

        for (int j = 0; j <= fullName.Length; j++)
        {
            dp[0, j] = j;
        }

        for (int i = 1; i <= password.Length; i++)
        {
            for (int j = 1; j <= fullName.Length; j++)
            {
                if (password[i - 1] == fullName[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1])) + 1;
                }
            }
        }

        var distance = dp[password.Length, fullName.Length];

        return distance / (double)fullName.Length;
    }

    private async Task<bool> IsPasswordCompromisedAsync(string password)
    {
        var hash = GeneratePasswordHash(password);

        var prefix = hash.Substring(0, 5);
        var suffix = hash.Substring(5);

        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync($"https://api.pwnedpasswords.com/range/{prefix}");
        var hashArray = response?.Split("\r\n");
        var exists = hashArray?.Any(x => x.StartsWith(suffix));

        return exists ?? false;
    }
    private string GeneratePasswordHash(string password)
    {
        using var sha1 = SHA1.Create();

        var hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

        return Convert.ToHexString(hashed);
    }

    private async Task<HttpResponseData> CreateResponse(HttpRequestData req, HttpStatusCode httpStatus, PasswordComplexityResponse response)
    {
        var httpResponse = req.CreateResponse(httpStatus);
        await httpResponse.WriteAsJsonAsync(response, new JsonObjectSerializer(new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        }));
        httpResponse.StatusCode = httpStatus;
        return httpResponse;
    }
}
