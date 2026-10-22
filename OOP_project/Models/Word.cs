using OOP_project.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    internal class Word
    {
        protected readonly Database _database = new Database();

        private readonly StringBuilder wordText = new StringBuilder();

        private Random random = new Random();

        public Word(int affixNumber , string root)
        {
            var affixes = _database.ReadAffixesWithTypes();

            if (affixNumber < 0 || affixNumber >= affixes.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(affixNumber),
                    "Nieprawidłowy numer afiksu.");
            }

            var selectedAffix = affixes[affixNumber];

            if (selectedAffix.Type == "Prefix")
            {
                wordText.Append(selectedAffix.Affix);
                wordText.Append(root);
            }
            else if (selectedAffix.Type == "Suffix")
            {
                wordText.Append(root);
                wordText.Append(selectedAffix.Affix);
            }
            else
            {
                throw new NotSupportedException(
                    $"Nieobsługiwany typ afiksu: {selectedAffix.Type}");
            }
        }

        public Word(int affixNumber)
        {
            var affixes = _database.ReadAffixesWithTypes();
            var roots = _database.ReadRoots();
            string root = roots[random.Next(0, roots.Count)]; 

            if (affixNumber < 0 || affixNumber >= affixes.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(affixNumber),
                    "Nieprawidłowy numer afiksu.");
            }

            var selectedAffix = affixes[affixNumber];

            if (selectedAffix.Type == "Prefix")
            {
                wordText.Append(selectedAffix.Affix);
                wordText.Append(root);
            }
            else if (selectedAffix.Type == "Suffix")
            {
                wordText.Append(root);
                wordText.Append(selectedAffix.Affix);
            }
            else
            {
                throw new NotSupportedException(
                    $"Nieobsługiwany typ afiksu: {selectedAffix.Type}");
            }
        }

        public string ReturnWord()
        {
            return wordText.ToString();
        }

    }
}