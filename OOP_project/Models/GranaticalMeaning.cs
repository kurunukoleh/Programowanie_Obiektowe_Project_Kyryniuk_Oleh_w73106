using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    internal class GranaticalMeaning
    {

        private readonly Database _database = new Database();
        private List<string> _choose_roles;

        public GranaticalMeaning(int RolesCount)
        {
            _choose_roles = _database.ReadGramaticalRoles();

            Random random = new Random();
            using var connection = _database.GetConnection();

            if (_choose_roles.Count > 0)
            {
              

            }
        }
    }
}
