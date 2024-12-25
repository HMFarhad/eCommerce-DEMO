using eCommerce.Infrastructure.Auth;

namespace eCommerce.Api.Controllers.Codelist;


[Route("v{version:apiVersion}/Demo")]
[Tags("Demo")]
public class DemoController : VersionedApiController
{

    public DemoController()
    {
    }

    [HttpPost("Get-Something")]
    [OpenApiOperation("Inquiry", "This API is used to perform lookup")]
    [Authorize(AuthenticationSchemes = AuthSchemes.ApiKey)]
    public async Task<IActionResult> DemoSomething()
    {
     
        return Ok("Request Successfully returned desired data");
        
    }

}
