using System;

namespace RadLine
{
    internal static class IServiceProviderExtensions
    {
        public static T? GetService<T>(this IServiceProvider provider)
            where T : class
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var result = provider.GetService(typeof(T));
            return result as T;
        }
    }
}
