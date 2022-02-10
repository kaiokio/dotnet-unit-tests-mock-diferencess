using MockTests.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        public void Add(Sale sale)
        {
            Console.WriteLine("Sale added successfully");
        }
    }
}
