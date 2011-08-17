using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class VariableTests : LinqTestCase
	{
		[Test]
		[Ignore]
		public void ReusingSameVariable()
		{
			int foo = 3;
			Assert.AreEqual(3, db.Customers.Select(c => new { c.ContactName, foo }).First().foo);
			foo = 5;
			Assert.AreEqual(5, db.Customers.Select(c => new { c.ContactName, foo }).First().foo);
		}
	}
}
