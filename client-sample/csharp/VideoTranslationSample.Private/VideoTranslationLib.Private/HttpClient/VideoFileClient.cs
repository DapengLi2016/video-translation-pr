//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.Enums;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.DataContracts;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs.FileEntity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class VideoFileClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public VideoFileClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "videofiles";

    public async Task<TVideoFileMetadata> UploadVideoFileAsync<TVideoFileMetadata>(
        string name,
        string description,
        CultureInfo locale,
        int? speakerCount,
        string videoFilePath,
        Uri videoFileUrl)
        where TVideoFileMetadata : VideoFileMetadata
    {
        return await RequestWithRetryAsync(async () =>
        {
            var response = PostUploadVideoFileWithResponseAsync(
                name: name,
                description: description,
                locale: locale,
                speakerCount: speakerCount,
                videoFilePath: videoFilePath,
                videoFileUrl: videoFileUrl);

            return await response
                .ReceiveJson<TVideoFileMetadata>();
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<VideoFileMetadata>> QueryVideoFilesAsync()
    {
        var url = BuildRequestBase()
            // Set apiVersion to 2 to response status.
            .SetQueryParam("apiVersion", "2");

        return await RequestWithRetryAsync(async () =>
        {
            return await url.GetAsync()
                .ReceiveJson<PaginatedResources<VideoFileMetadata>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<IFlurlResponse> DeleteVideoFileAsync(
        Guid videoFileId,
        bool deleteAssociations)
    {
        var queryParams = new Dictionary<string, string>();
        if (deleteAssociations)
        {
            queryParams["deleteAssociations"] = bool.TrueString;
        }

        return await RequestWithRetryAsync(async () =>
        {
            return await DeleteByIdAsync(
                id: videoFileId.ToString(),
                queryParams: queryParams).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<PaginatedResources<Translation>> QueryTranslationsAsync(Guid videoFileId)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(videoFileId.ToString())
            .AppendPathSegment("translations");

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetAsync()
                .ReceiveJson<PaginatedResources<Translation>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<IFlurlResponse> DeleteTargetLocaleAsync(
        Guid videoFileId,
        CultureInfo locale,
        bool deleteAssociations)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(videoFileId.ToString())
            .AppendPathSegment(locale.Name);
        if (deleteAssociations)
        {
            url = url.SetQueryParam("deleteAssociations", deleteAssociations);
        }

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .DeleteAsync()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<WebVttFileMetadata> QueryTargetLocaleWebVttAsync(
        Guid videoFileId,
        CultureInfo locale)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(videoFileId.ToString())
            .AppendPathSegment(locale.Name)
            .AppendPathSegment("webvtt");

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .GetAsync()
                .ReceiveJson<WebVttFileMetadata>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<VideoFileMetadata> QueryVideoFileWithLocaleAndFileContentSha256Async(
        CultureInfo locale,
        string fileContentSha256)
    {
        if (locale == null || string.IsNullOrEmpty(locale.Name))
        {
            throw new ArgumentNullException(nameof(locale));
        }

        if (string.IsNullOrEmpty(fileContentSha256))
        {
            throw new ArgumentNullException(nameof(fileContentSha256));
        }

        var url = BuildRequestBase()
            .AppendPathSegment("QueryByFileContentSha256")
            .SetQueryParam("locale", locale.Name)
            .SetQueryParam("fileContentSha256", fileContentSha256)
            // Set apiVersion to 2 to response status.
            .SetQueryParam("apiVersion", "2");

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ReceiveJson<VideoFileMetadata>()
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

    public async Task<TVideoFileMetadata> QueryVideoFileAsync<TVideoFileMetadata>(Guid id)
        where TVideoFileMetadata : VideoFileMetadata
    {
        var url = BuildRequestBase();

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .AppendPathSegment(id.ToString())
                // Set apiVersion to 2 to response status.
                .SetQueryParam("apiVersion", "2")
                .GetAsync()
                .ReceiveJson<TVideoFileMetadata>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<TVideoFileMetadata> QueryVideoFileUntilTerminatedAsync<TVideoFileMetadata>(Guid id)
        where TVideoFileMetadata : VideoFileMetadata
    {
        var startTime = DateTime.Now;
        TVideoFileMetadata videoFile = null;
        OneApiState? lastState = null;
        do
        {
            videoFile = await this.QueryVideoFileAsync<TVideoFileMetadata>(id).ConfigureAwait(false);
            if (videoFile?.Status == null)
            {
                return videoFile;
            }

            if (videoFile.Status != lastState)
            {
                Console.WriteLine(videoFile.Status.Value.AsString());
                lastState = videoFile.Status;
            }

            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            Console.Write(".");
        }
        while (!new[] { OneApiState.Succeeded, OneApiState.Failed }.Contains(videoFile.Status.Value) &&
            DateTime.Now - startTime < CommonConst.Http.VideoTranslationTaskExpiredDuration);
        return videoFile;
    }

    public async Task<IFlurlResponse> PostUploadVideoFileWithResponseAsync(
        string name,
        string description,
        CultureInfo locale,
        int? speakerCount,
        string videoFilePath,
        Uri videoFileUrl)
    {
        if (string.IsNullOrEmpty(videoFilePath) && string.IsNullOrWhiteSpace(videoFileUrl?.OriginalString))
        {
            throw new ArgumentException($"Please provide either {nameof(videoFilePath)} or {nameof(videoFileUrl)}");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var url = BuildRequestBase();

        url.ConfigureRequest(settings => settings.Timeout = VideoTranslationConstant.UploadVideoOrAudioFileTimeout);

        return await url
            .PostMultipartAsync(mp =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    mp.AddString(nameof(VideoFileCreate.DisplayName), name);
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    mp.AddString(nameof(VideoFileCreate.Description), description);
                }

                if (locale != null && !string.IsNullOrEmpty(locale.Name))
                {
                    mp.AddString(nameof(VideoFileCreate.Locale), locale.Name);
                }

                if (speakerCount != null)
                {
                    mp.AddString(nameof(VideoFileCreate.SpeakerCount), speakerCount.Value.ToString(CultureInfo.InvariantCulture));
                }

                // When ApiVersion is null or < 2, for video file response, Status and LastActionDateTime is null for compatiable.
                // When ApiVersion >= 2, response Status and LastActionDateTime
                mp.AddString("apiVersion", "2");
                if (!string.IsNullOrWhiteSpace(videoFilePath))
                {
                    mp.AddFile("videoFile", videoFilePath);
                }
                else if (!string.IsNullOrWhiteSpace(videoFileUrl?.OriginalString))
                {
                    mp.AddString(nameof(VideoFileCreate.UploadKind), VideoTranslationFileUploadKind.AzureBlobUrl.AsString());
                    mp.AddString(nameof(VideoFileCreate.VideoOrAudioFileUrl), videoFileUrl.OriginalString);
                }
            });
    }
}
