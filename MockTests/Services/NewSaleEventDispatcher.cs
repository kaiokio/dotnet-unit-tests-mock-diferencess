using MockTests.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockTests.Services
{
    public class NewSaleEventDispatcher : INewSaleEventDispatcher
    {
        public void DispatchEvent(Sale sale)
        {
            Console.WriteLine("Event dispatched successfully");
        }
    }
}
