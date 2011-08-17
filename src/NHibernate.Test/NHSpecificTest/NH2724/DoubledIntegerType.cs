using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    class DoubledIntegerType : IUserType
    {
        public SqlType[] SqlTypes
        {
            get { return new[] { SqlTypeFactory.Int64 }; }
        }

        public System.Type ReturnedType
        {
            get { return typeof (DoubledInteger); }
        }

        bool IUserType.Equals(object x, object y)
        {
            return Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            return new DoubledInteger(Convert.ToInt32(NHibernateUtil.Int64.NullSafeGet(rs, names) ?? 0));
        }


        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            NHibernateUtil.Int64.NullSafeSet(cmd, ((DoubledInteger)value).undoubledValue, index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }
    }
}
