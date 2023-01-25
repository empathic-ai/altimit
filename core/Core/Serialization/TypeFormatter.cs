using MessagePack;
using MessagePack.Formatters;
using System;

namespace Altimit.Serialization
{
    public sealed class TypeFormatter: IMessagePackFormatter<Type>
    {
        public void Serialize(ref MessagePackWriter writer, Type type, MessagePackSerializerOptions options)
        {
            options.Resolver.GetFormatter<string>().Serialize(ref writer, type.GetTypeName(), options);
        }

        public Type Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return TypeExtensions.GetTypeByAName(options.Resolver.GetFormatter<string>().Deserialize(ref reader, options));
        }
    }
}
