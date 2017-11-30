using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_SearchApp
{
    class Customer
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string FirstName_LastName { get; set; }
        public string TypeOfBattery { get; set; }
        public string DateOfPurchase { get; set; }
    }
}
