﻿using System.ComponentModel.DataAnnotations;

namespace DwitterSocial.Entities
{
    public class Group
    {
        public Group()
        {
        }
        public Group(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; } = string.Empty;
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();  
    }
}
