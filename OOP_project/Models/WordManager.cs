using OOP_project.Data;
using System;
using System.Collections.Generic;

namespace OOP_project.Models
{
    public class WordManager
    {
        private readonly Database _database;
        private readonly Random _random = new Random();

        public WordManager(Database database)
        {
            _database = database;
        }

        public List<string> ReturnWordList()
        {
            var forms = new List<string>();

            var affixes = _database.ReadAffixesWithTypes();
            var roots = _database.ReadRoots();

            if (roots.Count == 0)
                return forms;

            string root = roots[_random.Next(roots.Count)];

            for (int i = 0; i < affixes.Count; i++)
            {
                var word = new Word(i, root);
                forms.Add(word.ReturnWord());
            }

            return forms;
        }
    }
}