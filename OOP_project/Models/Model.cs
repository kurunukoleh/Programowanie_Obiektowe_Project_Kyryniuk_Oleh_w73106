using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    public abstract class Model
    {

        protected readonly Database _database = new Database();
        public int Id { get; set; }

        public Model(int id)
        {
            Id = id;
        }
    }

    
}
