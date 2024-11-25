using HotChocolate.Authorization;

namespace graphql_api_test.Models;

public class TransactionType : ObjectType<Transaction>
{
    
}


[Authorize]
public class Transaction
{
    public string Id { get; set; }
    public string Status { get; set; }
    
    [Authorize(Roles = new[] { "owner" })]
    public string Confidential { get; set; }
}