using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace OOP_project.Data
{
    public class Database
    {
        private readonly string _connectionString =
            @"Data Source=C:\LinguaForge\linguaForge.db";

        public SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public Dictionary<string, string> ReadSymbols()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Symbol, Category FROM AlphabetLibrary", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string symbol = reader.GetString(0);
                string category = reader.GetString(1);

                symbols[symbol] = category;
            }

            return symbols;
        }


    }

}
