using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDBTest.Model;
using MongoDBTest.Repository;

namespace MongoDBTest.Service
{
    public class MongoDBService : IHostedService, IDisposable
    {
        private readonly ILogger<MongoDBService> _logger;
        private readonly IApplicationLifetime _applicationLifetime;        
        private readonly INoteRepository _noteRepository;

        public MongoDBService(
            ILogger<MongoDBService> logger, 
            IApplicationLifetime applicationLifetime, 
            INoteRepository noteRepository)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _noteRepository = noteRepository;
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
            await _noteRepository.RemoveAllNotes();
            
            await AddWork();

            await ShowWork();

            await ChangeWork();
            
            await ShowWork();
                   
            _applicationLifetime.StopApplication();
        }

        public async Task AddWork()
        {
            for (int i =1; i < 1000; i++)
            {
                await _noteRepository.AddNote(new Note() { 
                    Id = i.ToString(), 
                    Body = $"Test note {i}", 
                    CreatedOn = DateTime.Now, 
                    UpdatedOn = DateTime.Now, 
                    UserId = i });
                
                _logger.LogInformation("{Action} {NoteId}", "Put", i);
            }
        }

        public async Task ChangeWork()
        {
            for (int i =1; i < 1000; i++)
            {
                await _noteRepository.UpdateNoteDocument(i.ToString(), Guid.NewGuid().ToString());
                _logger.LogInformation("{Action} {NoteId}", "Change", i);
            }            
        }

        public async Task ShowWork()
        {
            var notes = await _noteRepository.GetAllNotes();
            _logger.LogInformation("{NoteCount}", notes.Count());

            foreach(var note in notes)
            {
                _logger.LogInformation("{Action} {NoteId} {NoteBody}", "Get", note.Id, note.Body);
            }
        }

    }
}