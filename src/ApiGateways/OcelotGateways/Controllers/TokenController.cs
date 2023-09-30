using Contracts.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OcelotGateways.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService) 
        { 
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var result = _tokenService.GetToken(new Shared.DTO.Identity.TokenRequest());
            return Ok(result);
        }
    }
}
