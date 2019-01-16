using MongoDB.Driver;
using MongoDBTest.Model;

namespace MongoDBTest.Context
{
    public interface INoteContext
    {
        IMongoCollection<Note> Notes { get; }
    }
}