using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    class PhoneticSystem
    {
        private readonly Database _database = new Database();
        private Dictionary<string , string> _choose_symbols;



        public PhoneticSystem(int LettersCount)
        {
            _choose_symbols = _database.ReadSymbols();

            Random random = new Random();
            using var connection = _database.GetConnection();

            if (_choose_symbols.Count > 0)
            {
                for(int i = 0; i < LettersCount; i++) 
                {
                    var item = _choose_symbols.ElementAt(
                    random.Next(_choose_symbols.Count));

                    string symbol = item.Key;
                    string category = item.Value;

                    using var command = new SqliteCommand(
                        "INSERT INTO AlphabetSybbols (Symbol, Category) " +
                        "VALUES (@symbol, @category)",
                        connection);

                    command.Parameters.AddWithValue("@symbol", symbol);
                    command.Parameters.AddWithValue("@category", category);

                    command.ExecuteNonQuery();
                }
                
            }
        }





    }
}
