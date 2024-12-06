using MongoDB.Driver;
using Rabbit.Models;
namespace DBRepository;

public class MongoRepository : IStatisticsRepository
{
    private readonly IMongoDatabase _database;

    public MongoRepository()
    {
        const string connectionUri = "mongodb+srv://Falastin:password@cluster0.vpt1l.mongodb.net/";
        var client = new MongoClient(connectionUri);
        _database = client.GetDatabase("Statistics");
    }

    public async Task Add(ReciverServerStatistics receiverServerStatistics)
    {
        var collection = _database.GetCollection<ReciverServerStatistics>("ServerStatistics");
        await collection.InsertOneAsync(receiverServerStatistics);
    }
}
