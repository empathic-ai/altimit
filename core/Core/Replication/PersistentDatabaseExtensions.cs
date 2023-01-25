#define ALTIMIT_LITEDB

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if ALTIMIT_LITEDB
using LiteDB;
#endif

namespace Altimit
{
    public static partial class PersistentDatabaseExtensions
    {

#if ALTIMIT_LITEDB
        public static T BsonToObject<T>(BsonValue bsonValue)
        {
            return (T)BsonToObject(typeof(T), bsonValue);
        }

        public static object BsonToObject(Type type, BsonValue bsonValue)
        {
            if (OS.LogBSONFormatting)
                OS.Log($"Converting BSON to object of type {type.GetTypeName()}.");

            if (bsonValue == null)
                return null;

            if (type.IsObjectType())
            {
                var bsonDocument = bsonValue as BsonDocument;
                return BsonToObject(TypeExtensions.GetTypeByAName(bsonDocument["type"]), bsonDocument["value"]);
            }
            else if (type == typeof(Guid))
            {
                return new AID((Guid)bsonValue.RawValue);
            }
            else if (type.IsCollection())
            {
                var bsonDocument = bsonValue as BsonDocument;

                var obj = Activator.CreateInstance(type);

                int index = 0;
                BsonValue value;

                var elementType = type.GetATypeInfo().ElementType;

                while (bsonDocument.TryGetValue(index.ToString(), out value))
                {
                    // If we're dealing with a reference
                    obj.SetProperty(index.ToString(), BsonToObject(elementType, value));
                    index++;
                }
                return obj;
            }
            else if (type.IsStructure())
            {
                var bsonDocument = bsonValue as BsonDocument;

                var constructorInfo = type.GetATypeInfo().DefaultConstructorInfo;

                var obj = constructorInfo.Construct(constructorInfo.PropertyNames.Select(x => BsonToObject(type.GetPropertyInfo(x).PropertyType, bsonDocument[x])).ToArray());

                foreach (var propertyInfo in type.GetATypeInfo().ReplicatedPropertyInfos)
                {
                    if (OS.LogBSONFormatting)
                        OS.Log($"Converting BSON to property named {propertyInfo} of type {propertyInfo.PropertyType.GetTypeName()}.");

                    if (propertyInfo.CanSet)
                        obj.SetProperty(propertyInfo.Name, BsonToObject(propertyInfo.PropertyType, bsonDocument[propertyInfo.Name]));
                }
                return obj;
            }
            else
            {
                if (type == typeof(Type))
                {
                    return TypeExtensions.GetTypeByAName((string)bsonValue.RawValue);
                } else if (type.IsEnum)
                {
                    return Enum.ToObject(type, (int)bsonValue.RawValue);
                }
                return bsonValue.RawValue;
            }
        }
        
        public static BsonDocument InstanceToBSON(object instance)
        {
            var bsonDocument = (BsonDocument)ObjectToBSON(instance);
            bsonDocument["_id"] = instance.GetInstanceID().Value;
            return bsonDocument;
        }

        public static BsonValue ObjectToBSON(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is AID)
            {
                return new BsonValue(((AID)obj).Value);
            }
            else if (obj.IsStructure())
            {
                var bsonDocument = new BsonDocument();

                if (OS.LogBSONFormatting)
                    OS.Log($"Converting object of type {obj.GetType().GetTypeName()} to BSON.");

                foreach (var (propertyInfo, property) in obj.GetPropertiesByInfo())
                {
                    if (OS.LogBSONFormatting)
                        OS.Log($"Converting property named {propertyInfo} with value {property.ToNestedString()} to BSON.");

                    // If we're dealing with a reference
                    if (propertyInfo.PropertyType.IsObjectType()) {
                        bsonDocument[propertyInfo.Name] = new BsonDocument()
                        {
                            ["type"] = new BsonValue(property.GetAType().GetTypeName()),
                            ["value"] = ObjectToBSON(property)
                        };
                    }
                    else {

                        bsonDocument[propertyInfo.Name] = ObjectToBSON(property);
                    }
                }
                return bsonDocument;
            }
            else
            {

                if (obj is float)
                {
                    obj = Convert.ToDouble(obj);
                }
                else if (obj is Type)
                {
                    obj = TypeExtensions.GetTypeName(obj as Type);
                } else if (obj is Enum)
                {
                    obj = (int)obj;
                }
                return new BsonValue(obj);
            }
        }

        public static string ToString(this RuntimeDatabase db)
        {
            var globeString = "";
            foreach (var globalByID in db.InstancesByID)
            {
                globeString += (globalByID.Key.ToString() + ", " + globalByID.Value.ToNestedString()).ToBracketSring() + "\n";
            }
            return globeString;
        }

        public static string ToFormattedString(this object instance, FormatType formatType, int maxDepth = 10)
        {
            if (instance == null)
                return ToDereferencedString(null);

            RuntimeDatabase instanceDatabase;
            instance.TryGetDB(out instanceDatabase);
            return instance.ToFormattedString(instanceDatabase, formatType, maxDepth);
        }

