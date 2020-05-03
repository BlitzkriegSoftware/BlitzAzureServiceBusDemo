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


## Desirable practices

1. Make sure your queues are configured before attempting to read or write to it
2. Make sure your writes succeed 
3. Make sure your reads are "leased" e.g. if your unit-of-work does not complete, the item comes back on the queue e.g. require an acknoledgement of message processing
4. Pay attention to your queue settings for your use case
 

# About 

Stuart Williams

* Cloud/DevOps Practice Lead
 
* Magenic Technologies Inc.
* Office of the CTO
 
* <a href="mailto:stuartw@magenic.com" target="_blank">stuartw@magenic.com</a> (e-mail)

* Blog: <a href="https://blitzkriegsoftware.azurewebsites.net/Blog" target="_blank">http://blitzkriegsoftware.net/Blog</a> 
* LinkedIn: <a href="http://lnkd.in/P35kVT" target="_blank">http://lnkd.in/P35kVT</a> 

* YouTube: <a href="https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg" target="_blank">https://www.youtube.com/channel/UCO88zFRJMTrAZZbYzhvAlMg</a> 
