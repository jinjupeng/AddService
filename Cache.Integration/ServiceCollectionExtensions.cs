using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetModular.Lib.Cache.Abstractions;
using NetModular.Lib.Utils.Core.Helpers;

namespace NetModular.Lib.Cache.Integration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration cfg)
        {
            var config = new CacheConfig();
            var section = cfg.GetSection("Cache");
            section?.Bind(config);

            services.AddSingleton(config);

            if (!config.IsEnabled) // 如果未启用缓存
            {
                return services;
            }

            var assembly = AssemblyHelper.LoadByNameEndString($"Cache.{config.Provider}");

            Check.NotNull(assembly, $"缓存实现程序集({config.Provider})未找到");

            var handlerType = assembly.GetTypes().FirstOrDefault(m => typeof(ICacheHandler).IsAssignableFrom(m));
            if (handlerType != null)
            {
                services.AddSingleton(typeof(ICacheHandler), handlerType);
            }

            return services;
        }
    }
}