namespace b2c_ApiConnector;

public class PasswordComplexityResponse : ErrorResponse
{
    public double similatity { get; set; }

    public bool Compromised { get; set; }
}
