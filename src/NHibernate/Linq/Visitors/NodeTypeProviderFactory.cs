using System;
using System.Collections.Generic;

using NHibernate.Util;
using Remotion.Linq.Parsing.Structure;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Visitors
{
    internal sealed class NodeTypeProviderFactory
    {
        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(NodeTypeProviderFactory));

        public static INodeTypeProvider CreateNodeTypeProvider(IDictionary<string, string> properties)
        {
            string provider;
            if (properties.TryGetValue(Environment.LinqNodeTypeProvider, out provider))
            {
                try
                {
                    log.Info("Initializing NodeTypeProvider: " + provider);
                    return (INodeTypeProvider)Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(provider));
                }
                catch (Exception e)
                {
                    log.Fatal("Could not instantiate NodeTypeProvider", e);
                    throw new HibernateException("Could not instantiate NodeTypeProvider: " + provider, e);
                }
            }
            return new NHibernateNodeTypeProvider();
        }
    }
}