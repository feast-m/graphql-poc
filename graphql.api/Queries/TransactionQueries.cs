using graphql_api_test.Models;
using graphql_api_test.Repository;

namespace graphql_api_test.Queries;

public class TransactionQueries
{
    private readonly MongoRepo _repository;
    
    public TransactionQueries(MongoRepo repository)
    {
        _repository = repository;
    }
    
    public async Task<IList<Transaction>> GetTransactions()
    {
        return await _repository.Get();
    }
}