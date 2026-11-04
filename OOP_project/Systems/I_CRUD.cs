using OOP_project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_project.Systems
{
    internal interface I_CRUD<T> where T : Model
    {
        void delete(int Model_ID);
        List<T> readtable();
        void insert(T model);
        void update(T model);
    }
}
