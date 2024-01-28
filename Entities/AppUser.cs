﻿using DwitterSocial.Data;
using DwitterSocial.Extensions;
using Microsoft.AspNetCore.Identity;

namespace DwitterSocial.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string LookingFor { get; set; } = string.Empty;
        public string Interests { get; set; } = string.Empty;   
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<Photo> Photos { get; set; } = new();
        public ICollection<UserLikes> LikedUsers { get; set; }
        public ICollection<UserLikes> LikedByUsers { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}