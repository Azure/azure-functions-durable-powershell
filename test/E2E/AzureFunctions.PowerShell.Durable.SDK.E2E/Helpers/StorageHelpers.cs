// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    class StorageHelpers
    {
        public static QueueClient _queueClient = new QueueClient(Constants.Queue.StorageConnectionStringSetting, Constants.Queue.QueueName);

        public async static Task ClearQueue()
        {
            if (await _queueClient.ExistsAsync())
            {
                await _queueClient.ClearMessagesAsync();
            }
        }

        public async static Task<string> ReadFromQueue()
        {
            QueueMessage retrievedMessage = await _queueClient.ReceiveMessageAsync();
            await _queueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt);
            return retrievedMessage.Body.ToString();
        }
    }
}
