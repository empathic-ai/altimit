#define ALTIMIT_LITEDB

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Linq.Expressions;
using Altimit.Serialization;
using Altimit;
using System.Threading;
using LiteDB;

namespace Altimit
{
    public static class QueryExtensions
    {
        public static object GetLocalProperty(Type type, string propertyName, object propertyValue)
        {
            APropertyInfo propertyInfo;
            if (type.TryGetPropertyInfo(propertyName, out propertyInfo) && propertyInfo.PropertyType.IsInstanceType())
            {
                return propertyValue.GetInstanceID();
            }
            return propertyValue;
        }
    }

    public abstract class QueryNode
    {
        public abstract Func<object, bool> Compile();
#if ALTIMIT_LITEDB
        public abstract BsonExpression CompileToLocalBson();
#endif
    }

    public class ContainsQueryNode<T> : QueryNode
    {
        string propertyName;
        string value;

        public ContainsQueryNode(string propertyName, string value)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName).ToString().Contains(value);
        }

#if ALTIMIT_LITEDB
        public override BsonExpression CompileToLocalBson()
        {
            return Query.Contains(propertyName, value);
        }
#endif
    }

    public class NotQueryNode<T> : QueryNode
    {
        string propertyName;
        object propertyValue;

        public NotQueryNode(string fieldName, object value)
        {
            this.propertyName = fieldName;
            this.propertyValue = value;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName) != propertyValue;
        }

#if ALTIMIT_LITEDB
        public override BsonExpression CompileToLocalBson()
        {
            return Query.Not(propertyName, new BsonValue(QueryExtensions.GetLocalProperty(typeof(T), propertyName, propertyValue)));
        }
#endif
    }

    public class EQQueryNode<T> : QueryNode
    {
        string propertyName;
        object propertyValue;

        public EQQueryNode(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName) == propertyValue;
        }
#if ALTIMIT_LITEDB
        public override BsonExpression CompileToLocalBson()
        {
            return Query.EQ(propertyName, new BsonValue(QueryExtensions.GetLocalProperty(typeof(T), propertyName, propertyValue)));
        }
#endif
    }

    public class AndQueryNode : QueryNode
    {
        QueryNode[] nodes;

        public AndQueryNode(params QueryNode[] nodes)
        {
            this.nodes = nodes;
        }
        public override Func<object, bool> Compile()
        {
            return x => {
                foreach (var node in nodes)
                {
                    if (!node.Compile()(x))
                        return false;
                }
                return true;
            };
        }
#if ALTIMIT_LITEDB
        public override BsonExpression CompileToLocalBson()
        {
            return Query.And(nodes.Select(x => x.CompileToLocalBson()).ToArray());
        }
#endif
    }

    public class QueryBuilder<TSource>
    {
        public QueryNode Contains(Expression<Func<TSource, string>> propExp, string propertyValue)
        {
            if (String.IsNullOrWhiteSpace(propertyValue))
                return new NotQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, null);

            return new ContainsQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode Not<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty propertyValue)
        {
            return new NotQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode EQ<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty propertyValue)
        {
            return new EQQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode And(params QueryNode[] nodes)
        {
            return new AndQueryNode(nodes);
        }
    }
}

#if LEGACY
using LiteDB;
namespace Altimit
{
    public static class QueryExtensions
    {
        public static object GetLocalProperty(Type type, string propertyName, object propertyValue)
        {
            APropertyInfo propertyInfo;
            if (type.TryGetPropertyInfo(propertyName, out propertyInfo) && propertyInfo.PropertyType.IsInstance())
            {
                return propertyValue.GetInstanceID().Value;
            }
            return propertyValue;
        }
    }

    public abstract class QueryNode
    {
        public abstract Func<object, bool> Compile();
        public abstract BsonExpression CompileToLocalBson();
    }

    public class ContainsQueryNode<T> : QueryNode
    {
        string propertyName;
        string value;

        public ContainsQueryNode(string propertyName, string value)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName).ToString().Contains(value);
        }

        public override BsonExpression CompileToLocalBson()
        {
            return Query.Contains(propertyName, value);
        }
    }

    public class NotQueryNode<T> : QueryNode
    {
        string propertyName;
        object propertyValue;

        public NotQueryNode(string fieldName, object value)
        {
            this.propertyName = fieldName;
            this.propertyValue = value;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName) != propertyValue;
        }

        public override BsonExpression CompileToLocalBson()
        {
            return Query.Not(propertyName, new BsonValue(QueryExtensions.GetLocalProperty(typeof(T), propertyName, propertyValue)));
        }
    }

    public class EQQueryNode<T> : QueryNode
    {
        string propertyName;
        object propertyValue;

        public EQQueryNode(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
        }

        public override Func<object, bool> Compile()
        {
            return x => x.GetProperty(propertyName) == propertyValue;
        }

        public override BsonExpression CompileToLocalBson()
        {
            return Query.EQ(propertyName, new BsonValue(QueryExtensions.GetLocalProperty(typeof(T), propertyName, propertyValue)));
        }
    }

    public class AndQueryNode : QueryNode
    {
        QueryNode[] nodes;

        public AndQueryNode(params QueryNode[] nodes)
        {
            this.nodes = nodes;
        }
        public override Func<object, bool> Compile()
        {
            return x => {
                foreach (var node in nodes)
                {
                    if (!node.Compile()(x))
                        return false;
                }
                return true;
            };
        }

        public override BsonExpression CompileToLocalBson()
        {
            return Query.And(nodes.Select(x => x.CompileToLocalBson()).ToArray());
        }
    }

    public class QueryBuilder<TSource>
    {
        public QueryNode Contains(Expression<Func<TSource, string>> propExp, string propertyValue)
        {
            if (String.IsNullOrWhiteSpace(propertyValue))
                return new NotQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, null);

            return new ContainsQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode Not<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty propertyValue)
        {
            return new NotQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode EQ<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty propertyValue)
        {
            return new EQQueryNode<TSource>(typeof(TSource).GetPropertyInfo(propExp).Name, propertyValue);
        }

        public QueryNode And(params QueryNode[] nodes)
        {
            return new AndQueryNode(nodes);
        }
    }


}
#endif