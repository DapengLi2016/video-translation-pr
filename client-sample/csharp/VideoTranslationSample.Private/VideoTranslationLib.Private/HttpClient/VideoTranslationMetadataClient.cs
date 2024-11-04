//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using System;
using System.Threading.Tasks;

public class VideoTranslationMetadataClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public VideoTranslationMetadataClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "videotranslationmetadata";

    public async Task<VideoTranslationMetadata> QueryMetadataAsync()
    {
        var url = BuildRequestBase();

        return await RequestWithRetryAsync(async () =>
        {
            return await url.GetAsync()
               .ReceiveJson<VideoTranslationMetadata>()
               .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
