#if (Hosting)
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Nice3point.Revit.AddIn.Config.Options;
#endif
#if (Hosting && log)
using Microsoft.Extensions.Logging;
#endif
using Microsoft.Extensions.DependencyInjection;
#if (!NoWindow)
using Nice3point.Revit.AddIn.Views;
using Nice3point.Revit.AddIn.ViewModels;
#endif
#if (log && UseIoc)
using Nice3point.Revit.AddIn.Config.Logging;
#endif

namespace Nice3point.Revit.AddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
#if (Container)
    private static IServiceProvider? _serviceProvider;
#endif
#if (Hosting)
    private static IHost? _host;
#endif

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static void Start()
    {
#if (Container)
        var services = new ServiceCollection();
#if (log)

        //Logging
        services.AddSerilogConfiguration();
#endif
#if (!NoWindow)

        //MVVM
        services.AddTransient<Nice3point.Revit.AddInViewModel>();
        services.AddTransient<Nice3point.Revit.AddInView>();
#endif

        _serviceProvider = services.BuildServiceProvider();
#endif
#if (Hosting)
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location),
            DisableDefaults = true
        });
#if (log)

        //Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilogConfiguration();
#endif

        //Options
        builder.Services.AddApplicationOptions();
#if (!NoWindow)

        //MVVM
        builder.Services.AddTransient<Nice3point.Revit.AddInViewModel>();
        builder.Services.AddTransient<Nice3point.Revit.AddInView>();
#endif

        _host = builder.Build();
        _host.Start();
#endif
    }
#if (Hosting)

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static void Stop()
    {
        _host!.StopAsync().GetAwaiter().GetResult();
    }
#endif

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
#if (Container)
        return _serviceProvider!.GetRequiredService<T>();
#endif
#if (Hosting)
        return _host!.Services.GetRequiredService<T>();
#endif
    }
}