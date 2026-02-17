using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskForge.Core.Common;

namespace TaskForge.Core;


public class InvocationData
{
    private static readonly ConcurrentDictionary<MethodDeserializerCacheKey, MethodDeserializerCacheValue> methodDeserializerCache = new();
    private static readonly ConcurrentDictionary<MethodSerializerCacheKey, MethodSerializerCacheValue> methodSerializerCache = new();
    //方法类型
    public string? Type{get;set;}
    //方法名称
    public string? Method{get;set;}
    //参数类型
    public string[] ParameterTypes{get;set;}
    //参数值
    public string[] Arguments{get;set;}

    public InvocationData(string type, string method, string[] parameterTypes, string[] arguments)
    {
        Type = type;
        Method = method;
        ParameterTypes = parameterTypes;
        Arguments = arguments;
    }
    public static InvocationData SerializeJob(Job job)
    {
        CachedSerializeMethod(TypeHelper.CurrentTypeSerializer,job.TargetType,job.TargetMethod,out var typeName, out var methodName, out var parameterTypes);
        var arguments=(job.Args ?? Array.Empty<object>()).Select((arg)=>JsonSerializer.Serialize(arg)).ToArray();
        return new InvocationData(typeName, methodName, parameterTypes, arguments);
    }
    
    private static void CachedSerializeMethod(
        Func<Type, string> typeSerializer, Type type, MethodInfo methodInfo,
            out string typeName, out string methodName, out string[] parameterTypes)
    {
        var value = methodSerializerCache.GetOrAdd(new MethodSerializerCacheKey{
            TypeSerializer = typeSerializer,
            Type = type,
            Method = methodInfo
        }, key =>
        {
            var methodParameters = key.Method.GetParameters();
            ValidateParameters(methodParameters);
            return new MethodSerializerCacheValue
            {
                TypeName = key.TypeSerializer(key.Type),
                MethodName = key.Method.Name,
                ParameterTypes = methodParameters.Select(p=>key.TypeSerializer(p.ParameterType)).ToArray(),
            };
        });
        typeName = value.TypeName;
        methodName = value.MethodName;
        parameterTypes = value.ParameterTypes;
    }
    
    private static void ValidateParameters(ParameterInfo[] parameters)
    {
        foreach (var parameter in parameters)
        {
            var type = parameter.ParameterType;

            if (type == typeof(object))
            {
                throw new InvalidOperationException(
                    $"Parameter '{parameter.Name}' cannot be of type object. " +
                    $"TaskForge does not support polymorphic parameters.");
            }

            if (type.IsInterface)
            {
                throw new InvalidOperationException(
                    $"Parameter '{parameter.Name}' cannot be an interface type ({type.FullName}).");
            }

            if (type.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"Parameter '{parameter.Name}' cannot be an abstract class ({type.FullName}).");
            }

            if (type.ContainsGenericParameters)
            {
                throw new InvalidOperationException(
                    $"Parameter '{parameter.Name}' cannot be an open generic type ({type.FullName}).");
            }
        }
    }

    private readonly record struct MethodDeserializerCacheKey
    {
        public Func<string, Type> TypeResolver { get; init; }
        public string TypeName { get; init; }
        public string MethodName { get; init; }
        public string ParameterTypes { get; init; }
    }

    private readonly record struct MethodDeserializerCacheValue
    {
        public Type Type { get; init; }
        public MethodInfo Method { get; init; }
    }

    private readonly record struct MethodSerializerCacheKey
    {
        public Func<Type, string> TypeSerializer { get; init; }
        public Type Type { get; init; }
        public MethodInfo Method { get; init; }
    }
    
    private readonly record struct MethodSerializerCacheValue
    {
        public string TypeName { get; init; }
        public string MethodName { get; init; }
        public string[] ParameterTypes { get; init; }
    }
}
