using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TaskForge.Core
{
    /// <summary>
    /// 通用任务执行器，支持同步/异步/泛型返回值/静态方法/实例方法
    /// </summary>
    public class Job
    {
        public Guid Id { get; }
        public Type TargetType { get; }
        public MethodInfo TargetMethod { get; }
        public object[]? Args { get; }

        // 执行委托缓存，线程安全
        private Func<CancellationToken, Task<object?>>? _execute;

        // 动态生成委托，避免每次反射调用
        private Func<object?, object[], object?>? _syncDelegate;
        private Func<object?, object[], Task>? _asyncDelegate;

        public Job(Guid id, Type targetType, MethodInfo targetMethod, object[]? args)
        {
            Id = id;
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            TargetMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            Args = args;

            CreateDelegates();
        }

        /// <summary>
        /// 核心可执行函数，支持 CancellationToken
        /// </summary>
        public Func<CancellationToken, Task<object?>> Execute
        {
            get
            {
                var exec = Volatile.Read(ref _execute);
                if (exec != null) return exec;

                exec = async (ct) => await ExecuteAsync(ct).ConfigureAwait(false);
                Volatile.Write(ref _execute, exec);
                return exec;
            }
            set => Volatile.Write(ref _execute, value);
        }

        /// <summary>
        /// 泛型版本，类型安全
        /// </summary>
        public async Task<T?> ExecuteAsync<T>(CancellationToken cancellationToken = default)
        {
            var result = await ExecuteAsync(cancellationToken).ConfigureAwait(false);
            if (result == null) return default;
            return (T)result;
        }

        /// <summary>
        /// 核心执行逻辑
        /// </summary>
        public async Task<object?> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // 静态方法不需要实例
            object? instance = TargetMethod.IsStatic ? null : Activator.CreateInstance(TargetType);

            object? result;
            try
            {
                if (_syncDelegate != null)
                {
                    // 同步方法
                    result = _syncDelegate(instance, Args ?? Array.Empty<object>());
                }
                else if (_asyncDelegate != null)
                {
                    // 异步方法
                    var task = _asyncDelegate(instance, Args ?? Array.Empty<object>());
#if NET6_0_OR_GREATER
                    await task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
                    using var reg = cancellationToken.Register(() => throw new OperationCanceledException(cancellationToken));
                    await task.ConfigureAwait(false);
#endif
                    // Task<T> 返回值处理
                    var type = task.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var property = type.GetProperty("Result");
                        return property?.GetValue(task);
                    }
                    return null;
                }
                else
                {
                    throw new InvalidOperationException("未能生成方法调用委托");
                }
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }

            return result;
        }

        /// <summary>
        /// 根据 TargetMethod 生成委托，提升性能
        /// </summary>
        private void CreateDelegates()
        {
            if (TargetMethod.ReturnType == typeof(void) || !typeof(Task).IsAssignableFrom(TargetMethod.ReturnType))
            {
                // 同步方法
                _syncDelegate = CreateSyncDelegate(TargetMethod);
            }
            else
            {
                // 异步方法 Task 或 Task<T>
                _asyncDelegate = CreateAsyncDelegate(TargetMethod);
            }
        }

        /// <summary>
        /// 创建同步方法委托
        /// </summary>
        private static Func<object?, object[], object?> CreateSyncDelegate(MethodInfo method)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var parameters = method.GetParameters();
            var argExpressions = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var accessor = Expression.ArrayIndex(argsParam, index);
                var converted = Expression.Convert(accessor, parameters[i].ParameterType);
                argExpressions[i] = converted;
            }

            var instanceCast = method.IsStatic ? null : Expression.Convert(instanceParam, method.DeclaringType!);
            var call = Expression.Call(instanceCast, method, argExpressions);

            Expression body = method.ReturnType == typeof(void)
                ? (Expression)Expression.Block(call, Expression.Constant(null, typeof(object)))
                : Expression.Convert(call, typeof(object));

            var lambda = Expression.Lambda<Func<object?, object[], object?>>(body, instanceParam, argsParam);
            return lambda.Compile();
        }

        /// <summary>
        /// 创建异步方法委托
        /// </summary>
        private static Func<object?, object[], Task> CreateAsyncDelegate(MethodInfo method)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var parameters = method.GetParameters();
            var argExpressions = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var accessor = Expression.ArrayIndex(argsParam, index);
                var converted = Expression.Convert(accessor, parameters[i].ParameterType);
                argExpressions[i] = converted;
            }

            var instanceCast = method.IsStatic ? null : Expression.Convert(instanceParam, method.DeclaringType!);
            var call = Expression.Call(instanceCast, method, argExpressions);

            var taskCall = Expression.Convert(call, typeof(Task));
            var lambda = Expression.Lambda<Func<object?, object[], Task>>(taskCall, instanceParam, argsParam);
            return lambda.Compile();
        }
    }
}
