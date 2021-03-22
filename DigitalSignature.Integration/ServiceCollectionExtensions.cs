using DigitalSignature.Abstractions;
using DigitalSignature.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DigitalSignature.Integration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDigitalSignature(this IServiceCollection services)
        {
            var assembly = AssemblyHelper.LoadByNameEndString($"DigitalSignature");
            if (assembly == null)
                return services;

            var providerType = assembly.GetTypes().FirstOrDefault(m => typeof(IDigitalSignatureProvider).IsAssignableFrom(m));
            if (providerType != null)
            {
                services.AddSingleton(typeof(IDigitalSignatureProvider), providerType);
            }
            return services;
        }
    }
}
