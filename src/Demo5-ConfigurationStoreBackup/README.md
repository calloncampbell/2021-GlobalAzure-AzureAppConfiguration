# Configuration Store Backup Function App

1. We set up Event Grid events emitted by Azure App Configuration to be routed to an Azure Storage queue. Note that event order is not guaranteed by Event grid or storage queue.  
2. We create a timer triggered Azure function to read contents of storage queue every <10> minutes. In this tutorial, we are using the same storage account for this Azure function and Storage Queue. 
3. Azure function checks if there are any messages from event grid in the queue. Any other messages in the queue in invalid format will be discarded.
4. If there are any messages in the queue, retrieve a batch of maximum 32 messages at a time for processing.
5. For all retrieved messages, extract the key+label from event message and store them in a hash set. We need to keep track of any unique key+label that was modified or deleted.
6. Fetch all settings from primary store. 
7. Update only those settings in secondary store which have a corresponding event in the storage queue.
8. Delete all settings that were present in storage queue but not in primary store.
9. Delete all messages that we received in this batch from the storage queue.
10. Repeat from step 4 if there are any messages remaining in storage queue.

## Configuration

### Azure Function Configuration

The following are the necessary configuration settings to be added to the function app configuration:

```json
{    
    "PrimaryStoreEndpoint": "ADD_CONNECTION_STRING",
    "SecondaryStoreEndpoint": "ADD_CONNECTION_STRING",
    "StorageQueueUri": "ADD_QUEUE_URI"
}
```

Example:

- PrimaryStoreEndpoint: https://{store1}.azconfig.io
- SecondaryStoreEndpoint: https://{store2}.azconfig.io
- StorageQueueUri: https://{account_name}.queue.core.windows.net/{queue_name}

# References
https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-backup-config-store
https://github.com/Azure/AppConfiguration/tree/main/examples/ConfigurationStoreBackup