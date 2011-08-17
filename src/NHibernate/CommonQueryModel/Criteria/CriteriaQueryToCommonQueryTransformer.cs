using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.CommonQueryModel.Ast;
using NHibernate.Impl;

namespace NHibernate.CommonQueryModel.Criteria
{
    public class CriteriaQueryToCommonQueryTransformer
    {
        public CommonQuery Transform(CriteriaImpl criteria)
        {
            return new CommonQuery();
        }
    }
}
