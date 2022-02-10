using MockTests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockTests.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientDocument { get; set; }
        public int ProductId { get; set; }
        public decimal Value { get; set; }
        public bool Pending { get; set; }
    }
}
