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

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
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
    }
}
