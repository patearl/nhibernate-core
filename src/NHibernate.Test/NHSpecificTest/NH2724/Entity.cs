using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    public class Entity
    {
        public Entity()
        {
            Items = new List<Item>();
        }

        public Guid Id { get; set; }

        public IList<Item> Items { get; set; }
    }
}
