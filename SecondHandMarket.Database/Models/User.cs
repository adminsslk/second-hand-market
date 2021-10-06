using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondHandMarket.Database
{
    public partial class User
    {
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
