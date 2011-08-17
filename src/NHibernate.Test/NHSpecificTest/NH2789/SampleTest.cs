using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2789
{
    [TestFixture]
    public class SampleTest : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                DomainClass entity = new DomainClass();
                entity.Id = 1;
                entity.NullableByteProperty = 16;
                entity.NullableIntProperty = 1024;
                session.Save(entity);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = this.OpenSession())
            {
                string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        [Test]
        public void LinqQueryOnNullableByteShouldWork()
        {
            using (ISession session = this.OpenSession())
            {
                var entityQueryable = from x in session.Query<DomainClass>()
                                      where x.NullableByteProperty == Convert.ToByte(16)
                                      select x;

                Assert.IsTrue(entityQueryable.Count() == 1);
            }
        }

        [Test]
        public void LinqQueryOnNullableIntShouldWork()
        {
            using (ISession session = this.OpenSession())
            {
                var entityQueryable = from x in session.Query<DomainClass>()
                                      where x.NullableIntProperty == Convert.ToByte(2)
                                      select x;

                // fails
                //var personList = entityQueryable.ToList();

                Assert.IsTrue(entityQueryable.Count() == 0);
            }
        }


        [Test]
        public void DoSomething()
        {
            byte? nb = 3;
            byte b = 3;
            //Expression<Func<bool>> expression = () => nb == (byte?)Convert.ToByte(16);
            Expression<Func<bool>> expression = () => b == Convert.ToByte(16);
            var x = 3;
        }
    }
}
