using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    public class WordFormView
    {
        public string Word { get; set; }
        public string GrammaticalRole { get; set; }
        public string RootTranslation { get; set; }

        public WordFormView(
            string word,
            string grammaticalRole,
            string rootTranslation)
        {
            Word = word;
            GrammaticalRole = grammaticalRole;
            RootTranslation = rootTranslation;
        }
    }
}
