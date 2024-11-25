using graphql_api_test.Models;
using graphql_api_test.Repository;
using HotChocolate.Subscriptions;
using MongoDB.Driver;

namespace graphql_api_test.Subscriptions;

public class MongoWatcher
{
    private readonly MongoRepo _repository;
    private readonly ITopicEventSender  _sender;
    
    private int subscriptions;
    private CancellationTokenSource watcherCancellationTokenSource;

    public MongoWatcher([Service]ITopicEventSender sender, MongoRepo repository)
    {
        _repository = repository;
        _sender = sender;
    }

    public void MaybeStartWatching(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => MaybeStopWatching(), true);
        
        if (subscriptions == 0)
        {
            StartWatching();
        }
        
        subscriptions++;
    }

    private void StartWatching()
    {
        watcherCancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Transaction>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Update);
        
            var changeStreamOptions = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };
            
            using var cursor = await _repository.GetClient().GetDatabase("graphql").GetCollection<Transaction>("transactions").WatchAsync(pipeline, changeStreamOptions);
        
            await cursor.ForEachAsync(change =>
            {  
                _sender.SendAsync("TransactionMutated", change.FullDocument);
            }, watcherCancellationTokenSource.Token);
        }, watcherCancellationTokenSource.Token);
    }

    private void MaybeStopWatching()
    {
        subscriptions--;

        if (subscriptions == 0)
        {
            watcherCancellationTokenSource.Cancel();
        }
    }
}