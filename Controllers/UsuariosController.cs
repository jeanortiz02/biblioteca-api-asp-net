using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        public UsuariosController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost("registro")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(
            CredencialesUsuarioDTO credencialesUsuarioDTO
        )
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirRespuesta(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem();
            }
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirRespuesta(
            CredencialesUsuarioDTO credencialesUsuarioDTO
        )
        {
            var claim = new List<Claim>
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("Lo que yo deseo", "Cualquier Valor")
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario!);

            claim.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claim,
                expires: expiracion,
                signingCredentials: credenciales
            );
            
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }
    }
}



