
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

        public Dictionary<string, string> ReadSymbols()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"SELECT Symbol, Category
                  FROM AlphabetLibrary;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                symbols[reader.GetString(0)] =
                    reader.GetString(1);
            }

            return symbols;
        }

        public Dictionary<string, string> ReadAlphabet()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"SELECT Symbol, Category
                  FROM AlphabetSymbols;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                symbols[reader.GetString(0)] =
                    reader.GetString(1);
            }

            return symbols;
        }

        public List<AlphabetLetter> ReadSymbolsToDelete()
        {
            var symbols = new List<AlphabetLetter>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"SELECT Id, Symbol, Category
          FROM AlphabetSymbols;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                symbols.Add(new AlphabetLetter
                {
                    Id = reader.GetInt32(0),
                    Symbol = reader.GetString(1),
                    Category = reader.GetString(2)
                });
            }

            return symbols;
        }

        public void DeleteLetters(int id)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "DELETE FROM AlphabetSymbols WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void AddSymbol(string symbol, string category)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(
                    "Symbol cannot be empty.", nameof(symbol));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException(
                    "Category cannot be empty.", nameof(category));

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"INSERT INTO AlphabetSymbols (Symbol, Category)
          VALUES (@symbol, @category);",
                connection);

            command.Parameters.AddWithValue("@symbol", symbol.Trim());
            command.Parameters.AddWithValue("@category", category.Trim());

            command.ExecuteNonQuery();
        }

        public void GenerateAlphabet(int lettersCount, int languageId)
        {
            if (lettersCount < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(lettersCount));

            var symbols = ReadSymbols();

            if (symbols.Count == 0)
                throw new InvalidOperationException(
                    "AlphabetLibrary is empty.");

            if (lettersCount > symbols.Count)
                throw new ArgumentException(
                    "Not enough unique symbols in AlphabetLibrary.");

            var random = new Random();

            var selected = symbols
                .OrderBy(x => random.Next())
                .Take(lettersCount)
                .ToList();

            using var connection = _database.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            using (var deleteCommand = new SqliteCommand(
                "DELETE FROM AlphabetSymbols;",
                connection,
                transaction))
            {
                deleteCommand.ExecuteNonQuery();
            }

            using (var insertCommand = new SqliteCommand(
                @"INSERT INTO AlphabetSymbols (Symbol, Category)
                  VALUES (@symbol, @category);",
                connection,
                transaction))
            {
                var symbolParameter =
                    insertCommand.Parameters.Add(
                        "@symbol", SqliteType.Text);

                var categoryParameter =
                    insertCommand.Parameters.Add(
                        "@category", SqliteType.Text);

                foreach (var item in selected)
                {
                    symbolParameter.Value = item.Key;
                    categoryParameter.Value = item.Value;

                    insertCommand.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
    }
}
