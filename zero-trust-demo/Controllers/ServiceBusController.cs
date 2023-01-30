using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace zero_trust_demo.Controllers;

public class ServiceBusController : ControllerBase
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly ServiceBusAdministrationClient _adminClient;

    public ServiceBusController(ServiceBusClient client, ServiceBusAdministrationClient adminClient,
        IOptions<ServiceBusOptions> options)
    {
        _options = options.Value;
        _client = client;
        _adminClient = adminClient;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetMessageCount()
    {
        var properties = await _adminClient.GetQueueRuntimePropertiesAsync(_options.Queue);
        return Content(properties.Value.TotalMessageCount.ToString());
    }

    [HttpPost("add/{message}")]
    public async Task<IActionResult> AddMessage(string message)
    {
        var sender = _client.CreateSender(_options.Queue);
        using var messageBatch = await sender.CreateMessageBatchAsync();
        var sbMessage = new ServiceBusMessage(message);
        messageBatch.TryAddMessage(sbMessage);
        await sender.SendMessagesAsync(messageBatch);
        return Ok();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create()
    {
        var exists = await _adminClient.QueueExistsAsync(_options.Queue);
        if (!exists.Value)
        {
            await _adminClient.CreateQueueAsync(_options.Queue);
        }

        return Ok();
    }

    [HttpDelete("purge")]
    public async Task<IActionResult> Purge()
    {
        var receiver = _client.CreateReceiver(_options.Queue);
        var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, TimeSpan.FromMilliseconds(500));
        while (receivedMessages.Count > 0)
        {
            foreach (var receivedMessage in receivedMessages)
            {
                await receiver.CompleteMessageAsync(receivedMessage);
            }

            receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, TimeSpan.FromMilliseconds(500));
        }

        return Accepted();
    }
}