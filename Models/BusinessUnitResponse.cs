namespace b2c_ApiConnector.Models;

public class BusinessUnitResponse : ErrorResponse
{
    public string AccountStatus { get; set; }

    public string Persona { get; set; }

    public string BusinessId { get; set; }

    public bool? ExtemporaneousAllowed { get; set; }

    public string[] Brands { get; set; }

    public bool? PayByTerms { get; set; }

    public bool? BusinessOwner { get; set; }

    public bool? CanPurchase { get; set; }

    public string MarketoForm { get; set; }

    public BusinessUnitItem[] Businesses { get; set; }
}