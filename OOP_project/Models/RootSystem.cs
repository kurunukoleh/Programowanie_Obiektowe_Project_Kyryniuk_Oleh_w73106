
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Web;

namespace OOP_project.Models
{
    public class RootSystem : Morpheme
    {
        private readonly Random _random = new Random();
        private readonly Database _database;

        public RootSystem(Database database) : base(database)
        {
            _database = database;
        }

        public Dictionary<int, string> ReadRoots()
        {
            var items = new Dictionary<int, string>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT Id, Symbol FROM Roots;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int key = reader.GetInt32(0);
                string item = reader.GetString(1);

                items.Add(key, item);
            }

            return items;
        }

        public void GenerateRoots(int count = 1)
        {
            if (count < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Illość nie może być mnieksz za zero.");

            using var connection = _database.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            using var command = new SqliteCommand(
                "INSERT INTO Roots (Symbol) VALUES (@Symbol);",
                connection,
                transaction);

            var parameter = command.Parameters.Add(
                "@Symbol",
                SqliteType.Text);

            for (int i = 0; i < count; i++)
            {
                parameter.Value = GenerateMorpheme(_random.Next(2, 15));
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public void AddRoot(string root, string translate)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"INSERT INTO Roots (Symbol, Translation)
          VALUES (@Symbol, @Translation);",
                connection);

            command.Parameters.AddWithValue("@Symbol", root);
            command.Parameters.AddWithValue("@Translation", translate);

            command.ExecuteNonQuery();
        }


        public void DeleteRoot(int id)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "DELETE FROM Roots WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void UpdateRootTranslation(int rootId, string translation)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"UPDATE Roots
          SET Translation = @Translation
          WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Translation", translation);
            command.Parameters.AddWithValue("@Id", rootId);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows == 0)
                throw new InvalidOperationException("Root not found.");
        }
    }
}
