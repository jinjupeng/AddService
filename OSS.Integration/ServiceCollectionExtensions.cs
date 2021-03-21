using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetModular.Lib.OSS.Abstractions;
using NetModular.Lib.Utils.Core.Helpers;
using System.Linq;

namespace NetModular.Lib.OSS.Integration
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加OSS功能
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static IServiceCollection AddOSS(this IServiceCollection services, IConfiguration cfg)
        {
            var config = new OSSConfig();
            // 从appsetting.json中读取
            var section = cfg.GetSection("OSS");
            if (section != null)
            {
                // 配置数据绑定实体类
                section.Bind(config);
            }

            // 如果未启用oss服务
            if (!config.IsEnabled)
            {
                return services;
            }

            if (config.Aliyun != null && config.Aliyun.Domain.NotNull() && !config.Aliyun.Domain.EndsWith("/"))
            {
                config.Aliyun.Domain += "/";
            }

            services.AddSingleton(config);

            var assembly = AssemblyHelper.LoadByNameEndString($"OSS.{config.Provider}");
            if (assembly == null)
                return services;

            var providerType = assembly.GetTypes().FirstOrDefault(m => typeof(IFileStorageProvider).IsAssignableFrom(m));
            if (providerType != null)
            {
                services.AddSingleton(typeof(IFileStorageProvider), providerType);
            }

            return services;
        }
    }
}