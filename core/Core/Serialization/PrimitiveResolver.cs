using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Altimit.Serialization
{
    public class PrimitiveResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly PrimitiveResolver Instance;
        /// <summary>
        /// A <see cref="MessagePackSerializerOptions"/> instance with this formatter pre-configured.
        /// </summary>
        public static readonly MessagePackSerializerOptions Options;
        static readonly Dictionary<Type, IMessagePackFormatter> formatterMap = new Dictionary<Type, IMessagePackFormatter>() {

            // ID
            //{  typeof(Guid), new GuidFormatter() },

            // Type
            {  typeof(Type), new TypeFormatter() },
            
            // Primitive
            { typeof(Int16), Int16Formatter.Instance },
            { typeof(Int32), Int32Formatter.Instance },
            { typeof(Int64), Int64Formatter.Instance },
            { typeof(UInt16), UInt16Formatter.Instance },
            { typeof(UInt32), UInt32Formatter.Instance },
            { typeof(UInt64), UInt64Formatter.Instance },
            { typeof(Single), SingleFormatter.Instance },
            { typeof(Double), DoubleFormatter.Instance },
            { typeof(bool), BooleanFormatter.Instance },
            { typeof(byte), ByteFormatter.Instance },
            { typeof(sbyte), SByteFormatter.Instance },
            { typeof(DateTime), DateTimeFormatter.Instance },
            { typeof(char), CharFormatter.Instance },

            // Nulllable Primitive
            { typeof(Int16?), NullableInt16Formatter.Instance },
            { typeof(Int32?), NullableInt32Formatter.Instance },
            { typeof(Int64?), NullableInt64Formatter.Instance },
            { typeof(UInt16?), NullableUInt16Formatter.Instance },
            { typeof(UInt32?), NullableUInt32Formatter.Instance },
            { typeof(UInt64?), NullableUInt64Formatter.Instance },
            { typeof(Single?), NullableSingleFormatter.Instance },
            { typeof(Double?), NullableDoubleFormatter.Instance },
            { typeof(bool?), NullableBooleanFormatter.Instance },
            { typeof(byte?), NullableByteFormatter.Instance },
            { typeof(sbyte?), NullableSByteFormatter.Instance },
            { typeof(DateTime?), NullableDateTimeFormatter.Instance },
            { typeof(char?), NullableCharFormatter.Instance },

            // StandardClassLibraryFormatter
            { typeof(string), NullableStringFormatter.Instance },
            { typeof(decimal), DecimalFormatter.Instance },
            { typeof(decimal?), new StaticNullableFormatter<decimal>(DecimalFormatter.Instance) },
            { typeof(TimeSpan), TimeSpanFormatter.Instance },
            { typeof(TimeSpan?), new StaticNullableFormatter<TimeSpan>(TimeSpanFormatter.Instance) },
            { typeof(DateTimeOffset), DateTimeOffsetFormatter.Instance },
            { typeof(DateTimeOffset?), new StaticNullableFormatter<DateTimeOffset>(DateTimeOffsetFormatter.Instance) },
            { typeof(Guid), NativeGuidFormatter.Instance },
            { typeof(Guid?), new StaticNullableFormatter<Guid>(NativeGuidFormatter.Instance) },
            { typeof(Uri), UriFormatter.Instance },
            { typeof(Version), VersionFormatter.Instance },
            { typeof(StringBuilder), StringBuilderFormatter.Instance },
            { typeof(BitArray), BitArrayFormatter.Instance },
            //{ typeof(Type), TypeFormatter<Type>.Instance },

            // special primitive
            { typeof(byte[]), ByteArrayFormatter.Instance },

                        // Nil
            { typeof(Nil), NilFormatter.Instance },
            { typeof(Nil?), NullableNilFormatter.Instance },

            // optimized primitive array formatter
            { typeof(Int16[]), Int16ArrayFormatter.Instance },
            { typeof(Int32[]), Int32ArrayFormatter.Instance },
            { typeof(Int64[]), Int64ArrayFormatter.Instance },
            { typeof(UInt16[]), UInt16ArrayFormatter.Instance },
            { typeof(UInt32[]), UInt32ArrayFormatter.Instance },
            { typeof(UInt64[]), UInt64ArrayFormatter.Instance },
            { typeof(Single[]), SingleArrayFormatter.Instance },
            { typeof(Double[]), DoubleArrayFormatter.Instance },
            { typeof(Boolean[]), BooleanArrayFormatter.Instance },
            { typeof(SByte[]), SByteArrayFormatter.Instance },
            { typeof(DateTime[]), DateTimeArrayFormatter.Instance },
            { typeof(Char[]), CharArrayFormatter.Instance },
            { typeof(string[]), NullableStringArrayFormatter.Instance },

            // well known collections
            { typeof(List<Int16>), new ListFormatter<Int16>() },
            { typeof(List<Int32>), new ListFormatter<Int32>() },
            { typeof(List<Int64>), new ListFormatter<Int64>() },
            { typeof(List<UInt16>), new ListFormatter<UInt16>() },
            { typeof(List<UInt32>), new ListFormatter<UInt32>() },
            { typeof(List<UInt64>), new ListFormatter<UInt64>() },
            { typeof(List<Single>), new ListFormatter<Single>() },
            { typeof(List<Double>), new ListFormatter<Double>() },
            { typeof(List<Boolean>), new ListFormatter<Boolean>() },
            { typeof(List<byte>), new ListFormatter<byte>() },
            { typeof(List<SByte>), new ListFormatter<SByte>() },
            { typeof(List<DateTime>), new ListFormatter<DateTime>() },
            { typeof(List<Char>), new ListFormatter<Char>() },
            { typeof(List<string>), new ListFormatter<string>() },
        };

        static PrimitiveResolver()
        {
            Instance = new PrimitiveResolver();
            Options = MessagePackSerializerOptions.Standard.WithResolver(Instance);
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            IMessagePackFormatter formatter;
            lock (formatterMap)
            {
                if (typeof(T).IsTypeType())
                {
                    formatter = formatterMap[typeof(Type)];
                }
                else
                {
                    if (!formatterMap.TryGetValue(typeof(T), out formatter))
                    {
                        throw new Exception($"Can't find primitive formatter for type {typeof(T).Name}!");
                    }
                }
            }
            return (IMessagePackFormatter<T>)formatter;
        }
    }
}