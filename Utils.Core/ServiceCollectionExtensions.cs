using Microsoft.Extensions.DependencyInjection;
using NetModular.Lib.Utils.Core.Attributes;
using NetModular.Lib.Utils.Core.Helpers;
using System;
using System.Linq;
using System.Reflection;

namespace NetModular.Lib.Utils.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 从指定程序集中注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            /*
             * 1、官方注入的时候，仅支持一个一个注入，没办法注入整个程序集的(Autofac 可以~)，所以为了方便，做了一个简单的封装，框架会自动将包含SingletonAttribute特性的类使用单例模式注入到容器中。但是该功能仅扫描框架定义好的结构，比如 Domian、Application、Infrastructure、Web 这些程序集，如果你是自己定义了一个程序集，则需要用到下面的方式
             * 2、IServiceCollection添加了一个扩展方法AddSingletonFromAssembly，它接收一个Assembly类型的参数，该方法会自动扫描所有包含SingletonAttribute特性的类，使用单例模式注入到容器中。
             * 3、原则上，能够使用单例注入的，不再使用静态类的方式。
             * 4、框架的所有功能基本都采用了依赖注入，比如缓存(Cache)、OSS等等，在这些库里面，都会有一个ServiceCollectionExtensions.cs扩展类，里面就是当前库使用的注入信息。
             * 
             */
            foreach (var type in assembly.GetTypes())
            {
                #region ==单例注入==

                // 如果类属性声明为[Singleton]时，则使用单例注入方式直接注入
                var singletonAttr = (SingletonAttribute)Attribute.GetCustomAttribute(type, typeof(SingletonAttribute));
                if (singletonAttr != null)
                {
                    //注入自身类型
                    if (singletonAttr.Itself)
                    {
                        services.AddSingleton(type);
                        continue;
                    }
                    // C#中的Type.GetInterfaces()方法用于获取当前Type实现或继承的所有接口
                    var interfaces = type.GetInterfaces().Where(m => m != typeof(IDisposable)).ToList();
                    if (interfaces.Any())
                    {
                        foreach (var i in interfaces)
                        {

                            //Console.WriteLine(type);
                            services.AddSingleton(i, type);
                        }
                    }
                    else
                    {
                        Console.WriteLine(type);
                        services.AddSingleton(type);
                    }

                    continue;
                }

                #endregion

                #region ==瞬时注入==

                var transientAttr = (TransientAttribute)Attribute.GetCustomAttribute(type, typeof(TransientAttribute));
                if (transientAttr != null)
                {
                    //注入自身类型
                    if (transientAttr.Itself)
                    {
                        services.AddSingleton(type);
                        continue;
                    }

                    var interfaces = type.GetInterfaces().Where(m => m != typeof(IDisposable)).ToList();
                    if (interfaces.Any())
                    {
                        foreach (var i in interfaces)
                        {
                            services.AddTransient(i, type);
                        }
                    }
                    else
                    {
                        services.AddTransient(type);
                    }
                    continue;
                }

                #endregion

                #region ==Scoped注入==
                var scopedAttr = (ScopedAttribute)Attribute.GetCustomAttribute(type, typeof(ScopedAttribute));
                if (scopedAttr != null)
                {
                    //注入自身类型
                    if (scopedAttr.Itself)
                    {
                        services.AddSingleton(type);
                        continue;
                    }

                    var interfaces = type.GetInterfaces().Where(m => m != typeof(IDisposable)).ToList();
                    if (interfaces.Any())
                    {
                        foreach (var i in interfaces)
                        {
                            services.AddScoped(i, type);
                        }
                    }
                    else
                    {
                        services.AddScoped(type);
                    }
                }

                #endregion
            }

            return services;
        }

        /// <summary>
        /// 注入所有服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNetModularServices(this IServiceCollection services)
        {
            var assemblies = AssemblyHelper.Load();
            foreach (var assembly in assemblies)
            {
                services.AddServicesFromAssembly(assembly);
            }
            return services;
        }
    }
}
