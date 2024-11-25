using graphql_api_test.Models;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace graphql_api_test.Controllers;

[Authorize]
public class PlaceholderController : ControllerBase
{
    private readonly ITopicEventSender  _sender;
    
    public PlaceholderController([Service]ITopicEventSender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    [Route("/trigger")]
    public async Task<IActionResult> Trigger()
    {
        await _sender.SendAsync("TransactionMutated", new Transaction{});
        return Ok();
    }
}