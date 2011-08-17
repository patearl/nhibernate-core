using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NUnit.Framework;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Utilities;
using SharpTestsEx;

namespace NHibernate.Test.Linq
{
	public static class MyLinqExtensions
	{
		public static bool IsLike(this string source, string pattern)
		{
			pattern = Regex.Escape(pattern);
			pattern = pattern.Replace("%", ".*?").Replace("_", ".");
			pattern = pattern.Replace(@"\[", "[").Replace(@"\]","]").Replace(@"\^", "^");

			return Regex.IsMatch(source, pattern);
		}

        public static int Sum(this IQueryable<OrderLine> source)
        {
            return source.Provider.Execute<int>(Expression.Call((MethodInfo)MethodBase.GetCurrentMethod(), source.Expression));
        }

        public static int Sum(this IEnumerable<OrderLine> source)
        {
            return source.Sum(ol => ol.Quantity);
        }
	}

	public class MyLinqToHqlGeneratorsRegistry: DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry()
		{
			RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null)),
			                  new IsLikeGenerator());
            RegisterGenerator(new RuntimeGenerator());
        }
	}

    public class MyLinqNodeTypeProvider : NHibernateNodeTypeProvider
    {
        public MyLinqNodeTypeProvider()
        {
            Register(new[] { ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.Sum(null))}, typeof(MySumExpressionNode));
        }
    }

    public class MySumExpressionNode : SumExpressionNode
    {
        public MySumExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalSelector) : base(parseInfo, optionalSelector)
        {
        }

        protected override Remotion.Linq.Clauses.ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new MySumResultOperator();
        }
    }

    public class MySumResultOperator : SumResultOperator
    {
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new MySumResultOperator();
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            ArgumentUtility.CheckNotNull("input", input);
            var result = MyLinqExtensions.Sum(input.GetTypedSequence<OrderLine>());
            return new StreamedValue(result, (StreamedValueInfo)GetOutputDataInfo(input.DataInfo));
        }
    }

	public class IsLikeGenerator : BaseHqlGeneratorForMethod
	{
		public IsLikeGenerator()
		{
			SupportedMethods = new[] {ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsLike(null, null))};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, 
			ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
			                        visitor.Visit(arguments[1]).AsExpression());
		}
	}

    public class RuntimeGenerator : IRuntimeMethodHqlGenerator
    {
        public bool SupportsMethod(MethodInfo method)
        {
            return false;
        }

        public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomExtensionsExample : LinqTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		    configuration.LinqNodeTypeProvider<MyLinqNodeTypeProvider>();
		}

		[Test]
		public void CanUseMyCustomExtension()
		{
			var contacts = (from c in db.Customers where c.ContactName.IsLike("%Thomas%") select c).ToList();
			contacts.Count.Should().Be.GreaterThan(0);
			contacts.Select(customer => customer.ContactName).All(c => c.Satisfy(customer => customer.Contains("Thomas")));
		}

        [Test]
        [Ignore]
        public void CustomSum()
        {
            decimal sum = db.OrderLines.Sum();
            sum.Should().Be.GreaterThan(0);
        }
	}
}