﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET: api/Values
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            // return http responds action result
            var values =  await _context.Values.ToListAsync();
            return  Ok(values);
        }
           
        

        // GET: api/Values/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FindAsync(id);
            return Ok(value);
        }

        // POST: api/Values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var mydate = new DateTime(1989, 1, 1);

          var date =  mydate.Date;
        }
    }
}
