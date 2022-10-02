using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using LiteDB;
using System.Linq.Expressions;
using Altimit.Serialization;
using Altimit;
using System.Threading;

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
        /*
        public static BsonExpression Contains<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty propertyValue)
        {
            if (typeof(TProperty) == typeof(string) && String.IsNullOrWhiteSpace(propertyValue.ToString()))
                return Query.Not("_id", null);

            return Query.Contains(typeof(TSource).GetPropertyInfo(propExp).Name, new BsonValue(propertyValue));
        }

        public static BsonExpression EQ<TProperty>(Expression<Func<TSource, TProperty>> propExp, Guid value) where TProperty : class
        {
            return Query.EQ(typeof(TSource).GetPropertyInfo(propExp).Name, new BsonValue(value));
        }

        public static BsonExpression EQ<TProperty>(Expression<Func<TSource, TProperty>> propExp, TProperty value)
        {
            object propertyValue;
            if (typeof(TProperty).IsInstanceType())
            {
                propertyValue = value.GetInstanceID();
            } else
            {
                propertyValue = value;
            }
            return Query.EQ(typeof(TSource).GetPropertyInfo(propExp).Name, new BsonValue(propertyValue));
        }*/
    }

    public static class LocalDatabaseExtensions
    {
        public static ILiteCollection<BsonDocument> GetTypeCollection(this LiteDatabase db, Type type)
        {
            return db.GetCollection(GetTypeCollectionName(type));
        }
        public static string GetTypeCollectionName(Type type)
        {
            return type.GetFullName().Replace(".", "_").Replace(",", "_").Replace("[", "_$").Replace("]", "$_");
        }
    }

    public partial class LocalInstanceDatabase : IDisposable
    {
        ILogger logger;
        LiteDatabase db;
        AID TypesByInstanceIDID => new AID("56018309-8402-4e00-b6ce-96601ecd888f");
        ADictionary<AID, Type> TypesByInstanceID;

        public LocalInstanceDatabase()
        {

        }

        public async Task Connect(string dbPath, ILogger logger, bool reset = false)
        {
            db = new LiteDatabase(dbPath);
            logger.Log($"Connected to local database at path {dbPath}.");
            if (reset)
                Clear();

            TypesByInstanceID = await FindOrAddOne<ADictionary<AID, Type>>(TypesByInstanceIDID);
        }

        public async Task<IEnumerable<TSource>> Where<TSource, TProperty>(Expression<Func<TSource, TProperty>> propExp, Func<TProperty, bool> predicate) where TSource : class
        {
            return null;
        }

        public async Task<IEnumerable<TSource>> FindOne<TSource>(QueryBuilder<TSource> query) where TSource : class
        {
            return null;
        }

        async void OnInstanceAdded(AID instanceID, object instance)
        {
            try
            {
                if (TypesByInstanceID != null)
                {
                    TypesByInstanceID.Add(instanceID, instance.GetType());
                    await Update(TypesByInstanceIDID, TypesByInstanceID);
                }

            } catch (Exception e)
            {
                OS.Logger.LogError(e);
            }
        }

        public async Task<bool> Update(AID instanceID, object instance)
        {
            var col = db.GetTypeCollection(instance.GetType());
            var isInserted = col.Upsert(DBExtensions.InstanceToBSON(instanceID, instance));
            if (!TypesByInstanceID.ContainsKey(instanceID))
            {
                OnInstanceAdded(instanceID, instance);
            }
            return isInserted;
        }

        // Generic version of FindOne
        public async Task<Tuple<AID, TSource>> FindOne<TSource>(BsonExpression query) where TSource : class
        {
            var (id, instance) = await FindOne(typeof(TSource), query);
            return new Tuple<AID, TSource>(id, instance as TSource);
        }

        public async Task<Tuple<AID, object>> FindOne(AID instanceID)
        {
            Type type;
            if (!TypesByInstanceID.TryGetValue(instanceID, out type))
                return new Tuple<AID, object>(AID.Empty, null);

            return await FindOne(type, instanceID);
        }

        public async Task<Tuple<AID, object>> FindOne(Type type, AID instanceID)
        {
            return await FindOne(type, Query.EQ("_id", instanceID.Value));
        }

        public async Task<T> FindOrAddOne<T>(AID instanceID) where T : class, new()
        {
            var (_, instance) = await FindOne(typeof(T), instanceID);
            if (instance == null)
            {
                instance = await AddInstance(instanceID, Activator.CreateInstance<T>());
            }

            return (T)instance;
        }

        public async Task<Tuple<AID, object>> FindOne(Type type, BsonExpression query)
        {
            return await Task.Run(() =>
            {
                var col = db.GetTypeCollection(type);
                var doc = col.FindOne(query);
                if (doc == null)
                    return new Tuple<AID, object>(AID.Empty, null);

                var id = new AID((Guid)doc["_id"]);
                var instance = DBExtensions.BsonToObject(type, doc);
                return new Tuple<AID, object>(id, instance);
            });
        }

        public async Task<IEnumerable<TSource>> Find<TSource>(QueryBuilder<TSource> query) where TSource : class
        {
            return null;
        }

        public async Task<IEnumerable<Tuple<AID, object>>> Find<TSource>(BsonExpression expression) where TSource : class
        {
            var col = db.GetTypeCollection(typeof(TSource));
            var docs = col.Find(expression);
            return docs.Select(x=> new Tuple<AID, object>(new AID((Guid)x["_id"]), DBExtensions.BsonToObject(typeof(TSource), x)));
        }

        public async Task<TSource> FirstOrDefaultProp<TSource, TProperty>(Expression<Func<TSource, TProperty>> propExp, BsonExpression expression) where TSource : class
        {
            return null;
        }

        object GetObject(BsonDocument doc, string name)
        {
            BsonValue value;
            if (doc.TryGetValue(name, out value))
            {
                if (value.IsString)
                    return value.AsString;
                if (value.IsInt32)
                    return value.AsInt32;
                if (value.IsBoolean)
                    return value.AsBoolean;
                return value.AsString;
            }
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource, TProperty1, TProperty2>(Expression<Func<TSource, TProperty1>> propExp1, Expression<Func<TSource, TProperty2>> propExp2,
            Func<TProperty1, TProperty2, bool> predicate) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource, TProperty1, TProperty2>(Expression<Func<TSource, TProperty1>> propExp1, Expression<Func<TSource, TProperty2>> propExp2,
      Func<AID, AID, bool> predicate) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource>(AID instanceID) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource, TProperty>(Expression<Func<TSource, TProperty>> propExp, Func<AID, bool> predicate) where TSource : class where TProperty : class
        {
            return null;
        }

        public async Task<object> AddInstance(AID instanceID, object instance)
        {
            var col = db.GetTypeCollection(instance.GetType());
            var doc = DBExtensions.InstanceToBSON(instanceID, instance);
            col.Insert(doc);
            return instance;
        }

        public async Task<TSource> FindOrCreate<TSource>(AID instanceID) where TSource : class
        {
            return null;
        }

        public async Task Remove<TSource>(TSource instance) where TSource : class
        {
            return;
        }
        public async Task<bool> Delete(Type type, AID instanceID)
        {

                var col = db.GetTypeCollection(type);
                return col.Delete(instanceID.Value);
        }

        public void Clear()
        {
            foreach (var collectionName in db.GetCollectionNames().ToList())
            {
                db.DropCollection(collectionName);
            }
        }

        public async Task ClearAsync()
        {   
            foreach (var collectionName in db.GetCollectionNames().ToList())
            {
                db.DropCollection(collectionName);
            }
        }

        public async Task<bool> Clear(Type type)
        {
            return db.DropCollection(LocalDatabaseExtensions.GetTypeCollectionName(type));
        }

        public async Task<int> DeleteMany<TSource>(BsonExpression expression) where TSource : class
        {
            var col = db.GetTypeCollection(typeof(TSource));
            var count = col.DeleteMany(expression);
            OS.Log(count.ToString());
            return count;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
