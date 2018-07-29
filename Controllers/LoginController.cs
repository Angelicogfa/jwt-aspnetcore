using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleJWT.InputConfigs;
using SampleJWT.Models;

namespace SampleJWT.Controllers
{
    [Produces("application/json")]
    public class LoginController : Controller
    {
        readonly Token _token;
        public LoginController(IOptions<Token> token)
        {
            _token = token.Value;
        }

        [Route("token"), HttpPost]
        public IActionResult Post([FromBody] LoginInput login)
        {
            //Processo de validação de token

            //Cria o payload do token e gerar o token  
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(login.Login, "Login"),
                    new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, login.Login)
                    }
                );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromSeconds(_token.Seconds);

            var key = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_token.Key)), SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _token.Issuer,
                Audience = _token.Audience,
                SigningCredentials = key,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            return Ok(new
            {
                authenticated = true,
                created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                message = "OK"
            });
        }
    }
}