// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    public static class Constants
    {
        public const string FUNC_PATH = "FUNC_PATH";
        public const string DurableAppCollectionName = "DurableAppCollection";
        public static string FunctionsHostUrl = Environment.GetEnvironmentVariable("FunctionAppUrl") ?? "http://localhost:7071";
        
        // Queue tests
        public static class Queue
        {
            public static string QueueName = "outqueue";
            public static string StorageConnectionStringSetting = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            public static string OutputBindingName = "test-output-ps";
            public static string InputBindingName = "test-input-ps";
        }    
    }
}
