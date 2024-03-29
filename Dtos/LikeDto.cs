﻿using System.ComponentModel.DataAnnotations;

namespace DwitterSocial.Dtos
{
    public class LikeDto
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }

    }
}
