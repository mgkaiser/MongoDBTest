﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using MongoDBTest.Service;
using MongoDB.Driver;
using MongoDBTest.Classes;
using MongoDBTest.Repository;
using MongoDBTest.Context;

namespace MongoDBTest
{
    class Program
    {       
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {                                       
                    // Register the service
                    services.AddHostedService<MongoDBService>();

                    // Get the config settings
                    services.Configure<Settings> (options => {
                        options.ConnectionString = hostContext.Configuration.GetSection("MongoConnection:ConnectionString").Value;
                        options.Database = hostContext.Configuration.GetSection("MongoConnection:Database").Value;
                    });
                    
                    // Register Mongo client
                    services.AddTransient<MongoClient>(serviceProvider => {
                        var settings = serviceProvider.GetRequiredService<IOptions<Settings>>();
                        return new MongoClient(settings.Value.ConnectionString);
                    });                     

                    // Register notes repository and context
                    services.AddTransient<INoteContext, NoteContext>();
                    services.AddTransient<INoteRepository, NoteRepository>();
                })
                .ConfigureAppConfiguration((hostContext, configApp) => {
                     configApp
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", reloadOnChange: true, optional: true);                                            
                })                
                .ConfigureLogging((hostContext, configLogging) =>
                {                    
                    Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", hostContext.Configuration.GetSection("ElasticConfiguration:Application")?.Value)
                        .Enrich.WithProperty("FriendlyName", System.AppDomain.CurrentDomain.FriendlyName)
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(hostContext.Configuration.GetSection("ElasticConfiguration:Uri")?.Value))
                        {
                            AutoRegisterTemplate = true,
                        })
                        .WriteTo.Console()                    
                        .WriteTo.File($"{hostContext.Configuration.GetSection("Serilog:LogRoot")?.Value}log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                                                        
                    configLogging.AddSerilog();
                })
                .RunConsoleAsync();
        }

        private static object MongoClient()
        {
            throw new NotImplementedException();
        }
    }
}

