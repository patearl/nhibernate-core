using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void AddMappings(Configuration configuration)
        {
            var mapper = new ModelMapper();
            
            mapper.Class<Entity>(m =>
            {
                m.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
                m.Bag(x => x.Items, map =>
                                        {
                                            map.Cascade(Mapping.ByCode.Cascade.All);
                                            map.Inverse(true);
                                        },
                                        r => r.OneToMany());
                m.Lazy(false);
            });
            mapper.Class<Item>(m =>
            {
                m.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
                m.Property(x => x.Value1, map => map.Type(NHibernateUtil.Custom(typeof(DoubledIntegerType))));
                m.Property(x => x.Value2, map => map.Type(NHibernateUtil.Custom(typeof(DoubledIntegerType))));
                m.ManyToOne(x => x.Entity);
                m.Lazy(false);
            });

            var mapping = mapper.CompileMappingFor(new[] {typeof (Entity), typeof(Item)});

            configuration.AddDeserializedMapping(mapping, "NH2724Mapping");

            configuration.LinqNodeTypeProvider<MyLinqNodeTypeProvider>();
            configuration.LinqToHqlGeneratorsRegistry<MyMethodRegistry>();
        }

        [Test]
        public void CustomSumMethod()
        {
            using (ISession session = OpenSession())
            {
                Entity e1 = new Entity();
                e1.Items.Add(new Item { Value1 = new DoubledInteger(1), Value2 = new DoubledInteger(2), Entity = e1 });
                e1.Items.Add(new Item { Value1 = new DoubledInteger(4), Value2 = new DoubledInteger(5), Entity = e1 });
                e1.Items.Add(new Item { Value1 = new DoubledInteger(7), Value2 = new DoubledInteger(8), Entity = e1 });

                Entity e2 = new Entity();
                e2.Items.Add(new Item { Value1 = new DoubledInteger(2), Value2 = new DoubledInteger(1), Entity = e2 });
                e2.Items.Add(new Item { Value1 = new DoubledInteger(5), Value2 = new DoubledInteger(4), Entity = e2 });
                e2.Items.Add(new Item { Value1 = new DoubledInteger(8), Value2 = new DoubledInteger(7), Entity = e2 });

                Entity e3 = new Entity();
                e3.Items.Add(new Item { Value1 = new DoubledInteger(3), Value2 = new DoubledInteger(3), Entity = e3 });
                e3.Items.Add(new Item { Value1 = new DoubledInteger(6), Value2 = new DoubledInteger(6), Entity = e3 });
                e3.Items.Add(new Item { Value1 = new DoubledInteger(9), Value2 = new DoubledInteger(9), Entity = e3 });

                session.Save(e1);
                session.Save(e2);
                session.Save(e3);
                session.Flush();
            }

            using (ISession session = OpenSession())
            {
                var entities = session.Query<Entity>().ToList();
                Assert.AreEqual(3, entities.Count);
                CollectionAssert.AreEquivalent(new[] {12,15,18}, entities.Select(entity => entity.Items.Sum(item => item.Value1.DoubledValue/2)).ToArray());

                var check = session.Query<Entity>().Where(e => e.Items.Select(i => i.Value1).Sum() == e.Items.Select(i => i.Value2).Sum()).ToList();
                Assert.AreEqual(1, check.Count);
                Assert.AreEqual(36, check[0].Items.Sum(i => i.Value1.DoubledValue));

                //IQuery hqlQuery = session.CreateQuery("select sum(e.DoubledInteger) from Entity e");
                //hqlQuery.SetResultTransformer(new MyResultTransformer());
                //Assert.AreEqual(new DoubledInteger(18), hqlQuery.UniqueResult());

                //Assert.AreEqual(new DoubledInteger(18), session.Query<Entity>());
            }

            using (ISession session = OpenSession())
            {
                session.Delete("from Entity");
                session.Flush();
            }
        }
    }

    public class MyResultTransformer : IResultTransformer
    {
        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return new DoubledInteger(Convert.ToInt32(tuple[0]));
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }
    }
}
