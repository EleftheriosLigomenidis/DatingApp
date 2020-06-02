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
using AutoMapper;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _conf;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo,IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _conf = config;
            _mapper = mapper;
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

            var userToCreate = _mapper.Map<User>(dto);

        
            var createdUser = await _repo.Register(userToCreate, dto.Password);
            var userToReturn = _mapper.Map<UserForDetailsDto>(createdUser);
            return CreatedAtRoute("GetUser", new { Controller = "Users",id = createdUser.Id  },userToReturn);


        }


        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            //throw new Exception("QQ");
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
            var user = _mapper.Map<UserForListDto>(userFromRepo);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });

        }
    }
}
