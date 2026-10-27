using Microsoft.Data.Sqlite;
using OOP_project.Data;
using System;
using System.Collections.Generic;

namespace OOP_project.Models
{
    public class WordManager
    {
        private readonly Database _database;

        private AffixSystem _affix_system;

        private  RootSystem _root_system;
        private readonly Random _random = new Random();

        public WordManager(Database database, AffixSystem affixSystem, RootSystem rootSystem)
        {
            _database = database;
            _affix_system = affixSystem;
            _root_system = rootSystem;
        }

        public List<string> ReturnWordList(int rootId)
        {
            var forms = new List<string>();
            var affixes = _affix_system.ReadAffixeInformation();

            foreach (var affix in affixes)
            {
                int index = affixes.IndexOf(affix);

                var word = new Word(
                    index,
                    rootId,
                    _database,
                    _affix_system,
                    _root_system);

                forms.Add(word.ReturnWord());
            }

            return forms;
        }


        public List<WordFormView> ReturnWordFormView(int rootId)
        {
            var forms = new List<WordFormView>();
            var affixes = _affix_system.ReadAffixeInformation();

            for (int i = 0; i < affixes.Count; i++)
            {
                var word = new Word(
                    i, rootId, _database, _affix_system, _root_system);

                forms.Add(new WordFormView
                {
                    Word = word.WordText,
                    RootTranslation = word.ReadRootTranslation(rootId),
                    GrammaticalRole = word.GrammaticalRole
                });
            }

            return forms;
        }

        public List<WordFormView> ReturnWordFormViewText(
            string text,
            string previousGrammaticalRole,
            string rootTranslation)
                {
                    var forms = new List<WordFormView>();
                    var affixes = _affix_system.ReadAffixeInformation();

                    for (int i = 0; i < affixes.Count; i++)
                    {
                        var word = new Word(
                            text,
                            i,
                            previousGrammaticalRole,
                            rootTranslation,
                            _database,
                            _affix_system,
                            _root_system);

                        forms.Add(new WordFormView
                        {
                            Word = word.WordText,
                            RootTranslation = word.RootTranslation,
                            GrammaticalRole = word.GrammaticalRole
                        });
                    }

                    return forms;
                }

        public void SaveWordToDictionary(
        string word,
        string translation,
        int? rootId)
            {
                using var connection = _database.GetConnection();
                connection.Open();

                using var command = new SqliteCommand(
                    @"INSERT INTO Dictionary (Word, Translation, RootId)
              VALUES (@Word, @Translation, @RootId);",
                    connection);

                command.Parameters.AddWithValue("@Word", word);
                command.Parameters.AddWithValue("@Translation", translation);

                command.Parameters.AddWithValue(
                    "@RootId",
                    rootId.HasValue ? rootId.Value : DBNull.Value);

                command.ExecuteNonQuery();
            }


    }
}