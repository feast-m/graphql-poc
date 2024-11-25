using graphql_api_test.Models;
using HotChocolate.Authorization;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Subscriptions;
using MongoDB.Driver;

namespace graphql_api_test.Subscriptions;

public class TransactionSubscriptionType : ObjectType
{
    private readonly MongoWatcher _watcher;
    
    public TransactionSubscriptionType(MongoWatcher watcher)
    {
        _watcher = watcher;
    }
    
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor
            .Field("transactionMutated")
            .Type<TransactionType>()
            .Resolve<Transaction>( context => context.GetEventMessage<Transaction>())
            .Subscribe(async context =>
            {
                var receiver = context.Service<ITopicEventReceiver>();

                var token = context.RequestAborted;

                ISourceStream stream =
                    await receiver.SubscribeAsync<Transaction>("TransactionMutated");
                
               _watcher.MaybeStartWatching(token);

                return stream;
            });
    }

   
    
}