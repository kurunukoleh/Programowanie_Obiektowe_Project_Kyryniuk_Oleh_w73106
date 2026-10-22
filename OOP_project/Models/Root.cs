using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;

namespace OOP_project.Models
{
    internal class Root : Morpheme
    {
        private readonly Random _random = new Random();


        public Root(Database database)
            : base(database)
        {
            using var connection = database.GetConnection();

            string root = GenerateMorpheme(_random.Next(2, 15));

            using var command = new SqliteCommand(
                "INSERT INTO Roots (Root) VALUES (@Root)",
                connection);

            command.Parameters.AddWithValue("@Root", root);

            command.ExecuteNonQuery();
        }

    }
}