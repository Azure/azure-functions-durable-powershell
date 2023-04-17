// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    public class StorageHelpers
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
            var bytesArray = Convert.FromBase64String(retrievedMessage.Body.ToString());
            return Encoding.ASCII.GetString(bytesArray);
        }
    }
}
