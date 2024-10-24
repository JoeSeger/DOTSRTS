using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DOTSRTS.Utilities.Mono.GameObjects
{
    public readonly struct Query<T> : IQueryable<T>, IDisposable
    {
        private readonly QueryProvider<T> _provider;
        private readonly IEnumerable<T> _data;

        public Query(IEnumerable<T> data) : this()
        {
            var enumerable = data as T[] ?? data.ToArray();
            _provider = new QueryProvider<T>(enumerable);
            _data = enumerable;
            Expression = CreateExpression(); // Initialize the expression after all fields
        }

        public Query(QueryProvider<T> provider) : this()
        {
            _provider = provider;
            Expression = CreateExpression(); // Initialize the expression after all fields
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider => _provider;

        private Expression CreateExpression() => Expression.Constant(this, typeof(IQueryable<T>));

        public IEnumerator<T> GetEnumerator() => _provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            _provider?.Dispose();
        }

        public bool IsCreated => _provider != null && _data != null;
    }
}