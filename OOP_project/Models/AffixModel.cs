using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;




namespace OOP_project.Models
{
    public class AffixModel : Model
    {
        public string Affix { get; set; }
        public int PartOfSpeechId { get; set; }
        public string GrammaticalRoleCode { get; set; }
        public int AffixTypeId { get; set; }
        public string AffixTypeName { get; set; }

        public AffixModel(
            int id,
            string affix,
            int partOfSpeechId,
            string grammaticalRoleCode,
            int affixTypeId,
            string affixTypeName) : base(id)
        {
            Affix = affix;
            PartOfSpeechId = partOfSpeechId;
            GrammaticalRoleCode = grammaticalRoleCode;
            AffixTypeId = affixTypeId;
            AffixTypeName = affixTypeName;
        }
    }

}
