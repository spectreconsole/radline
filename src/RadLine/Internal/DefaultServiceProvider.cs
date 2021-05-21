using System;
using System.Collections.Generic;

namespace RadLine
{
    internal sealed class DefaultServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider? _provider;
        private readonly Dictionary<Type, object> _registrations;

        public DefaultServiceProvider(IServiceProvider? provider)
        {
            _provider = provider;
            _registrations = new Dictionary<Type, object>();
        }

        public void RegisterOptional<TService, TImplementation>(TImplementation? implementation)
        {
            if (implementation != null)
            {
                _registrations[typeof(TService)] = implementation;
            }
        }

        public object? GetService(Type serviceType)
        {
            if (_provider != null)
            {
                var result = _provider.GetService(serviceType);
                if (result != null)
                {
                    return result;
                }
            }

            _registrations.TryGetValue(serviceType, out var registration);
            return registration;
        }
    }
}
