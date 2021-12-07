using System;
using System.Threading.Tasks;
using ComicShop.Domain.Features.Identity;
using ComicShop.Infra.Data.Features.Identity;
using ComicShop.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComicShop.WebApi.Controllers.v1.Identity
{
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("/v1/auth")]
        public async Task<IActionResult> AuthAsync([FromBody] User user)
        {
            try
            {
                //uma boa prática seria usar DI (Injeção de dependência)
                //mas não é o foco do artigo

                var userExists = new UserRepository().GetByEmail(user.Email);

                if (userExists == null)
                    return BadRequest(new { Message = "Email e/ou senha está(ão) inválido(s)." });


                if (userExists.Password != user.Password)
                    return BadRequest(new { Message = "Email e/ou senha está(ão) inválido(s)." });


                var token = JwtAuth.GenerateToken(userExists);

                return Ok(new
                {
                    Token = token,
                    Usuario = userExists
                });

            }
            catch (Exception)
            {
                return BadRequest(new { Message = "Ocorreu algum erro interno na aplicação, por favor tente novamente." });
            }
        }
    }
}
