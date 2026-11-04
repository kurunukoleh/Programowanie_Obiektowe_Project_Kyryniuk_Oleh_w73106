using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;




namespace OOP_project.Models
{
    public class AffixModel : Model
    {


        private string affix_type_name { get; set; }

        public string Affix { get; set; }

        public int PartOfSpeechId {  get; set; }

        public string GrammaticalRoleCode { get; set; }

        public int AffixTypeId { get; set; }

        



        public AffixModel (int id , string affix, int partOfSpeechId, string grammaticalRoleCode, int affixTypeId) : base(id)
        {
            Affix = affix;
            PartOfSpeechId = partOfSpeechId;
            GrammaticalRoleCode = grammaticalRoleCode;
            AffixTypeId = affixTypeId  ;


            using var connection = _database.GetConnection();

            using var command = new SqliteCommand(
                """
                SELECT a.Name
                FROM AffixType a
                JOIN Affixes t ON a.Id = t.AffixTypeId
                WHERE t.Id = @id;
                """,
                connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            affix_type_name = reader.Read()
                ? reader.GetString(0)
                : throw new InvalidOperationException(
                    "Nie znaleziono typu afiksu.");
        }

        public string getType()
        {
            return affix_type_name;
        }

    }
}
