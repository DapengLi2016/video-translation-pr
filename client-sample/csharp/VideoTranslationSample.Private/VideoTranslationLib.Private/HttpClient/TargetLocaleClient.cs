//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation;
using Microsoft.SpeechServices.CustomVoice;
using Microsoft.SpeechServices.DataContracts;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using Microsoft.SpeechServices.VideoTranslation.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

public class TargetLocaleClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public TargetLocaleClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "videofiletargetlocales";


    public async Task<VideoFileTargetLocale> QueryTargetLocaleAsync(
        Guid id)
    {
        var url = BuildRequestBase();
        return await RequestWithRetryAsync(async () =>
        {
            return await url
               .AppendPathSegment(id)
               .GetAsync()
               .ReceiveJson<VideoFileTargetLocale>()
               .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<VideoFileTargetLocaleBrief>> QueryTargetLocalesAsync(
        int skip = 0,
        int top = 100,
        string orderby = "lastActionDateTime desc")
    {
        var url = BuildRequestBase();
        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .SetQueryParam("skip", skip)
                .SetQueryParam("top", top)
                .SetQueryParam("orderby", orderby)
                .GetAsync()
                .ReceiveJson<PaginatedResources<VideoFileTargetLocaleBrief>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }


    public async Task<VideoFileTargetLocale> UpdateTargetLocaleAsync(
        Guid targetLocaleId,
        VideoFileTargetLocaleUpdate targetLocaleUpdate)
    {
        var url = BuildRequestBase();
        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .AppendPathSegment(targetLocaleId)
                .PatchJsonAsync(targetLocaleUpdate)
                .ReceiveJson<VideoFileTargetLocale>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<TargetLocaleEditingWebvttFileMetadata> UpdateTargetLocaleEdittingWebvttFileAsync(
        Guid id,
        VideoTranslationWebVttFilePlainTextKind? kind,
        string webvttFilePath)
    {
        if (string.IsNullOrEmpty(webvttFilePath))
        {
            throw new ArgumentNullException(webvttFilePath);
        }

        if (!File.Exists(webvttFilePath))
        {
            throw new FileNotFoundException(webvttFilePath);
        }

        var url = BuildRequestBase()
            .AppendPathSegment(id.ToString())
            .AppendPathSegment("webvtt");
        if ((kind ?? VideoTranslationWebVttFilePlainTextKind.None) != VideoTranslationWebVttFilePlainTextKind.None)
        {
            url = url.SetQueryParam("kind", kind.Value.AsString());
        }

        return await RequestWithRetryAsync(async () =>
            {
                return await url
                    .PostMultipartAsync(mp =>
                    {
                        mp.AddFile("webVttFile", webvttFilePath);
                    });
            })
            .ReceiveJson<TargetLocaleEditingWebvttFileMetadata>()
            .ConfigureAwait(false);
    }

}
