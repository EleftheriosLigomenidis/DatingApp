﻿

using System;

namespace DatingApp.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Discription { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsMain { get; set; }

        public string PuplicId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}