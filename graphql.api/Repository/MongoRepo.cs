using graphql_api_test.Models;
using MongoDB.Driver;

namespace graphql_api_test.Repository;

public class MongoRepo
{
    private readonly MongoClient _client;
        
    public MongoRepo()
    {
        var settings = new MongoClientSettings
        {
            Server = new MongoServerAddress("localhost", 27017),
            DirectConnection = true,
        };
        
        _client = new MongoClient(settings);

        if (false)
        {
            var list = new List<Transaction>();
        
            for (var i = 0; i < 10; i++)
            {
         
                list.Add(new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = "active",
                    Confidential = "idk"
                });
            }
        
            _client.GetDatabase("graphql").GetCollection<Transaction>("transactions").InsertMany(list);
        }
    }

    public async Task<IList<Transaction>> Get()
    {
        var doc = await _client.GetDatabase("graphql").GetCollection<Transaction>("transactions").FindAsync(x => true);
        return doc.ToList();
    }

    public MongoClient GetClient()
    {
        return _client;
    }
    
}