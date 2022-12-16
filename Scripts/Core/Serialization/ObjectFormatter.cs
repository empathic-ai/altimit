using MessagePack;
using MessagePack.Formatters;
using System;
using System.Linq;

namespace Altimit.Serialization
{
    public static class ObjectFormatter// : IMessagePackFormatter<object>
    {
        public static void Serialize(Type type, ref MessagePackWriter writer, object obj, MessagePackSerializerOptions options)
        {
            if (OS.LogSerializiation)
                OS.Log($"Serializing object of type {type.GetTypeName()} with value {obj.LocalToString()}.");

            if (type.IsEnum())
            {
                writer.WriteInt32((int)obj);
            }
            else if (type.IsObjectType())
            {
                if (obj == null)
                {
                    writer.WriteNil();
                }
                else
                {
                    //if (OS.LogSerializiation)
                    //    OS.Log($"Serializing object with actual type {obj.GetAType().GetTypeName()} and value {obj.LocalToString()}.");
                    options.Resolver.GetFormatter<Type>().Serialize(ref writer, obj.GetAType(), options);
                    Serialize(obj.GetAType(), ref writer, obj, options);
                }
            } else if (type.IsStructure())
            {
                writer.Write(obj == null);

                if (obj == null)
                {
                    return;
                }

                if (type.IsCollection())
                {
                    writer.WriteArrayHeader(obj.GetPropertyCount());
                    if (OS.LogSerializiation)
                        OS.Log($"Collection has {obj.GetPropertyCount()} elements.");
                }

                foreach (var (propertyInfo, property) in obj.GetPropertiesByInfo())
                {
                    if (OS.LogSerializiation)
                        OS.Log($"Serializing property named {propertyInfo} " +
                            $"of type {propertyInfo.PropertyType} and value {property.LocalToString()}");
                    Serialize(propertyInfo.PropertyType, ref writer, property, options);
                }
            }
            else
            {
                if (obj == null)
                {
                    writer.WriteNil();
                } else
                {
                    if (type == typeof(float))
                    {
                        type = typeof(double);
                        obj = Convert.ToDouble(obj);
                    }
                    MessagePackSerializer.Serialize(type, ref writer, obj, options);
                }
                //OS.Log(type.IsObjectType());
                /*
                var formatter = options.Resolver.GetFormatterDynamic(instance.GetType());
                var methodInfo = formatter.GetMethodInfo("Serialize", new Type[] { typeof(MessagePackWriter), instance.GetType(), typeof(MessagePackSerializerOptions) });
                var w = writer;
                var args = new object[] { w, instance, options };
                */
               

//                methodInfo.Invoke(formatter, w, instance, options);
//                throw new Exception(string.Format("Attempted to serialize unknown type {0}!", type));
            }
        }

        public static object Deserialize(Type type, ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            object obj;

            //if (OS.LogSerializiation)
            //    OS.Log($"Derializing object of type {type.GetTypeName()}.");

            if (type.IsEnum())
            {
                obj = reader.ReadInt32();
            }
            else if (type.IsObjectType())
            {
                if (reader.TryReadNil())
                {
                    obj = null;
                }
                else
                {
                    type = options.Resolver.GetFormatter<Type>().Deserialize(ref reader, options);

                    //if (OS.LogSerializiation)
                    //    OS.Log($"Object's type was deserialized as {type.GetTypeName()}. Deserializing object.");

                    obj = Deserialize(type, ref reader, options);
                }
            }
            else if (type.IsStructure())
            {
                if (reader.ReadBoolean())
                {
                    obj = null;
                }
                else
                {
                    var typeInfo = type.GetATypeInfo();
                    var constructorInfo = typeInfo.DefaultConstructorInfo;

                    // Construct the global--if we're dealing with a collection,
                    // the constructor arguments are simply the collection's length
                    int elementCount;
                    if (type.IsCollection())
                    {
                        elementCount = reader.ReadArrayHeader();
                        obj = constructorInfo.Construct(new object[] { elementCount });

                        if (OS.LogSerializiation)
                            OS.Log($"Collection has {elementCount} elements.");
                    }
                    else
                    {
                        obj = constructorInfo.Construct(new object[0]);
                        elementCount = type.GetATypeInfo().ReplicatedPropertyInfos.Length;
                    }

 
                    // Set any other properties of the global
                    foreach (var propertyInfo in type.GetPropertyInfos(elementCount))
                    {
                        //if (OS.LogSerializiation)
                        //    OS.Log($"Deserializing property named {propertyInfo} of type {propertyInfo.PropertyType}.");
                        var property = Deserialize(propertyInfo.PropertyType, ref reader, options);
                        if (OS.LogSerializiation)
                            OS.Log($"Deserialized property named {propertyInfo} with value {property.LocalToString()}.");
                        BindingExtensions.SetProperty(ref obj, propertyInfo.Name, property);
                    }
                }
            }
            else
            {
                if (reader.TryReadNil())
                {
                    obj = null;
                }
                else
                {
                    if (type == typeof(float))
                    {
                        obj = Convert.ToSingle(MessagePackSerializer.Deserialize(typeof(double), ref reader, options));
                    }
                    else
                    {
                        obj = MessagePackSerializer.Deserialize(type, ref reader, options);
                    }
                }
                //throw new Exception(string.Format("Attempted to deserialize unknown type {0}!", type));
            }

            if (OS.LogSerializiation)
                OS.Log($"Derialized object of type {type.GetTypeName()} with value {obj.LocalToString()}.");

            return obj;
        }
    }
}
