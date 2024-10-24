using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

namespace DOTSRTS.Utilities.Mono.GameObjects
{
    public class QueryProvider<T> : IQueryProvider, IDisposable
    {
        [ShowInInspector]private IEnumerable<T> _data;
        private bool _disposed;
        private readonly object _lock = new object();
        private CancellationTokenSource _cancellationTokenSource = new();

        public QueryProvider(IEnumerable<T> data)
        {
            _data = data;
        }

        // CreateQuery methods for IQueryable support
        public IQueryable CreateQuery(Expression expression) => new Query<T>(this);

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression) =>
            new Query<TResult>(new QueryProvider<TResult>(_data.Cast<TResult>()));

        // Execute methods for query execution
        public object Execute(Expression expression)
        {
            // This method should interpret the expression and execute the query.
            return ExecuteTask().Result; // Blocking for synchronous execution
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(QueryProvider<T>));

            // Check if the expression is a MethodCallExpression for Contains
            if (expression is MethodCallExpression { Method: { Name: "Contains" } } methodCallExpression)
            {
                // Extract the arguments of the Contains method
                var arguments = methodCallExpression.Arguments;
                if (arguments.Count == 2)
                {
                    // The first argument should be the query source, and the second is the item to check for
                    var itemToCheck = Expression.Lambda(arguments[1]).Compile().DynamicInvoke();
                    var collection = _data;

                    // Perform the Contains check
                    bool containsResult = collection.Contains((T)itemToCheck);
                    return (TResult)(object)containsResult; // Cast to TResult, which should be a Boolean
                }
            }

            // Fallback for other expressions (non-Contains queries)
            var result = ExecuteTask().Result;

            if (typeof(TResult).IsAssignableFrom(typeof(IEnumerable<T>)))
            {
                return (TResult)(object)result;
            }
            else if (typeof(TResult) == typeof(T))
            {
                return (TResult)(object)result.FirstOrDefault();
            }

            throw new InvalidCastException($"Cannot cast result to type {typeof(TResult).Name}");
        }


        // Handle single item execution for non-collection TResult
        private T ExecuteSingle(Expression expression)
        {
            // Here we assume that the query expression filters down to a single element.
            // In a real-world scenario, this would be a more complex expression interpreter.
            return _data.FirstOrDefault();
        }

        // Thread-safe parallel execution using Task.Run and ConcurrentBag
        private async Task<List<T>> ExecuteTask()
        {
            var result = new ConcurrentBag<T>(); // Thread-safe collection for parallel access
            var token = _cancellationTokenSource.Token;

            var tasks = _data.Select(item => Task.Run(() =>
            {
                if (token.IsCancellationRequested) return;

                result.Add(item); // Thread-safe addition
            }, token)).ToList();

            await Task.WhenAll(tasks);
            return result.ToList(); // Convert ConcurrentBag to List
        }

        // IDisposable implementation
        public void Dispose()
        {
            if (_disposed) return;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _data = null;
            _disposed = true;
        }
    }
}
