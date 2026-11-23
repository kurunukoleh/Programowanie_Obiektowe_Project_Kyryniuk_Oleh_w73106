
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;
using System.Collections.Generic;

namespace OOP_project.Systems
{
    public class DictionarySystem : I_CRUD<WordDictionaryModel>
    {
        private readonly Database _database;

        public DictionarySystem(Database database)
        {
            _database = database;
        }

        public void delete(int modelId)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                "DELETE FROM Dictionary WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", modelId);

            command.ExecuteNonQuery();
        }

        public List<WordDictionaryModel> readtable()
        {
            var items = new List<WordDictionaryModel>();

            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                @"SELECT Id, Word, Translation,  CreatedAt
                  FROM Dictionary
                  ORDER BY Word ;",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new WordDictionaryModel(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    //reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    reader.GetString(3)
                ));
            }

            return items;
        }

        public void insert(WordDictionaryModel model)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                @"INSERT INTO Dictionary
                  (Word, Translation, RootId)
                  VALUES (@Word, @Translation, @RootId);",
                connection);

            command.Parameters.AddWithValue("@Word", model.Word);
            command.Parameters.AddWithValue("@Translation", model.Translation);
            command.Parameters.AddWithValue("@RootId", DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void update(WordDictionaryModel model)
        {
            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                @"UPDATE Dictionary
                  SET Word = @Word,
                      Translation = @Translation,
                      RootId = @RootId
                  WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@Word", model.Word);
            command.Parameters.AddWithValue("@Translation", model.Translation);
            //command.Parameters.AddWithValue(
            //    "@RootId",
            //    (object?)model.RootId ?? DBNull.Value);

            command.ExecuteNonQuery();
        }
    }
}
