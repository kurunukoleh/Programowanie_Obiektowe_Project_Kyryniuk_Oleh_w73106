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
    }
}