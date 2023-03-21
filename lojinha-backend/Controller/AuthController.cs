using lojinha_backend.Business;
using lojinha_backend.Data.VO;
using lojinha_backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
namespace lojinha_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;

        public AuthController(ILoginBusiness loginBusiness)
        {
            _loginBusiness = loginBusiness;
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult Signin([FromBody] User user)
        {
            if (user == null) return BadRequest("Invalid client request");
            var token = _loginBusiness.ValidateCredentials(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenVO tokenVo) 
        {
            if(tokenVo == null) return BadRequest("Invalid client request");
            var token = _loginBusiness.RefreshExpiredToken(tokenVo);
            if(token == null) return BadRequest("Invalid client request");
            return Ok(token);
        }

        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        public IActionResult Revoke()
        {   
            var username = User.Identity.Name;
            var result = _loginBusiness.RevokeToken(username);
            if (!result) return BadRequest("Invalid client request");
            return NoContent();
        }
    }
}
