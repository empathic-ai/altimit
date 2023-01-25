using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Altimit
{
    public static class InstanceExtensions
    {
        static Dictionary<object, AID> instanceIDsByInstance = new Dictionary<object, AID>();

        public static RuntimeDatabase GetDB(this object instance)
        {
            RuntimeDatabase db;
            if (!TryGetDB(instance, out db))
                throw new Exception(string.Format("Failed to find database for instance of type {0}! Add this object to a globe or don't call this extension method.",
                    instance.GetType()));
                //OS.Logger.LogError(string.Format("Failed to find database for instance of type {0}! Add this object to a globe or don't call this extension method.",
                //    instance.GetType()));
            return db;
        }

        public static bool TryGetDB(this object instance, out RuntimeDatabase db)
        {
            try
            {
                db = RuntimeDatabase.Databases.Where(x => x.HasInstance(instance)).SingleOrDefault();
            } catch (Exception e)
            {
                OS.Logger.LogError(new Exception($"Failed to get database for instance of type {instance.GetType()} and value {instance.ToNestedString()}.", e));
                db = null;
                return false;
            }
            return db != null;
        }

        public static T GetInstance<T>(this object instance, AID instanceID) where T : class
        {
            RuntimeDatabase db;
            object childInstance;
            if (instance.TryGetDB(out db))
            {
                db.TryGetInstance(instanceID, out childInstance);
                return (T)childInstance;
            }
            else
            {
                return null;
            }
        }

        public static AID GetInstanceID(this object instance, bool allowEmpty = false)
        {
            AID instanceID;
            if (instanceIDsByInstance.TryGetValue(instance, out instanceID))
            {
                return instanceID;
            }
            return AID.Empty;

            /*
            if (allowEmpty && (instance == null || !instance.TryGetDB(out _)))
            {
                return AID.Empty;
            }
            return instance.GetDB().GetInstanceID(instance);
            */
        }

        public static object SetInstanceID(this object instance, AID instanceID)
        {
            instanceIDsByInstance[instance] = instanceID;
            return instance;

            /*
            if (allowEmpty && (instance == null || !instance.TryGetDB(out _)))
            {
                return AID.Empty;
            }
            return instance.GetDB().GetInstanceID(instance);
            */
        }

        public static void SetInstance<T, P>(this T instance, Expression<Func<T, AID>> propExp, P value)
        {
            instance.SetProperty(propExp, instance.GetDB().TryAddOrGetInstanceID(value));
        }
    }
}