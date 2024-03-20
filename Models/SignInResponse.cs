namespace b2c_ApiConnector;

public class SignInResponse
{
    public string Version { get; set; } = "1.0.0";

    public int? Status { get; set; } = null;

    public string? userMessage { get; set; }

    public bool Success { get; set; }
}
