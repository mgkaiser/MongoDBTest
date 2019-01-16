using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDBTest.Classes;
using MongoDBTest.Model;

namespace MongoDBTest.Context
{
    public class NoteContext : INoteContext
    {
        private readonly IMongoDatabase _database = null;

        public NoteContext(IOptions<Settings> settings, MongoClient client)
        {            
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Note> Notes
        {
            get
            {
                return _database.GetCollection<Note>("Note");
            }
        }
    }
}