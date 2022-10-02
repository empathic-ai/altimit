using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace Altimit.Serialization
{
    /// <summary>
    /// High-Level API of Altimit Serialization, uses MessagePack under the hood.
    /// </summary>
    public static partial class Serializer
    {
        public static Dictionary<Type, object> AssetFormatters = new Dictionary<Type, object>();

        static Serializer() {
        }

        public static byte[] Serialize<T>(T obj)
        {
            try
            {
                return MessagePackSerializer.Serialize(obj);
            } catch (Exception e)
            {
                OS.Logger.LogError(e);
                throw (e);
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(bytes);
            }
            catch (Exception e)
            {
                OS.Logger.LogError(e);
                throw (e);
            }
        }
        public static async Task<byte[]> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            return await SerializeAsync(typeof(T), obj, cancellationToken);
        }

        public static async Task<byte[]> SerializeAsync(Type type, object obj, CancellationToken cancellationToken = default)
        {
            try
            {
                object assetFormatter;
                if (AssetFormatters.TryGetValue(type, out assetFormatter))
                {
                    return await (typeof(IAsyncFormatter<>).MakeGenericType(obj.GetType()).GetMethod("SerializeAsync").Invoke(assetFormatter, new object[] { obj, cancellationToken }) as Task<byte[]>);
                }

                using (var ms = new MemoryStream())
                {
                    //var writer = new MessagePackWriter()
                    //InstanceFormatter.Serialize(new MessagePackWriter(), obj, AltimitResolver.Options);

                    await SerializeAsync(type, ms, obj, PrimitiveResolver.Options, cancellationToken);
                    return ms.ToArray();
                }
            }
            catch (MessagePackSerializationException e)
            {
                // Hacky addition because MessagePack is weird
                if (e.InnerException is OperationCanceledException)
                    cancellationToken.ThrowIfCancellationRequested();
                throw new Exception("Error serializing object!", e);
            }
        }

        public static async Task<T> DeserializeAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
        {
            return (T)(await DeserializeAsync(typeof(T), bytes, cancellationToken));
        }

        public static async Task DeserializeAsync(Type type, object instance, byte[] bytes, CancellationToken cancellationToken = default)
        {
            object assetFormatter;
            if (AssetFormatters.TryGetValue(type, out assetFormatter))
            {
                var methodInfo = typeof(IAsyncFormatter<>).MakeGenericType(type).GetMethods().Single(x => x.Name == "DeserializeAsync" && x.GetParameters().Length == 3);
                await (methodInfo.Invoke(assetFormatter, new object[] { instance, bytes, cancellationToken }) as Task);
            }
            else
            {
                throw new Exception("Error deserializing object!");
            }
        }

        public static async Task<object> DeserializeAsync(Type type, byte[] bytes, CancellationToken cancellationToken = default)
        {
            try
            {
                object assetFormatter;
                if (AssetFormatters.TryGetValue(type, out assetFormatter))
                {
                    return await ((dynamic)assetFormatter).DeserializeAsync(bytes, cancellationToken);
                }

                using (var ms = new MemoryStream(bytes))
                {
                    return await DeserializeAsync(type, ms, PrimitiveResolver.Options, cancellationToken);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deserializing object!", e);
            }
        }
    }
}