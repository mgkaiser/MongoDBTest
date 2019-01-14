using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace MongoDBTest.Service
{
    public class MongoDBService : IHostedService, IDisposable
    {
        private readonly ILogger<MongoDBService> _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly MongoClient _mongoClient;

        public MongoDBService(ILogger<MongoDBService> logger, IApplicationLifetime applicationLifetime, MongoClient mongoClient)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _mongoClient = mongoClient;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{CALL_STATUS} {CLASS_NAME} {METHOD_TYPE}", "STARTED", "MongoDBService", "SERVICE");            

            await Task.Run(() => DoWork(), cancellationToken);                   
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{CALL_STATUS} {CLASS_NAME} {METHOD_TYPE}", "ENDED", "MongoDBService", "SERVICE");            
            await Task.CompletedTask;
        }
        #endregion

        public async Task DoWork()
        {            
            for (int i =0; i < 120; i++)
            {
                if (_applicationLifetime.ApplicationStopping.IsCancellationRequested) break;

                _logger.LogInformation("{MethodName} {Iteration}", "DoWork", i);
                await Task.Delay(100);
            }
            
            _applicationLifetime.StopApplication();
        }

    }
}