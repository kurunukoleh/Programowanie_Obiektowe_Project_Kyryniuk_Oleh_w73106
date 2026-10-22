
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_project.Models
{
    public class PhoneticSystem
    {
        private readonly Database _database;

        public PhoneticSystem(Database database)
        {
            _database = database;
        }

        public void GenerateAlphabet(int lettersCount, int languageId)
        {
            var symbols = _database.ReadSymbols();

            if (symbols.Count == 0)
            {
                throw new InvalidOperationException(
                    "AlphabetLibrary is empty.");
            }

            var random = new Random();

            using var connection = _database.GetConnection();

            for (int i = 0; i < lettersCount; i++)
            {
                var item = symbols.ElementAt(
                    random.Next(symbols.Count));

                using var command = new SqliteCommand(
                    @"INSERT INTO AlphabetSymbols
                      (Symbol, Category)
                      VALUES (@symbol, @category)",
                    connection);

                

                command.Parameters.AddWithValue(
                    "@symbol", item.Key);

                command.Parameters.AddWithValue(
                    "@category", item.Value);

                command.ExecuteNonQuery();
            }
        }
    }
}
