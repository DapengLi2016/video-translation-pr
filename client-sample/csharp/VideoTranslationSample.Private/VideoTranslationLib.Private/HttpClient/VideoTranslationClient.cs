//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.DataContracts;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class VideoTranslationClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public VideoTranslationClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "videotranslations";

    public async Task<IFlurlResponse> DeleteTranslationAsync(
        Guid translationId)
    {
        var url = BuildRequestBase()
            .AppendPathSegment(translationId.ToString());

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .DeleteAsync()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<Translation> GetTranslationAsync(
        Guid id,
        IReadOnlyDictionary<string, string> additionalHeaders)
    {
        var url = BuildRequestBase(additionalHeaders);

        url = url.AppendPathSegment(id.ToString());

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ReceiveJson<Translation>()
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

    public async Task<PaginatedResources<TranslationBrief>> GetTranslationsAsync()
    {
        var url = BuildRequestBase();

        return await RequestWithRetryAsync(async () =>
        {
            // var responseJson = await url.GetStringAsync().ConfigureAwait(false);
            return await url.GetAsync()
                .ReceiveJson<PaginatedResources<TranslationBrief>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<Translation> CreateTranslationAsync(
        TranslationCreate translation,
        string sourceLocaleWebVttFilePath,
        IReadOnlyDictionary<string, string> filePaths,
        IReadOnlyDictionary<string, string> additionalProperties = null,
        IReadOnlyDictionary<string, string> additionalHeaders = null)
    {
        return await RequestWithRetryAsync(async () =>
        {
            var response = CreateTranslationWithResponseAsync(
                translation: translation,
                sourceLocaleWebVttFilePath: sourceLocaleWebVttFilePath,
                filePaths: filePaths,
                additionalProperties: additionalProperties,
                additionalHeaders: additionalHeaders);
            return await response.ReceiveJson<Translation>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<string> CreateTranslationWithStringResponseAsync(
        TranslationCreate translation,
        string sourceLocaleWebVttFilePath,
        IReadOnlyDictionary<string, string> filePaths,
        IReadOnlyDictionary<string, string> additionalProperties = null,
        IReadOnlyDictionary<string, string> additionalHeaders = null)
    {
        return await RequestWithRetryAsync(async () =>
        {
            var response = CreateTranslationWithResponseAsync(
                translation: translation,
                sourceLocaleWebVttFilePath: sourceLocaleWebVttFilePath,
                filePaths: filePaths,
                additionalProperties: additionalProperties,
                additionalHeaders: additionalHeaders);
            return await response.ReceiveString()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    protected async Task<IFlurlResponse> CreateTranslationWithResponseAsync(
        TranslationCreate translation,
        string sourceLocaleWebVttFilePath,
        IReadOnlyDictionary<string, string> filePaths,
        IReadOnlyDictionary<string, string> additionalProperties = null,
        IReadOnlyDictionary<string, string> additionalHeaders = null)
    {
        ArgumentNullException.ThrowIfNull(translation);

        if (string.IsNullOrWhiteSpace(translation.DisplayName))
        {
            throw new ArgumentNullException(nameof(translation.DisplayName));
        }

        if (translation.TargetLocales != null && filePaths != null)
        {
            foreach (var targetLocale in translation.TargetLocales.Where(x => !string.IsNullOrEmpty(x.Value?.WebVttFileName)))
            {
                if (!filePaths.ContainsKey(targetLocale.Value.WebVttFileName))
                {
                    throw new InvalidDataException($"Not found file {targetLocale.Value.WebVttFileName} for locale {targetLocale.Key}");
                }

                if (!File.Exists(filePaths[targetLocale.Value.WebVttFileName]))
                {
                    throw new FileNotFoundException(filePaths[targetLocale.Value.WebVttFileName]);
                }
            }
        }

        var url = BuildRequestBase(additionalHeaders);

        return await url.PostMultipartAsync(mp =>
        {
            if (!string.IsNullOrWhiteSpace(translation.DisplayName))
            {
                mp.AddString(nameof(TranslationCreate.DisplayName), translation.DisplayName);
            }

            if (!string.IsNullOrWhiteSpace(translation.Description))
            {
                mp.AddString(nameof(TranslationCreate.Description), translation.Description);
            }

            if (!string.IsNullOrEmpty(translation.EnableFeatures))
            {
                mp.AddString(nameof(TranslationCreate.EnableFeatures), translation.EnableFeatures);
            }

            if (!string.IsNullOrEmpty(translation.ProfileName))
            {
                mp.AddString(nameof(TranslationCreate.ProfileName), translation.ProfileName);
            }

            if (!string.IsNullOrEmpty(translation.ProfileName))
            {
                mp.AddString(nameof(TranslationCreate.ProfileName), translation.ProfileName);
            }

            if ((translation.BaseTargetLocaleId ?? Guid.Empty) != Guid.Empty)
            {
                mp.AddString(nameof(TranslationCreate.BaseTargetLocaleId), translation.BaseTargetLocaleId.Value.ToString());
            }

            if (translation.AlignKind != null)
            {
                mp.AddString(nameof(TranslationCreate.AlignKind), translation.AlignKind.Value.AsString());
            }

            if (translation.VoiceKind != null)
            {
                mp.AddString(nameof(TranslationCreate.VoiceKind), translation.VoiceKind.Value.AsString());
            }

            if (translation.WithoutSubtitleInTranslatedVideoFile ?? false)
            {
                mp.AddString(nameof(TranslationCreate.WithoutSubtitleInTranslatedVideoFile),
                    translation.WithoutSubtitleInTranslatedVideoFile.Value.ToString());
            }

            if (translation.TargetLocaleSubtitleMaxCharCountPerSegment != null)
            {
                mp.AddString(nameof(TranslationCreate.TargetLocaleSubtitleMaxCharCountPerSegment),
                    translation.TargetLocaleSubtitleMaxCharCountPerSegment.Value.ToString());
            }

            if (translation.ExportPersonalVoicePromptAudioMetadata ?? false)
            {
                mp.AddString(nameof(TranslationCreate.ExportPersonalVoicePromptAudioMetadata),
                    translation.ExportPersonalVoicePromptAudioMetadata.Value.ToString());
            }

            if (translation.SpeakerCount != null)
            {
                mp.AddString(nameof(TranslationCreate.SpeakerCount),
                    translation.SpeakerCount.Value.ToString());
            }

            if (translation.EnableLipSync)
            {
                mp.AddString(nameof(TranslationCreate.EnableLipSync),
                    translation.EnableLipSync.ToString());
            }

            if (!string.IsNullOrEmpty(translation.PersonalVoiceModelName))
            {
                mp.AddString(nameof(TranslationCreate.PersonalVoiceModelName),
                    translation.PersonalVoiceModelName);
            }

            if (translation.IsAssociatedWithTargetLocale ?? false)
            {
                mp.AddString(nameof(TranslationCreate.IsAssociatedWithTargetLocale),
                    translation.IsAssociatedWithTargetLocale.Value.ToString());
            }

            if (translation.IsNewTargetLocaleCreation ?? false)
            {
                mp.AddString(nameof(TranslationCreate.IsNewTargetLocaleCreation),
                    translation.IsNewTargetLocaleCreation.Value.ToString());
            }

            if (translation.WebvttSourceKind != null)
            {
                mp.AddString(nameof(TranslationCreate.WebvttSourceKind),
                    translation.WebvttSourceKind.Value.AsString());
            }

            if (!string.IsNullOrWhiteSpace(translation.TtsCustomLexiconFileUrl?.OriginalString))
            {
                mp.AddString(nameof(TranslationCreate.TtsCustomLexiconFileUrl),
                    translation.TtsCustomLexiconFileUrl.OriginalString);
            }

            if (translation.DisableCache ?? false)
            {
                mp.AddString(nameof(TranslationCreate.DisableCache), bool.TrueString);
            }

            if ((translation.TtsCustomLexiconFileIdInAudioContentCreation ?? Guid.Empty) != Guid.Empty)
            {
                mp.AddString(nameof(TranslationCreate.TtsCustomLexiconFileIdInAudioContentCreation),
                    translation.TtsCustomLexiconFileIdInAudioContentCreation.ToString());
            }

            if (additionalProperties != null)
            {
                foreach (var (name, value) in additionalProperties)
                {
                    mp.AddString(name, value);
                }
            }

            if (!string.IsNullOrEmpty(sourceLocaleWebVttFilePath))
            {
                if (!File.Exists(sourceLocaleWebVttFilePath))
                {
                    throw new FileNotFoundException(sourceLocaleWebVttFilePath);
                }

                mp.AddFile("sourceLocaleWebVttFile", sourceLocaleWebVttFilePath, fileName: Path.GetFileName(sourceLocaleWebVttFilePath));
            }

            mp.AddJson(nameof(TranslationCreate.TargetLocales) + "JsonString", translation.TargetLocales);

            if (translation.Consents?.Any() ?? false)
            {
                mp.AddJson(nameof(TranslationCreate.Consents) + "JsonString", translation.Consents);
            }

            mp.AddString(nameof(TranslationCreate.VideoFileId), translation.VideoFileId.ToString());
            if (filePaths != null)
            {
                foreach (var (name, path) in filePaths.Where(x => translation.TargetLocales.Any(y =>
                    string.Equals(x.Key, y.Value.WebVttFileName, StringComparison.Ordinal))))
                {
                    mp.AddFile("files", path, fileName: name);
                }
            }
        }).ConfigureAwait(false);
    }
}
