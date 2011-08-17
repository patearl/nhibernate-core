using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;

namespace NHibernate.Linq.Visitors
{
    internal class QueryParserFactory
    {
        public static QueryParser CreateQueryParser(IDictionary<string, string> properties)
        {
            var nodeTypeProvider = NodeTypeProviderFactory.CreateNodeTypeProvider(properties);

			var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();

			var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);
			// Add custom processors here:
			// processor.InnerProcessors.Add (new MyExpressionTreeProcessor());

			var expressionTreeParser = new ExpressionTreeParser(nodeTypeProvider, processor);

			return new QueryParser(expressionTreeParser);			
        }
    }
}
