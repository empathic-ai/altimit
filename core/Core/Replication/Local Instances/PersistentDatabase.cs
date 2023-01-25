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
using Meridian;
using System.Data.Common;
//using Microsoft.Data.Sqlite;
//using System.Data.SQLite;

#if ALTIMIT_LITEDB
using LiteDB;
#else
using SQLite;
#endif

namespace Altimit
{
    // Has partial implimentation for SQLite, but was abandoned in favor of LiteDB
    // LiteDB appears easier to work with for nested data
    public partial class PersistentDatabase : IAsyncDisposable
    {
        ILogger logger;
        AID TypesByInstanceIDID => new AID("56018309-8402-4e00-b6ce-96601ecd888f");
        ADictionary<AID, Type> TypesByInstanceID = new ADictionary<AID, Type>();

#if !ALTIMIT_LITEDB
        SQLiteAsyncConnection db;

        public DereferencedDatabase()
        {
        }

        public async Task Connect(string dbPath, ILogger logger, bool reset = false)
        {

            if (reset)
                Clear();

            db = new SQLiteAsyncConnection(dbPath);
            logger.Log($"Connected to local database at path {dbPath}.");


            //TypesByInstanceID = await FindOrAddOne<ADictionary<Guid, Type>>(TypesByInstanceIDID);
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
                    //TypesByInstanceID.Add(instanceID, instance.GetType());
                    //await Update(TypesByInstanceIDID, TypesByInstanceID);
                }
            }
            catch (Exception e)
            {
                OS.Logger.LogError(e);
            }
        }

        // Returns a value indicating whether the database updated
        public async Task<bool> Update(IDereferenced dereferencedInstance)
        {
            await db.CreateTableAsync(dereferencedInstance.GetType());
            var rowsModified = await db.InsertOrReplaceAsync(dereferencedInstance);
            //var isInserted = col.Upsert(DBExtensions.InstanceToBSON(instanceID, instance));
            /*
            if (!TypesByInstanceID.ContainsKey(dereferencedInstance.ID))
            {
                OnInstanceAdded(dereferencedInstance.ID, dereferencedInstance);
            }
            */
            return rowsModified > 0;
        }

        // Generic version of FindOne
        public async Task<IDereferenced<TSource>> FindOne<TSource>(BsonExpression query) where TSource : class
        {
            var dereferencedInstance = await FindOne(typeof(TSource), query);
            return dereferencedInstance as IDereferenced<TSource>;
        }

        public async Task<IInstance> FindOne(AID instanceID)
        {
            Type type;
            if (!TypesByInstanceID.TryGetValue(instanceID, out type))
                return null;

            return await FindOne(type, instanceID);
        }

        public async Task<IInstance> FindOne(Type type, AID instanceID)
        {
            return (IInstance) (await db.FindAsync(instanceID, await db.GetMappingAsync(type)));
        }

        public async Task<T> FindOrAddOne<T>(AID instanceID) where T : IInstance, new()
        {
            var instance = await FindOne(typeof(T), instanceID);
            if (instance == null)
            {
                instance = Activator.CreateInstance<T>();
                instance.ID = instanceID;
                instance = await AddInstance(instance);
            }

            return (T)instance;
        }

        public async Task<IDereferenced> FindOne(Type type, BsonExpression query)
        {


            /*
            var col = db.GetTypeCollection(type);
            var doc = col.FindOne(query);
            if (doc == null)
                return new Tuple<Guid, object>(Guid.Empty, null);

            var id = new Guid((Guid)doc["_id"]);
            var instance = DBExtensions.BsonToObject(type, doc);
            return new Tuple<Guid, object>(id, instance);
            */
            return null;
        }

        public async Task<IEnumerable<IDereferenced<TReferencedType>>> FindReferencedType<TReferencedType>(QueryBuilder<TReferencedType> query) where TReferencedType : class
        {
            return null;
        }

        public async Task<IEnumerable<IDereferenced<TReferencedType>>> FindFromReferenced<TReferencedType>(BsonExpression expression) where TReferencedType : class
        {
            /*
            var col = db.GetTypeCollection(typeof(TSource));
            var docs = col.Find(expression);
            return docs.Select(x => new Tuple<Guid, object>(new Guid((Guid)x["_id"]), DBExtensions.BsonToObject(typeof(TSource), x)));
            */
            return null;
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
      Func<Guid, Guid, bool> predicate) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource>(AID instanceID) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource, TProperty>(Expression<Func<TSource, TProperty>> propExp, Func<Guid, bool> predicate) where TSource : class where TProperty : class
        {
            return null;
        }

        public async Task<IInstance> AddInstance(IInstance instance)
        {
            /*
            var col = db.GetTypeCollection(instance.GetType());
            var doc = DBExtensions.InstanceToBSON(instanceID, instance);
            col.Insert(doc);
            return instance;
            */
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
            return await db.DeleteAsync<Guid>(instanceID) > 0;

            //var col = db.GetTypeCollection(type);
            //return col.Delete(instanceID.Value);
        }

        public void Clear()
        {
            /*
            foreach (var collectionName in db.GetCollectionNames().ToList())
            {
                db.DropCollection(collectionName);
            }
            */
        }

        public async Task<bool> Clear(Type type)
        {
            return await db.DeleteAllAsync(await db.GetMappingAsync(type)) > 0;
        }

        public async Task<int> DeleteMany<TSource>(BsonExpression expression) where TSource : class
        {
            /*
            var col = db.GetTypeCollection(typeof(TSource));
            var count = col.DeleteMany(expression);
            OS.Log(count.ToString());
            return count;
            */
            return 0;
        }

        public async ValueTask DisposeAsync()
        {
            await db.CloseAsync();
        }

        public string GetTableName(Type type)
        {
            return type.GetFullName().Replace(".", "_").Replace(",", "_").Replace("[", "_$").Replace("]", "$_");
        }
