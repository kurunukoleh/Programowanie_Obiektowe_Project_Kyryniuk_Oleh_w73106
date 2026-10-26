//using Microsoft.Data.Sqlite;
//using OOP_project.Data;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace OOP_project.Models
//{
//    internal class GranaticalMeaning
//    {

//        private readonly Database _database = new Database();
//        private List<string> _choose_roles;

//        private AffixSystem affixSystem;

//        public GranaticalMeaning(int RolesCount)
//        {
//            affixSystem = new AffixSystem(_database);
//            _choose_roles = affixSystem.ReadAffixeInformation(_database).GrammaticalRoleCode;

//            Random random = new Random();
//            using var connection = _database.GetConnection();

//            if (_choose_roles.Count > 0)
//            {


//            }
//        }
//    }
//}