        public static string ToFormattedString(this object instance, RuntimeDatabase instanceDatabase, FormatType formatType, int maxDepth = 10)
        {
            if (instance == null)
                return ToDereferencedString(null);

            return ToDereferencedString(DereferenceProperty(instance, maxDepth));
        }
#endif

        public static IDereferencedObject DereferenceInstance(this object obj)
        {
            return (IDereferencedObject)obj.Dereference();
        }

        public static object Dereference(this object obj, int maxDepth = -1, int currentDepth = 0)
        {
            currentDepth += 1;
            if (maxDepth != -1 && currentDepth > maxDepth)
                return null;

            object dereferencedObj;


            if (obj != null && obj.IsStructure() && !obj.IsDereferenced())
            {
                if (obj.GetType().IsGenericType)
                {
                    dereferencedObj = Activator.CreateInstance(obj.GetAType().GetDereferencedType().MakeGenericType(obj.GetType().GetGenericArguments()));
                } else
                {
                    dereferencedObj = Activator.CreateInstance(obj.GetAType().GetDereferencedType());
                }

                foreach (var (propertyInfo, property) in obj.GetPropertiesByInfo(true))
                {
                    if (OS.LogFormatting)
                        OS.Log($"Dereferencing property named {propertyInfo.Name}.");
                    BindingExtensions.SetProperty(ref dereferencedObj, TypeExtensions.GetDereferencedPropertyName(propertyInfo), DereferenceProperty(property, propertyInfo.PropertyType, maxDepth, currentDepth));
                }

                if (OS.LogFormatting)
                    OS.Log($"Dereferenced structure to {dereferencedObj.ToNestedString()}.");
            }
            else
            {
                dereferencedObj = obj;
            }

            return dereferencedObj;
        }

        public static object DereferenceProperty(this object property, int maxDepth = -1, int currentDepth = 0)
        {
            if (property == null)
                return null;

            return property.DereferenceProperty(property.GetType(), maxDepth, currentDepth);
        }

        public static object DereferenceProperty(this object property, Type propertyType, int maxDepth = -1, int currentDepth = 0)
        {
            object localProperty;

            if (property == null || !property.GetType().IsReplicatedType())
            {
                if (OS.LogFormatting)
                    OS.Log($"Attempted to dereference a null property, returning null.");

                return null;
            }

            if (propertyType.IsInstanceType() || (propertyType.IsObjectType() && property != null && property.IsInstance()))
            {
                if (OS.LogFormatting)
                    OS.Log($"Dereferencing instance of type {propertyType}.");

                // possibly allow empty? changed but may need to be reverted to allow empty
                localProperty = property.GetInstanceID(true);
            }
            else if (propertyType.IsStructure() || (propertyType.IsObjectType() && property != null && property.IsStructure()))
            {
                if (OS.LogFormatting)
                    OS.Log($"Dereferencing structure of type {propertyType}.");

                localProperty = Dereference(property, maxDepth, currentDepth);
            }
            else
            {
                localProperty = property;
            }

            if (OS.LogFormatting)
            {
                if (localProperty == null)
                {
                    OS.Log($"Dereferenced property of type {propertyType} with value {property.ToNestedString()} is returning null.");
                }
                else
                {
                    OS.Log($"Dereferenced property of type {propertyType} with value {property.ToNestedString()} to type {localProperty.GetAType().GetTypeName()} with value {localProperty.ToNestedString()}.");
                }
            }

            return localProperty;
        }

        public static string ToDereferencedString(this object global, int maxDepth = 10)
        {
            if (global == null)
                return ToNestedString(null);

            return ToNestedString(global.Dereference(maxDepth));
        }

        public static string ToNestedString(this object obj, int maxDepth = 10, int currentDepth = -1)
        {
            currentDepth += 1;
            if (currentDepth > maxDepth)
            {
                //OS.Log("ASDF");
                //UnityEngine.Debug.Break();
                //UnityEditor.EditorApplication.isPlaying = false;
                return "";
            }

            var localString = "";
            if (obj != null)
            {
                if (obj.IsStructure())
                {
                    localString += "[";

                    var isFirst = true;
                    foreach (var (propertyInfo, property) in obj.GetPropertiesByInfo())
                    {
                        if (!isFirst)
                            localString += ", ";
                        localString += propertyInfo.Name + ": " + ToNestedString(property, maxDepth, currentDepth);
                        isFirst = false;
                    }

                    localString += "]";
                }
                else
                {
                    localString += obj.ToString();
                }
            }
            else
            {
                localString += "null";
            }

            return localString;
        }

        public static bool EqualFloats(object a, object b, float roundValue)
        {
            return Approximately((float)a, (float)b, roundValue);
        }

