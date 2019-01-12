﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
                    .CreateLogger();

                    // TODO: Add Serilog file logging

                    configLogging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));                        
                    configLogging.AddConsole();                        
                    configLogging.AddSerilog();
                })
                .RunConsoleAsync();
        }    
    }
}

