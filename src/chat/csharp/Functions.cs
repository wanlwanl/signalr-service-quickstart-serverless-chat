// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FunctionApp
{
    public static class Functions
    {
        // sample for replacing the host of Url property in negotiation response
        // you can replace any part of the Url in the same way
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "chat")] SignalRConnectionInfo connectionInfo)
        {
            // get a specific value by key in function app setting (local.settings.json for local development)
            var akamaiEndpoint = System.Environment.GetEnvironmentVariable("AkamaiHost", EnvironmentVariableTarget.Process);
            connectionInfo.Url = ReplaceHost(connectionInfo.Url, akamaiEndpoint);
            return connectionInfo;
        }

        [FunctionName("messages")]
        public static Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] object message,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { message }
                });
        }

        // sample only, you can replace any part you want
        public static string ReplaceHost(string original, string newHostName)
        {
            var builder = new UriBuilder(original);
            builder.Host = newHostName;
            return builder.Uri.ToString();
        }
    }
}
