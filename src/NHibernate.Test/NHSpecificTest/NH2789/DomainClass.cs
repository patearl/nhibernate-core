

namespace NHibernate.Test.NHSpecificTest.NH2789
{
    public class DomainClass
    {
        public virtual int Id { get; set; }
        public virtual byte? NullableByteProperty { get; set; }
        public virtual int? NullableIntProperty { get; set; }
    }
}
