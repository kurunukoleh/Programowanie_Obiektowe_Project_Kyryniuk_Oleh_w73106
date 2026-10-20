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

        public Dictionary<string, string> ReadAlphabet()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Symbol, Category FROM AlphabetSymbols", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string symbol = reader.GetString(0);
                string category = reader.GetString(1);

                symbols[symbol] = category;
            }

            return symbols;
        }

        public void InitializeDatabase()
        {
            using var connection = GetConnection();

            string sql = @"
        CREATE TABLE IF NOT EXISTS AlphabetLibrary (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Symbol TEXT NOT NULL,
            Category TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Affixes (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Affix TEXT NOT NULL,
        PartOfSpeechId INTEGER NOT NULL,
        GrammaticalCategoryId INTEGER NOT NULL,
        AffixTypeId INTEGER NOT NULL,
        FOREIGN KEY (PartOfSpeechId) REFERENCES PartOfSpeech(Id),
        FOREIGN KEY (GrammaticalCategoryId) REFERENCES GrammaticalCategory(Id),
        FOREIGN KEY (AffixTypeId) REFERENCES AffixType(Id)
    );

        CREATE TABLE IF NOT EXISTS AlphabetSymbols (
            Id         INTEGER PRIMARY KEY AUTOINCREMENT,
            LanguageId INTEGER NOT NULL,
            Symbol     TEXT    NOT NULL,
            Category   TEXT    NOT NULL,
            FOREIGN KEY (
                LanguageId
            )
            REFERENCES Languages (Id) 
        );

        CREATE TABLE IF NOT EXISTS Roots (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Root TEXT NOT NULL,
            TranslateId TEXT,
        );

        CREATE TABLE IF NOT EXISTS GrammaticalCategory (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            PartOfSpeechId INTEGER NOT NULL,
            Description TEXT,
            FOREIGN KEY (PartOfSpeechId)
                REFERENCES PartOfSpeech(Id),
            UNIQUE (Name, PartOfSpeechId)
        );

        CREATE TABLE IF NOT EXISTS AffixType (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL UNIQUE
        );

        INSERT OR IGNORE INTO AffixType (Name)
        VALUES
            ('Prefix'),
            ('Suffix'),
            ('Infix'),
            ('Circumfix'),
            ('Suprafix'),
            ('Interfix'),
            ('Transfix');

        CREATE TABLE IF NOT EXISTS PartOfSpeech (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL UNIQUE,
            Description TEXT
        );

            ";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }


    }

}
