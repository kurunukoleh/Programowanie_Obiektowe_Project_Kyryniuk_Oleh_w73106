using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Models
{
    public class RootModel : Model
    {

        public string Symbol { get; set; }

        public string Translation { get; set; }

        public RootModel(int id , string symbole, string translation ) : base(id)
        {
            Symbol = symbole;
            Translation = translation;
        }

    }
}
