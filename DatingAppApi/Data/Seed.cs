using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DatingApp.Models;
using DatingApp.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
            {
                var userData = File.ReadAllText("Data/UserSeed.json");

                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach(var user in users)
                {
                    byte[] passwordhas, passwordSalt;

                    CreatePasswordHash("password", out passwordhas, out passwordSalt);
                    user.PasswordHash = passwordhas;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
        }

        private static  void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key; // random generated key
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

        }
    }
}
