using Microsoft.AspNetCore.Authentication;

namespace Web.API.Auth;

public class ApiKeyAuthSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } =
        "VerySecret"; // We would call this from somewere very secure like Azure Key vault or AWS Secrets manager
}