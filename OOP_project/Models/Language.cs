using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    public class Language
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Language()
        {
        }

        public Language(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

}
