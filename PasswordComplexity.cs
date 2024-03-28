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
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, [FromBody] PasswordComplexityRequest passwordComplexityRequest)
    {
        if (!string.IsNullOrEmpty(passwordComplexityRequest.Fullname))
        {
            var similatity = LevenshteinDistance(passwordComplexityRequest.Password, passwordComplexityRequest.Fullname);
            if (similatity > 0.5)
            {
                throw new NotImplementedException();
            }
        }

        //Check Haveibeenpwned


        throw new NotImplementedException();
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
}
