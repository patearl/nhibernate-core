using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.CommonQueryModel.Loader
{
    class CommonQueryLoader : BasicLoader
    {
        private IEntityPersister persister;

        public CommonQueryLoader(ISessionFactoryImplementor factory, string rootEntityName) : base(factory)
        {
            persister = Factory.GetEntityPersister(rootEntityName);

            PostInstantiate();
        }

        #region Overrides of Loader

        public override SqlString SqlString
        {
            get
            {
                IOuterJoinLoadable outerJoinLoadable = (IOuterJoinLoadable) persister;
                string columns = outerJoinLoadable.SelectFragment("alias", "suffix");
                return new SqlString("select " + columns + " from " + outerJoinLoadable.FromTableFragment("alias"));
            }
        }

        public override ILoadable[] EntityPersisters
        {
            get { return new[] {(ILoadable)persister}; }
        }

        public override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
        {
            return new[] { LockMode.None };
        }

        protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
        {
            return Enumerable.Empty<IParameterSpecification>();
        }

        #endregion

        #region Overrides of BasicLoader

        protected override string[] Suffixes
        {
            get { return new[] {"suffix"}; }
        }

        protected override string[] CollectionSuffixes
        {
            get { return null; }
        }

        #endregion

        public IList List(ISessionImplementor session)
        {
            return List(session, new QueryParameters(), null, null);
        }

        protected override object GetResultColumnOrRow(object[] row, Transform.IResultTransformer resultTransformer, System.Data.IDataReader rs, ISessionImplementor session)
        {
            return row[0];
        }
    }
}
