using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;
using System;
using System.Collections.Generic;

namespace OOP_project.Systems
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

        


        public List<WordModel> ReturnWordFormView(int rootId)
        {
            var forms = new List<WordModel>();
            var affixes = _affix_system.readtable();
            var root = _root_system.GetRootById(rootId);

            foreach (var affix in affixes)
            {

                var word = new Word(
                    root,
                    affix,
                    _database,
                    _affix_system,
                    _root_system);

                forms.Add(new WordModel
                {
                    Word = word.WordText,
                    RootTranslation = word.RootTranslation,
                    GrammaticalRole = word.GrammaticalRole
                });
            }

            return forms;
        }







        public List<WordModel> ReturnWordFormView(
            string text,
            string previousGrammaticalRole,
            string rootTranslation)
                {
                    var forms = new List<WordModel>();
                    var affixes = _affix_system.readtable();

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

                        forms.Add(new WordModel
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