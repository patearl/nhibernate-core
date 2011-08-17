using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Utilities;

namespace NHibernate.Test.NHSpecificTest.NH2724
{
    public static class LinqExtensions
    {
        public static DoubledInteger Sum(this IQueryable<DoubledInteger> source)
        {
            return source.Provider.Execute<DoubledInteger>(Expression.Call((MethodInfo)MethodBase.GetCurrentMethod(), source.Expression));
        }

        public static DoubledInteger Sum(this IEnumerable<DoubledInteger> source)
        {
            return new DoubledInteger(source.Sum(d => d.DoubledValue)/2);
        }
    }

    public class MyLinqNodeTypeProvider : NHibernateNodeTypeProvider
    {
        public MyLinqNodeTypeProvider()
        {
            Register(new[] { ReflectionHelper.GetMethodDefinition(() => LinqExtensions.Sum(null))}, typeof(MySumExpressionNode));
        }
    }

    public class MySumExpressionNode : SumExpressionNode
    {
        public MySumExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalSelector) : base(parseInfo, optionalSelector)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
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
            var result = input.GetTypedSequence<DoubledInteger>().Sum();
            return new StreamedValue(result, (StreamedValueInfo)GetOutputDataInfo(input.DataInfo));
        }
    }

    public class MyMethodRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public MyMethodRegistry()
        {
            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => LinqExtensions.Sum((IQueryable<DoubledInteger>)null)), new SumGenerator());
            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => LinqExtensions.Sum((IEnumerable<DoubledInteger>)null)), new SumGenerator());
        }
    }

    public class SumGenerator : BaseHqlGeneratorForMethod
    {
        public SumGenerator()
        {
            SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition(() => LinqExtensions.Sum(null)), ReflectionHelper.GetMethodDefinition(() => LinqExtensions.Sum((IEnumerable<DoubledInteger>)null)) };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            //var castElements = treeBuilder.Cast(, typeof (int));
            return treeBuilder.Sum(visitor.Visit(arguments[0]).AsExpression());
        }
    }
}
