
using Microsoft.Data.Sqlite;
using OOP_project.Data;
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
            int affixNumber,
            int rootId,
            Database database,
            AffixSystem affixSystem,
            RootSystem rootSystem)
        {
            _database = database;
            _affixSystem = affixSystem;
            _rootSystem = rootSystem;
            _rootId = rootId;

            GenerateWord(affixNumber, rootId);
        }

        public Word(
            int affixNumber,
            Database database,
            AffixSystem affixSystem,
            RootSystem rootSystem)
        {
            _database = database;
            _affixSystem = affixSystem;
            _rootSystem = rootSystem;

            var roots = _rootSystem.ReadRoots();

            if (roots.Count == 0)
                throw new InvalidOperationException("Brak korzeni w bazie danych.");

            var root = roots.ElementAt(Random.Shared.Next(roots.Count));

            _rootId = root.Key;

            GenerateWord(affixNumber, _rootId);
        }

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

                    var affixes = _affixSystem.ReadAffixeInformation();

                    if (affixIndex < 0 || affixIndex >= affixes.Count)
                        throw new ArgumentOutOfRangeException(nameof(affixIndex));

                    var selectedAffix = affixes[affixIndex];

                    if (selectedAffix.Type == "Prefix")
                        _wordText.Append(selectedAffix.Affix).Append(text);
                    else if (selectedAffix.Type == "Suffix")
                        _wordText.Append(text).Append(selectedAffix.Affix);
                    else
                        throw new NotSupportedException(
                            $"Nieobsługiwany typ afiksu: {selectedAffix.Type}");

                    GrammaticalRole = string.IsNullOrWhiteSpace(previousGrammaticalRole)
                        ? selectedAffix.GrammaticalRoleCode
                        : $"{previousGrammaticalRole}, {selectedAffix.GrammaticalRoleCode}";

                    RootTranslation = rootTranslation;
                }

        private void GenerateWord(int affixNumber, int rootId)
        {
            var affixes = _affixSystem.ReadAffixeInformation();
            var roots = _rootSystem.ReadRoots();

            if (!roots.TryGetValue(rootId, out string? root))
                throw new ArgumentException(
                    "Nie znaleziono korzenia o podanym ID.",
                    nameof(rootId));

            if (affixNumber < 0 || affixNumber >= affixes.Count)
                throw new ArgumentOutOfRangeException(
                    nameof(affixNumber),
                    "Nieprawidłowy indeks afiksu.");

            var selectedAffix = affixes[affixNumber];
            GrammaticalRole = selectedAffix.GrammaticalRoleCode;

            if (selectedAffix.Type == "Prefix")
            {
                _wordText.Append(selectedAffix.Affix);
                _wordText.Append(root);
            }
            else if (selectedAffix.Type == "Suffix")
            {
                _wordText.Append(root);
                _wordText.Append(selectedAffix.Affix);
            }
            else
            {
                throw new NotSupportedException(
                    $"Nieobsługiwany typ afiksu: {selectedAffix.Type}");
            }
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
