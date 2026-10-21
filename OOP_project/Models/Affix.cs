using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;

namespace OOP_project.Models
{
    internal class Affix : Morpheme
    {
        private readonly Random _random = new Random();

        public int Id { get; set; }
        public string AffixText { get; set; } = "";
        public int PartOfSpeechId { get; set; }
        public int GrammaticalCategoryId { get; set; }
        public int AffixTypeId { get; set; }

        public Affix(Database database)
            : base(database)
        {
            using var connection = database.GetConnection();

            int partOfSpeechId =
                GetRandomId(connection, "PartOfSpeech");

            int categoryId =
                GetRandomId(connection, "GrammaticalLibrary");

            int affixTypeId =
                GetRandomId(connection, "AffixType");

            string affix = GenerateMorpheme(_random.Next(2, 10));

            using var command = new SqliteCommand(
                @"INSERT INTO Affixes
                  (Affix, PartOfSpeechId,
                   GrammaticalCategoryId, AffixTypeId)
                  VALUES
                  (@Affix, @PartOfSpeechId,
                   @Code, @AffixTypeId)",
                connection);

            command.Parameters.AddWithValue("@Affix", affix);
            command.Parameters.AddWithValue(
                "@PartOfSpeechId", partOfSpeechId);
            command.Parameters.AddWithValue(
                "@Code", categoryId);
            command.Parameters.AddWithValue(
                "@AffixTypeId", affixTypeId);

            command.ExecuteNonQuery();
        }

        private int GetRandomId(
            SqliteConnection connection,
            string tableName)
        {
            using var command = new SqliteCommand(
                $"SELECT Id FROM {tableName} ORDER BY RANDOM() LIMIT 1",
                connection);

            object? result = command.ExecuteScalar();

            if (result == null)
                throw new InvalidOperationException(
                    $"Tablica {tableName} jest pusta.");

            return Convert.ToInt32(result);
        }

        
    }
}