        /*
        public static bool EqualVectors(object a, object b, float roundValue)
        {
            bool? equalNulls = EqualNulls(a, b);
            if (equalNulls != null)
                return (bool)equalNulls;

            var va = (Vector3)a;
            var vb = (Vector3)b;
            return EqualFloats(va.x, vb.x, roundValue) && EqualFloats(va.y, vb.y, roundValue) && EqualFloats(va.z, vb.z, roundValue);
        }*/

        public static bool EqualLocals(object a, object b)
        {
            bool? equalNulls = TypeExtensions.EqualNulls(a, b);
            if (equalNulls != null)
                return (bool)equalNulls;

            //If types don't match
            if (a.GetType() != b.GetType())
                return false;
            var type = a.GetType();

            if (type.IsArray)
            {
                var aProperties = (object[])a;
                var bProperties = (object[])b;
                if (aProperties.Length != bProperties.Length)
                    return false;

                for (int i = 0; i < aProperties.Length; i++)
                {
                    if (!EqualLocals(aProperties[i], bProperties[i]))
                        return false;
                }
                return true;
            }
            if (a.GetType() == typeof(float))
            {
                return EqualFloats(a, b, .01f);
            }
            return a.Equals(b);
        }


        public static bool Approximately(float a, float b, float tolerance)
        {
            return (Math.Abs(a - b) < tolerance);
        }

        public static void SetSnapshot(this RuntimeDatabase globe, Dictionary<AID, object> globalInstancesByID)
        {
            foreach (var globalInstanceByID in globalInstancesByID)
            {
                globe.AddOrSetInstance(globalInstanceByID.Key, globalInstanceByID.Value);
            }
        }

        /*
        public static bool TryGetDeltaLocals(Globe globe, ref Dictionary<int[], BitArray> dirtyGlobals,
            out GlobalPathReference[] deltaGlobals)
        {
            var deltaLocalsList = new List<GlobalPathReference>();
            bool isDirty = false;

            foreach (var dirtyGlobal in dirtyGlobals)
            {
                //iterate over id path starting from first id in array
                //check if dictionary contains any partial path. If so, skip this keyValuePair

                var structureIDs = dirtyGlobal.Key;
                var globalParameters = globe.GetGlobalParameters(structureIDs);
                var structure = globalParameters.Value;

                if (structure != null && !structure.Equals(null))
                {
                    var classInfo = structure.GetClassInfo();
                    var dirtyProperties = dirtyGlobal.Value;

                    for (int i = 0; i < dirtyProperties.Length; i++)
                    {
                        if (dirtyProperties[i])
                        {
                            isDirty = true;
                            //Debug.Log(classInfo.Type.ToString() + ", " + i.ToString() + ", " + classInfo.IndexPropertyInfos.Length);
                            var propertyInfo = classInfo.IndexPropertyInfos[i];
                            var property = propertyInfo.Get(structure);

                            //Creates path of IDs to specific property
                            var propertyIDs = new int[structureIDs.Length + 1];
                            Array.Copy(structureIDs,propertyIDs,structureIDs.Length);
                            propertyIDs[structureIDs.Length] = propertyInfo.ID;

                            //Adds deltaLocalReference of property to deltaGlobal
                            deltaLocalsList.Add(new GlobalPathReference(propertyIDs,property));

                            dirtyProperties[i] = false;
                        }
                    }
                }
            }

            deltaGlobals = deltaLocalsList.ToArray();
            return isDirty;
        }*/
        /*
        public static void SetDeltaGlobals(Globe globe, GlobalPathReference[] deltaGlobals)
        {
            //Debug.Log("Setting delta snapshot.");
            foreach (var deltaGlobal in deltaGlobals)
            {
                globe.SetPathReference(deltaGlobal);
            }
        }
       
        public static void SetGlobalDirty(Globe globe, ref Dictionary<int[], BitArray> dirtyGlobals, int[] ids)
        {
            var propertyID = ids[ids.Length - 1];
            var structureIDs = new int[ids.Length - 1];
            Array.Copy(ids, structureIDs, structureIDs.Length);
            var classInfo = globe.GetGlobalParameters(structureIDs).Value.GetClassInfo();

            BitArray dirtyProperties;
            if (!dirtyGlobals.TryGetValue(structureIDs, out dirtyProperties))
            {
                dirtyProperties = new BitArray(classInfo.IndexPropertyInfos.Length);
                dirtyGlobals.Add(structureIDs, dirtyProperties);
            }

            dirtyProperties[Array.IndexOf(classInfo.IndexPropertyInfos, classInfo.GetPropertyInfoByID(propertyID))] = true;
        }

        public static void SetPropertyDirty(ref BitArray dirtyProperties, object global, string propertyName)
        {
            var classInfo = global.GetClassInfo();
            var propertyInfo = classInfo.GetPropertyInfoByName(propertyName);

            int index = Array.IndexOf(classInfo.IndexPropertyInfos, propertyInfo);
            if (index != -1)
                dirtyProperties[index] = true;
        }*/
    }
}
