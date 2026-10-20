using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Data
{

    public record SymbolCandidate(string Symbol, string Category);
    internal class SymbolLibrary
    {
        public static readonly List<SymbolCandidate> Symbols =
    [
        
        new("a", "Vowel"),
        new("e", "Vowel"),
        new("i", "Vowel"),
        new("o", "Vowel"),
        new("u", "Vowel"),
        new("y", "Vowel"),
        new("ø", "Vowel"),
        new("æ", "Vowel"),
        new("ə", "Vowel"),

        
        new("p", "Consonant"),
        new("b", "Consonant"),
        new("t", "Consonant"),
        new("d", "Consonant"),
        new("k", "Consonant"),
        new("g", "Consonant"),
        new("m", "Consonant"),
        new("n", "Consonant"),
        new("r", "Consonant"),
        new("l", "Consonant"),
        new("ʃ", "Consonant"),
        new("ʒ", "Consonant"),
        new("ŋ", "Consonant"),
        new("θ", "Consonant"),
        new("x", "Consonant"),
        new("ɣ", "Consonant")
    ];
    }
}
