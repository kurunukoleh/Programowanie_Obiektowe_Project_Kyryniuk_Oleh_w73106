using OOP_project.Models;
using System;
using System.Collections.Generic;
using System.Text;




namespace OOP_project.Models
{
    public class WordDictionaryModel : Model
    {

        public string Word { get; set; }
        public string Translation { get; set; }
        //public int RootId { get; set; }
        public string CreatedAt { get; set; }
        public WordDictionaryModel(int id , string word , string translation ,  string createdAt)  : base(id)
        {
            Word = word;
            Translation = translation;
            CreatedAt = createdAt;
            
            
        }



       
    }
}
