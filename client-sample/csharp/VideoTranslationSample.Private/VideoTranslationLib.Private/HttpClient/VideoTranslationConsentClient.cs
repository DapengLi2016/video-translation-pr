//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation;
using Microsoft.SpeechServices.DataContracts;
using System;
using System.Net;
using System.Threading.Tasks;

public class VideoTranslationConsentClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public VideoTranslationConsentClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "videotranslationconsents";

    public async Task<VideoTranslationConsent> GetConsentAsync(
        Guid id)
    {
        var url = BuildRequestBase();

        url = url.AppendPathSegment(id.ToString());

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ReceiveJson<VideoTranslationConsent>()
                    .ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }

                Console.Write($"Response failed with error: {await ex.GetResponseStringAsync().ConfigureAwait(false)}");
                throw;
            }
        }).ConfigureAwait(false);
    }

    public async Task<VideoTranslationConsent> CreateConsentAsync(
        VideoTranslationConsentCreate consent,
        string audioFilePath)
    {
        ArgumentNullException.ThrowIfNull(consent);
        if (string.IsNullOrEmpty(audioFilePath))
        {
            throw new ArgumentNullException(nameof(audioFilePath));
        }

        return await RequestWithRetryAsync(async () =>
        {
            var response = PostUploadConsentWithResponseAsync(
                consent: consent,
                audioFilePath: audioFilePath);

            return await response
                .ReceiveJson<VideoTranslationConsent>();
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<VideoTranslationConsent>> GetConsentsAsync()
    {
        var url = BuildRequestBase();
        return await RequestWithRetryAsync(async () =>
        {
            return await url.GetAsync()
                .ReceiveJson<PaginatedResources<VideoTranslationConsent>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<IFlurlResponse> DeleteConsentAsync(
        Guid consentId)
    {
        return await RequestWithRetryAsync(async () =>
        {
            return await DeleteByIdAsync(
                id: consentId.ToString()).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private async Task<IFlurlResponse> PostUploadConsentWithResponseAsync(
        VideoTranslationConsentCreate consent,
        string audioFilePath)
    {
        ArgumentNullException.ThrowIfNull(consent);

        var url = BuildRequestBase();

        url.ConfigureRequest(settings => settings.Timeout = VideoTranslationConstant.UploadVideoOrAudioFileTimeout);

        return await url
            .PostMultipartAsync(mp =>
            {
                if (!string.IsNullOrWhiteSpace(consent.DisplayName))
                {
                    mp.AddString(nameof(VideoTranslationConsentCreate.DisplayName), consent.DisplayName);
                }

                if (!string.IsNullOrWhiteSpace(consent.Description))
                {
                    mp.AddString(nameof(VideoTranslationConsentCreate.Description), consent.Description);
                }

                if (!string.IsNullOrEmpty(consent.Locale?.Name))
                {
                    mp.AddString(nameof(VideoTranslationConsentCreate.Locale), consent.Locale.Name);
                }

                mp.AddFile("audioFile", audioFilePath);
            });
    }
}
