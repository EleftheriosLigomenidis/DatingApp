using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Contracts;
using DatingApp.Repositories;
using DatingApp.Models;
using DatingApp.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _conf;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _conf = config;
        }

       [HttpPost("register")]

    
       public async Task<IActionResult> Register(UserForRegisterDto dto)
        {
            // validate request

            dto.Username = dto.Username.ToLower();

            if(await _repo.UserExists(dto.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User()
            {
                Username = dto.Username
            };

            var createdUser = await _repo.Register(userToCreate, dto.Password);

            return StatusCode(201);

        }


        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var userFromRepo = await _repo.Login(userForLogin.Username.ToLower(), userForLogin.Password);

            if(userFromRepo == null)
            {
                return Unauthorized();
            }
            // Claims of the token 
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
            
           
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_conf.GetSection("AppSettings:Token").Value)); // the signature of the server !!       

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
             
            var tokenHandler = new JwtSecurityTokenHandler();

            var token =tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}
