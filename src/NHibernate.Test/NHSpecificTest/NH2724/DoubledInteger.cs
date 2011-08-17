using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    [Serializable]
    public class DoubledInteger
    {
        public int DoubledValue
        {
            get { return undoubledValue * 2; }
        }

        internal int undoubledValue;

        public DoubledInteger(int undoubledValue)
        {
            this.undoubledValue = undoubledValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return undoubledValue == ((DoubledInteger)obj).undoubledValue;
        }

        public override int GetHashCode()
        {
            return undoubledValue.GetHashCode();
        }
    }
}