#else
        LiteDatabase db;

        public PersistentDatabase()
        {
        }

        public async Task Connect(string dbPath, ILogger logger, bool reset = false)
        {
            if (reset)
                File.Delete(dbPath);

            db = new LiteDatabase(dbPath);
            logger.Log($"Connected to local database at path {dbPath}.");

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

        async void OnInstanceAdded(object instance)
        {
            try
            {
                if (TypesByInstanceID != null)
                {
                    TypesByInstanceID.Add(instance.GetInstanceID(), instance.GetType());
                    //await Update(TypesByInstanceIDID, TypesByInstanceID);
                }

            } catch (Exception e)
            {
                OS.Logger.LogError(e);
            }
        }

        public async Task<bool> Update(object instance)
        {
            var col = db.GetTypeCollection(instance.GetType());
            var isInserted = col.Upsert(PersistentDatabaseExtensions.InstanceToBSON(instance));
            if (!TypesByInstanceID.ContainsKey(instance.GetInstanceID()))
            {
                OnInstanceAdded(instance);
            }
            return isInserted;
        }

        // Generic version of FindOne
        public async Task<TSource> FindOne<TSource>(BsonExpression query) where TSource : class
        {
            var instance = await FindOne(typeof(TSource), query);
            return instance as TSource;
        }

        public async Task<object> FindOne(AID instanceID)
        {
            Type type;
            if (!TypesByInstanceID.TryGetValue(instanceID, out type))
                return null;

            return await FindOne(type, instanceID);
        }

        public async Task<object> FindOne(Type type, AID instanceID)
        {
            return await FindOne(type, Query.EQ("_id", instanceID.Value));
        }

        public async Task<T> FindOrAddOne<T>(AID instanceID) where T : class, new()
        {
            var instance = await FindOne(typeof(T), instanceID);
            if (instance == null)
            {
                instance = await AddInstance(Activator.CreateInstance<T>().SetInstanceID(instanceID));
            }

            return (T)instance;
        }

        public async Task<object> FindOne(Type type, BsonExpression query)
        {
            return await Task.Run(() =>
            {
                var col = db.GetTypeCollection(type);
                var doc = col.FindOne(query);
                if (doc == null)
                    return null;

                var id = new AID((Guid)doc["_id"]);
                var instance = PersistentDatabaseExtensions.BsonToObject(type, doc);
                return instance;
            });
        }

        public async Task<IEnumerable<TSource>> Find<TSource>(QueryBuilder<TSource> query) where TSource : class
        {
            return null;
        }

        public async Task<IEnumerable<object>> Find<TSource>(BsonExpression expression) where TSource : class
        {
            var col = db.GetTypeCollection(typeof(TSource));
            var docs = col.Find(expression);
            return docs.Select(x => new Tuple<AID, object>(new AID((Guid)x["_id"]), PersistentDatabaseExtensions.BsonToObject(typeof(TSource), x)));
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

        public async Task<TSource> FirstOrDefault<TSource>(AID instanceID) where TSource : class
        {
            return null;
        }

        public async Task<TSource> FirstOrDefault<TSource, TProperty>(Expression<Func<TSource, TProperty>> propExp, Func<Guid, bool> predicate) where TSource : class where TProperty : class
        {
            return null;
        }

        public async Task<object> AddInstance(object instance)
        {
            var col = db.GetTypeCollection(instance.GetType());
            var doc = PersistentDatabaseExtensions.InstanceToBSON(instance);
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
            return count;
        }

        public async ValueTask DisposeAsync()
        {
            db.Dispose();
        }
#endif
    }

#if ALTIMIT_LITEDB
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
#endif
}