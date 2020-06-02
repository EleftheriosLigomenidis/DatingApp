﻿using AutoMapper;
using DatingApp.Contracts;
using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repositories
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

   
        public DatingRepository( DataContext context, IMapper mapper )
        {
            _context = context;
         
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhoto(int id)
        {
            return await _context.Photos.Where(u => u.UserId == id).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync( p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id )
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(p => p.Photos).ToListAsync();

            return users;
        }

        public  async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos);

            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
     
        }

        public async Task<bool> SaveAll()
        {

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
