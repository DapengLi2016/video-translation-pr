//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.Advanced.HttpClient;

using Flurl.Http;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;

public class SegmentClient : HttpClientBase
{
    public SegmentClient(HttpClientConfigBase config)
        : base(config)
    {
    }

    public override string ControllerName => "translations";

    public async Task<PagedSegment> GetSegmentsAsync(
        string translationId,
        string iterationId)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(translationId)
            .AppendPathSegment("iterations")
            .AppendPathSegment(iterationId)
            .AppendPathSegment("segments");

        Console.WriteLine(url.Url);
        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetJsonAsync<PagedSegment>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
