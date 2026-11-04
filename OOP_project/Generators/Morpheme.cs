using OOP_project.Data;
using OOP_project.Models;
using OOP_project.Systems;
using System.Text;

public abstract class Morpheme
{
    protected readonly List<AlphabetLetter> _choose_alphabet;

    protected Morpheme(Database database)
    {
        var phoneticSystem = new PhoneticSystem(database);
        _choose_alphabet = phoneticSystem.readtable();
    }

    protected string GetRandomSymbol()
    {
        if (_choose_alphabet.Count == 0)
            throw new InvalidOperationException("Alphabet is empty.");

        int index = Random.Shared.Next(_choose_alphabet.Count);

        return _choose_alphabet[index].Symbol;
    }

    protected virtual string GenerateMorpheme(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        var morphemeText = new StringBuilder();

        for (int i = 0; i < length; i++)
            morphemeText.Append(GetRandomSymbol());

        return morphemeText.ToString();
    }
}