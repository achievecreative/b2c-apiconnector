using b2c_ApiConnector.Models;

namespace b2c_ApiConnector;

public class ErrorResponse : ResponseObject
{
    public int? Status { get; set; } = null;

    public string? userMessage { get; set; }
}
