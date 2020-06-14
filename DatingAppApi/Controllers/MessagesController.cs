using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Contracts;
using DatingApp.Dtos;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturn>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage,
                messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPage);

            return Ok(messages);
        }
   [HttpGet("thread/{recipientId}")]
   public async Task<IActionResult> GetMessageThread(int userId,int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map < IEnumerable<MessageToReturn>>(messageFromRepo);

            return Ok();
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var MessageFromRepo = _repo.GetMessage(userId);

            if (MessageFromRepo == null)
                return NotFound();

            return Ok(MessageFromRepo);
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> Post(int userId, MessageFromCreationDto dto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            dto.SenderId = userId;

            var Recipient = await _repo.GetUser(dto.RecipientId);

            if(Recipient == null)
            {
                return BadRequest("could not find user");

            }

            var message = _mapper.Map<Message>(dto);

            _repo.Add(message);
            var MessageToReturn = _mapper.Map<MessageFromCreationDto>(message);
            if (await _repo.SaveAll())
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id, MessageToReturn });
            throw new Exception("Creating the message failed on save");
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
