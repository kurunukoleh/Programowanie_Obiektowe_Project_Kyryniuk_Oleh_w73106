using OOP_project.Data;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    public abstract class Morpheme
    {
        protected readonly Dictionary<string, string> _choose_alphabet;

        private PhoneticSystem _phonetic_system;

        protected Morpheme(Database database)
        {
            _phonetic_system = new PhoneticSystem(database);
            _choose_alphabet = _phonetic_system.ReadAlphabet();
        }

        protected string GetRandomSymbol()
        {
            if (_choose_alphabet.Count == 0)
                throw new InvalidOperationException("Alphabet is empty.");

            int index = Random.Shared.Next(_choose_alphabet.Count);

            return _choose_alphabet.Keys.ElementAt(index);
        }

        protected string GenerateMorpheme(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            StringBuilder morphemeText = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                morphemeText.Append(GetRandomSymbol());
            }

            return morphemeText.ToString();
        }
    }
}