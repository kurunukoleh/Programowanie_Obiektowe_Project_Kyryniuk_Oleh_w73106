
using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOP_project.Models
{
    internal class Word
    {
        private readonly Database _database;
        private readonly AffixSystem _affixSystem;
        private readonly RootSystem _rootSystem;

        public int RootId => _rootId;
        public string WordText => _wordText.ToString();
        public string GrammaticalRole { get; private set; } = "";
        public string RootTranslation { get; private set; } = "";

        private readonly StringBuilder _wordText = new StringBuilder();
        private readonly int _rootId;

        
        public Word(
            string text,
            int affixIndex,
            string previousGrammaticalRole,
            string rootTranslation,
            Database database,
            AffixSystem affixSystem,
            RootSystem rootSystem)
                {
                    _database = database;
                    _affixSystem = affixSystem;
                    _rootSystem = rootSystem;

                    

            var affixes = _affixSystem.readtable();

                    if (affixIndex < 0 || affixIndex >= affixes.Count)
                        throw new ArgumentOutOfRangeException(nameof(affixIndex));

                    var selectedAffix = affixes[affixIndex];

                    if (selectedAffix.getType() == "Prefix")
                        _wordText.Append(selectedAffix.Affix).Append(text);
                    else if (selectedAffix.getType() == "Suffix")
                        _wordText.Append(text).Append(selectedAffix.Affix);
                    else
                        throw new NotSupportedException(
                            $"Nieobsługiwany typ afiksu: {selectedAffix.AffixTypeId}");

                    GrammaticalRole = string.IsNullOrWhiteSpace(previousGrammaticalRole)
                        ? selectedAffix.GrammaticalRoleCode
                        : $"{previousGrammaticalRole}, {selectedAffix.GrammaticalRoleCode}";

                    RootTranslation = rootTranslation;
                }

        public Word(
    RootModel rootModel,
    AffixModel affixModel,
    Database database,
    AffixSystem affixSystem,
    RootSystem rootSystem)
        {
            if (rootModel == null)
                throw new ArgumentNullException(nameof(rootModel));

            if (affixModel == null)
                throw new ArgumentNullException(nameof(affixModel));

            _database = database;
            _affixSystem = affixSystem;
            _rootSystem = rootSystem;
            _rootId = rootModel.Id;

            if (affixModel.getType() == "Prefix")
                _wordText.Append(affixModel.Affix).Append(rootModel.Symbol);
            else if (affixModel.getType() == "Suffix")
                _wordText.Append(rootModel.Symbol).Append(affixModel.Affix);
            else
                throw new NotSupportedException(
                    $"Nieobsługiwany typ afiksu: {affixModel.AffixTypeId}");

            GrammaticalRole = affixModel.GrammaticalRoleCode;
            RootTranslation = rootModel.Translation;
        }




        public string ReturnWord()
        {
            return _wordText.ToString();
        }

        public string ReadRootTranslation(int rootId)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT Translation FROM Roots WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", rootId);

            return command.ExecuteScalar()?.ToString() ?? "";
        }



        public void AddWordToDictionary(string translation)
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"INSERT INTO Dictionary
                    (Word, Translation, RootId)
                  VALUES
                    (@Word, @Translation, @RootId);",
                connection);

            command.Parameters.AddWithValue("@Word", ReturnWord());
            command.Parameters.AddWithValue("@Translation", translation);
            command.Parameters.AddWithValue("@RootId", _rootId);

            command.ExecuteNonQuery();
        }
    }
}
