using System.Collections.Concurrent;

namespace TaskForge.Core.Common;

public class TypeHelper
{
    private static readonly ConcurrentDictionary<Type,string> typeNameCache = new();
    private static readonly ConcurrentDictionary<string,Type> typeCache = new();
    public static string DefaultSerializeType(Type type)
    {
        return typeNameCache.GetOrAdd(type, t => t.AssemblyQualifiedName 
                    ?? throw new InvalidOperationException("Type must have an assembly qualified name"));
    }
    public static Type DefaultDeserializeType(string typeName)
    {
        return typeCache.GetOrAdd(typeName, tn => Type.GetType(tn) 
                    ?? throw new InvalidOperationException($"Failed to deserialize type: {tn}"));
    }

    internal static object Serialize(object arg)
    {
        throw new NotImplementedException();
    }


    private static Func<Type, string>? _currentTypeSerializer;
    public static Func<Type, string> CurrentTypeSerializer
    {
        get => Volatile.Read(ref _currentTypeSerializer) ?? DefaultSerializeType;
        set => Volatile.Write(ref _currentTypeSerializer, value);
    }
    private static Func<string, Type>? _currentTypeResolver;
    public static Func<string, Type> CurrentTypeResolver
    {
        get => Volatile.Read(ref _currentTypeResolver) ?? DefaultDeserializeType;
        set => Volatile.Write(ref _currentTypeResolver, value); 
    }
}
