using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;

namespace OOP_project.Systems
{
    public class AffixSystem : Morpheme , I_CRUD<AffixModel>
    {
        private readonly Random _random = new Random();
        private Database _database;


        public AffixSystem(Database database) : base(database)
        {
            _database = database;
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




        public List<AffixModel> readtable()
        {

            

            var items = new List<AffixModel>();

            

            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                """
                SELECT
                    t.Id,
                    t.Affix,
                    t.PartOfSpeechId,
                    t.GrammaticalRoleCode,
                    t.AffixTypeId,
                    a.Name
                FROM Affixes t
                JOIN AffixType a ON a.Id = t.AffixTypeId;
                """,
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new AffixModel(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetString(3),
                    reader.GetInt32(4),
                    reader.GetString(5)
                ));
            }

            if (items.Any(x => x is not AffixModel))
                throw new ArgumentException("Nieprawidłowy typ modelu.");

            return items;
        }



        public void insert(AffixModel model)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                """
        INSERT INTO Affixes
        (Affix, PartOfSpeechId, GrammaticalRoleCode, AffixTypeId)
        VALUES
        (@Affix, @PartOfSpeechId, @Code, @AffixTypeId);
        """,
                connection);

            command.Parameters.AddWithValue("@Affix", model.Affix);
            command.Parameters.AddWithValue("@PartOfSpeechId", model.PartOfSpeechId);
            command.Parameters.AddWithValue("@Code", model.GrammaticalRoleCode);
            command.Parameters.AddWithValue("@AffixTypeId", model.AffixTypeId);

            command.ExecuteNonQuery();
        }


        public void delete(int id)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                "DELETE FROM Affixes WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }



        public void update(AffixModel model)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                """
        UPDATE Affixes
        SET Affix = @Affix,
            PartOfSpeechId = @PartOfSpeechId,
            GrammaticalRoleCode = @Code,
            AffixTypeId = @AffixTypeId
        WHERE Id = @Id;
        """,
                connection);

            command.Parameters.AddWithValue("@Affix", model.Affix);
            command.Parameters.AddWithValue("@PartOfSpeechId", model.PartOfSpeechId);
            command.Parameters.AddWithValue("@Code", model.GrammaticalRoleCode);
            command.Parameters.AddWithValue("@AffixTypeId", model.AffixTypeId);
            command.Parameters.AddWithValue("@Id", model.Id);

            command.ExecuteNonQuery();
        }

    }
}