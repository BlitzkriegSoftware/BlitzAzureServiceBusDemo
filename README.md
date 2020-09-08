# Blitz Azure Service Bus Demo

A Brief Demo of Using Azure Service Bus.

## Settin up to run unit tests

create a file in the `Blitz.Azure.ServiceBus.Library.Tests` folder called `testconfig.json`

Populate it with this layout and supply your connection string for ASB in Azure that has the `Manage` role.

```json
{
  "ConnectionString": "Endpoint=sb://..."
}
```

Right Click on the file in Visual Studio, select properties and make the file `content` and `copy always`

## Run the tests

Three outcomes:
* **CompleteAsync** - Message processed successfully, dequeue it for real
* **AbandonAsync** - Unable to process the message, put it back on the queue
* **DeadLetterAsync** - This message is hopeless, put it in the dead-letter-queue (generally malformed somehow)

## Desirable practices

1. Make sure your queues are configured before attempting to read or write to it
2. Make sure your writes succeed 
3. Make sure your reads are "leased" e.g. if your unit-of-work does not complete, the item comes back on the queue e.g. require an acknoledgement of message processing
4. Pay attention to your queue settings for your use case

## Simulating putting back messages w. a delay

```cs
OnNewMessage(model, out bool isOk);
// Complete message
await client.CompleteAsync(message.SystemProperties.LockToken);

// If not processed ok, requeue it w. a delay
if (!isOk) {
    // Back-off exponentially 
    var secondsToDelay = (int) Math.Pow(2, message.SystemProperties.DeliveryCount) + 1;
    var ts = new TimeSpan(0, 0, secondsToDelay);

    // Clone Message
    var messageNew = MakeMessage(model, model.Id);

    // Resend
    await client.ScheduleMessageAsync(messageNew, DateTime.UtcNow.Addts));
}
```

# About 

Stuart Williams

* Cloud/DevOps Practice Lead

* Magenic Technologies Inc.
* Office of the CTO

* [e-mail](stuartw@magenic.com)

* [Blog](https://blitzkriegsoftware.azurewebsites.net/Blog) 
* [LinkedIn](http://lnkd.in/P35kVT)

* [YouTube](https://www.youtube.com/user/spookdejur1962/videos)
