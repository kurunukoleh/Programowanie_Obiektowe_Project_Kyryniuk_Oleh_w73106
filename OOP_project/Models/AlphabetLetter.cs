using System;
using System.Collections.Generic;
using System.Text;




namespace OOP_project.Models
{
    public class AlphabetLetter : Model
    {
        public string Symbol { get; set; } = "";
        public string Category { get; set; } = "";


        public AlphabetLetter(int id , string symbol, string category) : base(id)
        {
            Symbol = symbol;
            Category = category;
        }

    }

    
}
