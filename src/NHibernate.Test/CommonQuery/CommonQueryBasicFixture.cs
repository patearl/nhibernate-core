using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate.DomainModel;
using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.CommonQuery
{
    [TestFixture]
    public class CommonQueryBasicFixture : TestCase
    {
        protected override void OnSetUp()
        {
            using (ISession session = base.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var simple = new Simple();
                simple.Name = "test";
                simple.Count = 10;
                session.Save(simple, 1);

                simple = new Simple();
                simple.Name = "test2";
                simple.Count = 20;
                session.Save(simple, 2);

                transaction.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = base.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete("from System.Object");
                transaction.Commit();
            }
        }

        [Test]
        public void Test()
        {
            using (ISession session = base.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var items = session.CreateCriteria<Simple>().List<Simple>();
                Assert.AreEqual(2, items.Count);
                Assert.AreEqual("test", items.OrderBy(i => i.Name).First().Name);
                Assert.AreEqual("test2", items.OrderBy(i => i.Name).Last().Name);
                transaction.Commit();
            }
        }

        [Test]
        public void Eq()
        {
            using (ISession session = base.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var items = session.CreateCriteria<Simple>().Add(Restrictions.Eq("Name", "test")).List<Simple>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("test", items[0].Name);
                transaction.Commit();
            }
        }

        #region Overrides of TestCase

        protected override IList Mappings
        {
            get { return new[] {"Simple.hbm.xml"}; }
        }

        #endregion
    }
}
