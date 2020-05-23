using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Contracts;
using DatingApp.Dtos;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;

        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo,
            IMapper mapper, IOptions<CloudinarySettings> cloudinaryOptions)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryOptions;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret

                );

            _cloudinary = new Cloudinary(acc);
           
        }


   

        // POST: api/Photos
        [HttpPost]
        public async Task<IActionResult>  AddPhotoForUser(int userId,[FromForm]PhotoForCreationDto dto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);

            var file = dto.File;

            var uploadResults = new ImageUploadResult();

            if(file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };

                    uploadResults = _cloudinary.Upload(uploadParams);
                }
            }

            dto.Url = uploadResults.Uri.ToString();
            dto.PublicId = uploadResults.PublicId;

            var photo = _mapper.Map<Photo>(dto);
            
            if(!user.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }


            user.Photos.Add(photo);
       
            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoToReturn>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId,int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhoto(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set a photo to main :(");
        }
       

        [HttpGet("{id}",Name ="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoToReturn>(photoFromRepo);

            return Ok(photo);
        }

        [HttpDelete("{{id}}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");




            if(photoFromRepo.PuplicId != null)
            {
                var deleteparams = new DeletionParams(photoFromRepo.PuplicId);
                var result = _cloudinary.Destroy(deleteparams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }
         
            if(photoFromRepo.PuplicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photos");
        }

        // PUT: api/Photos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

     
    }
}
