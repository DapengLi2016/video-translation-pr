//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.Advanced.HttpClient;

using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;
using System;
using System.Threading.Tasks;

public class ConfigurationClient : HttpClientBase
{
    public ConfigurationClient(HttpClientConfigBase config)
        : base(config)
    {
    }

    public override string ControllerName => "configurations";

    public async Task<EventHubConfig> CreateOrUpdateEventHubConfigAsync(EventHubConfig config)
    {
        var url = BuildRequestBase()
            .AppendPathSegment("event-hub");

        Console.WriteLine(url.Url);
        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .PutJsonAsync(config)
                .ReceiveJson<EventHubConfig>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<EventHubConfig> GetEventHubConfigAsync()
    {
        var url = BuildRequestBase()
            .AppendPathSegment("event-hub");

        Console.WriteLine(url.Url);
        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetJsonAsync<EventHubConfig>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task PingEventHubAsync()
    {
        var url = BuildRequestBase()
            .AppendPathSegment("event-hub:ping");

        Console.WriteLine(url.Url);
        await RequestWithRetryAsync(async () =>
        {
            return await url
                .PostAsync()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
