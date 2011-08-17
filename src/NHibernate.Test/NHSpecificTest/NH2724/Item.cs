using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    public class Item
    {
        public Guid Id { get; set; }
        public Entity Entity { get; set; }
        public DoubledInteger Value1 { get; set; }
        public DoubledInteger Value2 { get; set; }
    }
}
