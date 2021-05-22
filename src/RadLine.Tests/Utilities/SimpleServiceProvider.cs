using System;
using System.Collections.Generic;

namespace RadLine.Tests.Utilities
{
    public sealed class SimpleServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _registrations;

        public SimpleServiceProvider()
        {
            _registrations = new Dictionary<Type, object>();
        }

        public void Register<TService, TImplementation>(TImplementation implementation)
            where TImplementation : notnull
        {
            _registrations[typeof(TService)] = implementation;
        }

        public object? GetService(Type serviceType)
        {
            _registrations.TryGetValue(serviceType, out var result);
            return result;
        }
    }
}
