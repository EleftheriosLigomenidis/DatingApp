using AutoMapper;
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


        public DatingRepository(DataContext context, IMapper mapper)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public async Task<Like> GetLike(int userId,int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(l => l.LikeeId == recipientId 
            && l.LikerId == userId);
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
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        //public async Task<IEnumerable<User>> GetUsers(UserParams userParams)
        //{
        //    var users =  _context.Users.Include(p => p.Photos).AsQueryable();

        //    users = users.Where(u => u.Id != userParams.UserId);

        //    users = users.Where(u => u.Gender != userParams.Gender);
        //    return  users;
        //}

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDob);

            }

            if (userParams.Likees)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
       
            }

            if (userParams.Likers)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (!string.IsNullOrWhiteSpace(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                            break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);

        }

        private async Task<IEnumerable<int>> GetUserLikes(int id,bool likers)
        {
            // the current uer
            var user = await  _context.Users
                .Include(u => u.Likers)
                .Include(u => u.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (likers)
            {
                //users who liked the current user
                return user.Likers.Where(u => u.LikeeId == id).Select(u => u.LikerId);
            }
            else
            {
                //users who the current user likes
                return user.Likees.Where(u => u.LikerId == id).Select(u => u.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender)
                .ThenInclude(p => p.Photos).Include(u => u.Recipient)
                .AsQueryable();


            switch (messageParams.MessagesContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted==false);
                    break;
                case"Outbox":
                     messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted ==false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead==false && u.RecipientDeleted ==false);
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent);

            return  await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
              var messages = await _context.Messages
                .Include(u => u.Sender)
                .ThenInclude(p => p.Photos).Include(u => u.Recipient)
                .Where(m => m.RecipientId == userId && m.RecipientDeleted==false 
                && m.SenderId == recipientId 
                || m.RecipientId == recipientId && m.SenderId == userId 
                && m.SenderDeleted ==false)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}
