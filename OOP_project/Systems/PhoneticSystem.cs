
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_project.Systems
{
    public class PhoneticSystem : I_CRUD<AlphabetLetter>
    {
        private readonly Database _database;

        public PhoneticSystem(Database database)
        {
            _database = database;
        }


        private List<AlphabetLetter> ReadAlphabetLibrary()
        {
            var symbols = new List<AlphabetLetter>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT Id, Symbol, Category FROM AlphabetLibrary;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                symbols.Add(new AlphabetLetter(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)));
            }

            return symbols;
        }







        public void GenerateAlphabet(int lettersCount, int languageId)
        {
            if (lettersCount < 1)
                throw new ArgumentOutOfRangeException(nameof(lettersCount));

            var symbols = ReadAlphabetLibrary();

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
                var symbolParameter = insertCommand.Parameters.Add(
                    "@symbol", SqliteType.Text);

                var categoryParameter = insertCommand.Parameters.Add(
                    "@category", SqliteType.Text);

                foreach (var item in selected)
                {
                    symbolParameter.Value = item.Symbol;
                    categoryParameter.Value = item.Category;

                    insertCommand.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }

        public void delete(int Model_ID)
        {
            using var connection = _database.GetConnection();
            connection.Open();
            using var command = new SqliteCommand(
                "DELETE FROM AlphabetSymbols WHERE Id = @Id;",
                connection);
            command.Parameters.AddWithValue("@Id", Model_ID);
            command.ExecuteNonQuery();
        }


        public List<AlphabetLetter> readtable()
        {
            var symbols = new List<AlphabetLetter>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"SELECT*
                  FROM AlphabetSymbols;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                symbols.Add(new AlphabetLetter
                (
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                ));

            }

            return symbols;

        }
        
        public void insert(AlphabetLetter letter) 
        {

            if (string.IsNullOrWhiteSpace(letter.Symbol))
                throw new ArgumentException("Symbol cannot be empty.");

            if (string.IsNullOrWhiteSpace(letter.Category))
                throw new ArgumentException("Category cannot be empty.");

            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                @"INSERT INTO AlphabetSymbols (Symbol, Category)
          VALUES (@symbol, @category);",
                connection);

            command.Parameters.AddWithValue("@symbol", letter.Symbol.Trim());
            command.Parameters.AddWithValue("@category", letter.Category.Trim());

            command.ExecuteNonQuery();
        }
        public void update(AlphabetLetter letter)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                """
        UPDATE AlphabetSymbols
        SET Symbol = @Symbol,
            Category = @Category
        WHERE Id = @Id;
        """,
                connection);

            command.Parameters.AddWithValue("@Symbol", letter.Symbol);
            command.Parameters.AddWithValue("@Category", letter.Category);
            command.Parameters.AddWithValue("@Id", letter.Id);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows == 0)
                throw new InvalidOperationException("Symbol not found.");
        }

    }
}
