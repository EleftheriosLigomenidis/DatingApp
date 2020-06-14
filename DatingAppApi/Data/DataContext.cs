using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) 
            : base(options)
        {}


        public DbSet<Value> Values { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(l => new { l.LikerId, l.LikeeId });

            builder.Entity<Like>()
                .HasOne(l => l.Likee)
                .WithMany(l => l.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
             .HasOne(l => l.Liker)
             .WithMany(l => l.Likees)
             .HasForeignKey(u => u.LikeeId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
          .HasOne(l => l.Sender)
          .WithMany(l => l.MessagesSent)
          .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
        .HasOne(l => l.Recipient)
        .WithMany(l => l.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
