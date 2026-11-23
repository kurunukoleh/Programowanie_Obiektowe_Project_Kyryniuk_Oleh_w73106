
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;

namespace OOP_project.Systems
{
    public class RootSystem : Morpheme , I_CRUD<RootModel>
    {
        private readonly Random _random = new Random();
        private readonly Database _database;

        public RootSystem(Database database) : base(database)
        {
            _database = database;
        }

        

        public void GenerateRoots(int count = 1)
        {
            if (count < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Illość nie może być mnieksz za zero.");

            using var connection = _database.GetConnection();
            

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


        public RootModel GetRootById(int id)
        {
            using var connection = _database.GetConnection();
            

            using var command = new SqliteCommand(
                "SELECT Id, Symbol, Translation FROM Roots WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new RootModel(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? "" : reader.GetString(2)
                );
            }

            throw new InvalidOperationException(
                $"Nie znaleziono rdzenia o Id {id}.");
        }



        public void delete(int Model_ID)
        {
            using var connection = _database.GetConnection();
            

            using var command = new SqliteCommand(
                "DELETE FROM Roots WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", Model_ID);
            command.ExecuteNonQuery();
        }


        public List<RootModel> readtable()
        {
            var items = new List<RootModel>();
            using var connection = _database.GetConnection();
            
            using var command = new SqliteCommand(
                "SELECT Id, Symbol, Translation FROM Roots;",
                connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new RootModel(
                    reader.GetInt32(0),    // Id
                    reader.GetString(1),   // Symbol
                     reader.IsDBNull(2) ? "" : reader.GetString(2)    // Translation
                ));
            }
            return items;
        }
        public void insert(RootModel model)
        {
            if (model is not RootModel rootModel)
                throw new ArgumentException("Nieprawidłowy typ modelu.");
            using var connection = _database.GetConnection();
            
            using var command = new SqliteCommand(
                @"INSERT INTO Roots (Symbol, Translation)
          VALUES (@Symbol, @Translation);",
                connection);
            command.Parameters.AddWithValue("@Symbol", rootModel.Symbol);
            command.Parameters.AddWithValue("@Translation", rootModel.Translation);
            command.ExecuteNonQuery();
        }

        public void update(RootModel model)
        {
            if (model is not RootModel rootModel)
                throw new ArgumentException("Nieprawidłowy typ modelu.");
            using var connection = _database.GetConnection();
            
            using var command = new SqliteCommand(
                @"UPDATE Roots
          SET Symbol = @Symbol, Translation = @Translation
          WHERE Id = @Id;",
                connection);
            command.Parameters.AddWithValue("@Symbol", rootModel.Symbol);
            command.Parameters.AddWithValue("@Translation", rootModel.Translation);
            command.Parameters.AddWithValue("@Id", rootModel.Id);
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
                throw new InvalidOperationException("Root not found.");
        }

    }
}
