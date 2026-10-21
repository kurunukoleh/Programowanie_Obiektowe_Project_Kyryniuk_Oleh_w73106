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


            using var command = new SqliteCommand(
                "INSERT INTO Roots (Symbol) VALUES (@Root)",
                connection);

            command.Parameters.AddWithValue(
                "@Root",
                GenerateMorpheme(_random.Next(2, 15)));

            command.ExecuteNonQuery();
        }
    }
}