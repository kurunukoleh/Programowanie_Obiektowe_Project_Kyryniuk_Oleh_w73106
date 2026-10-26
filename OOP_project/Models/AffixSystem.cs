using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace OOP_project.Models
{
    public class AffixSystem : Morpheme
    {
        private readonly Random _random = new Random();
        private Database _database;

        //public int Id { get; set; }
        //public string AffixText { get; set; } = "";
        //public int PartOfSpeechId { get; set; }
        //public int GrammaticalCategoryId { get; set; }
        //public int AffixTypeId { get; set; }


        public AffixSystem(Database database) : base(database)
        {
            _database = database;
        }

        public List<(string Affix, string Type, int PartOfSpeechId,
             string GrammaticalRoleCode)> ReadAffixeInformation()
        {
            var items =
                new List<(string Affix, string Type, int PartOfSpeechId,
                          string GrammaticalRoleCode)>();

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                """
        SELECT
            a.Affix,
            t.Name,
            a.PartOfSpeechId,
            a.GrammaticalRoleCode
        FROM Affixes a
        JOIN AffixType t ON a.AffixTypeId = t.Id;
        """,
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add((
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetString(3)
                ));
            }

            return items;
        }


        public void GenerateAffix()
        {
            using var connection = _database.GetConnection();

            int partOfSpeechId =
                GetRandomId(connection, "PartOfSpeech");

            string grammaticalRoleCode =
                GetRandomCode(connection, "GrammaticalLibrary");

            int affixTypeId =
                GetRandomId(connection, "AffixType");

            string affix = GenerateMorpheme(_random.Next(2, 10));

            using var command = new SqliteCommand(
                @"INSERT INTO Affixes
          (Affix, PartOfSpeechId, GrammaticalRoleCode, AffixTypeId)
          VALUES
          (@Affix, @PartOfSpeechId, @Code, @AffixTypeId);",
                connection);

            command.Parameters.AddWithValue("@Affix", affix);
            command.Parameters.AddWithValue("@PartOfSpeechId", partOfSpeechId);
            command.Parameters.AddWithValue("@Code", grammaticalRoleCode);
            command.Parameters.AddWithValue("@AffixTypeId", affixTypeId);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException(
                    $"nie udało się dodać.\n" +
                    $"Database: {_database.DatabasePath}\n" +
                    $"PartOfSpeechId: {partOfSpeechId}\n" +
                    $"GrammaticalRoleCode: {grammaticalRoleCode}\n" +
                    $"AffixTypeId: {affixTypeId}\n" +
                    $"Affix: {affix}\n" +
                    $"SQLite error: {ex.Message}",
                    ex);
            }
        }

        public void AddAffix(string affix , string partOfSpeechId , string grammaticalRoleCode, string affixTypeId)
        {

            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                @"INSERT INTO Affixes
          (Affix, PartOfSpeechId, GrammaticalRoleCode, AffixTypeId)
          VALUES
          (@Affix, @PartOfSpeechId, @Code, @AffixTypeId);",
                connection);

            command.Parameters.AddWithValue("@Affix", affix);
            command.Parameters.AddWithValue("@PartOfSpeechId", partOfSpeechId);
            command.Parameters.AddWithValue("@Code", grammaticalRoleCode);
            command.Parameters.AddWithValue("@AffixTypeId", affixTypeId);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException(
                    $"nie udało się dodać.\n" +
                    $"Database: {_database.DatabasePath}\n" +
                    $"PartOfSpeechId: {partOfSpeechId}\n" +
                    $"GrammaticalRoleCode: {grammaticalRoleCode}\n" +
                    $"AffixTypeId: {affixTypeId}\n" +
                    $"Affix: {affix}\n" +
                    $"SQLite error: {ex.Message}",
                    ex);
            }
        }

        public void DeleteAffixes(int id)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "DELETE FROM Affixes WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }



        private string GetRandomCode(
            SqliteConnection connection,
            string tableName)
        {
            using var command = new SqliteCommand(
                $"SELECT Code FROM {tableName} ORDER BY RANDOM() LIMIT 1",
                connection);

            object? result = command.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    $"Tabela {tableName} jest pusta.");
            }

            return Convert.ToString(result)!;
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