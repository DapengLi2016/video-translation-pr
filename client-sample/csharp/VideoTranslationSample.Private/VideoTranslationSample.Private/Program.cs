//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.CommandParser;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation;
using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;
using Microsoft.SpeechServices.VideoTranslation.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SpeechServices.CustomVoice.TtsLib.Util;
using DeploymentEnvironment = Microsoft.SpeechServices.VideoTranslationLib.Enums.DeploymentEnvironment;
using Microsoft.SpeechServices.CustomVoice;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        ConsoleMaskSasHelper.ShowSas = true;
        return await ConsoleApp<Arguments<DeploymentEnvironment>>.RunAsync(
            args,
            ProcessAsync<DeploymentEnvironment, VideoFileMetadata>).ConfigureAwait(false);
    }

    public static async Task<int> ProcessAsync<TDeploymentEnvironment, TVideoFileMetadata>(
        Arguments<TDeploymentEnvironment> args)
        where TVideoFileMetadata : VideoFileMetadata
        where TDeploymentEnvironment : Enum
    {
        try
        {
            var httpConfig = new VideoTranslationPrivatePreviewHttpClientConfig<TDeploymentEnvironment>(
                args.Environment,
                args.SpeechSubscriptionKey)
            {
                ApiVersion = string.IsNullOrEmpty(args.ApiVersion) ?
                    VideoTranslationPrivateConstant.ApiVersions.PrivatePreviewApiVersion20230401Preview : args.ApiVersion,
            };

            var translationClient = new VideoTranslationClient<TDeploymentEnvironment>(httpConfig);

            var fileClient = new VideoFileClient<TDeploymentEnvironment>(httpConfig);

            var metadataClient = new VideoTranslationMetadataClient<TDeploymentEnvironment>(httpConfig);

            var targetLocaleClient = new TargetLocaleClient<TDeploymentEnvironment>(httpConfig);

            var consentClient = new VideoTranslationConsentClient<TDeploymentEnvironment>(httpConfig);

            var defaultConsentTemplatetClient = new DefaultConsentTemplateClient<TDeploymentEnvironment>(httpConfig);

            switch (args.Mode)
            {
                case Mode.QueryMetadata:
                    {
                        var metadata = await metadataClient.QueryMetadataAsync().ConfigureAwait(false);

                        Console.WriteLine(JsonConvert.SerializeObject(
                            metadata,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryTargetLocales:
                    {
                        var targetLocales = await targetLocaleClient.QueryTargetLocalesAsync().ConfigureAwait(false);

                        Console.WriteLine(JsonConvert.SerializeObject(
                            targetLocales,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryTargetLocale:
                    {
                        var targetLocale = await targetLocaleClient.QueryTargetLocaleAsync(
                            args.Id).ConfigureAwait(false);

                        Console.WriteLine(JsonConvert.SerializeObject(
                            targetLocale,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.UpdateTargetLocaleEdittingWebvttFile:
                    {
                        var editingWebvttFileMetadata = await targetLocaleClient.UpdateTargetLocaleEdittingWebvttFileAsync(
                            id: args.TypedTargetLocaleId.Value,
                            kind: !string.IsNullOrEmpty(args.SourceLocaleWebvttFilePath) ? VideoTranslationWebVttFilePlainTextKind.SourceLocalePlainText : null,
                            webvttFilePath: !string.IsNullOrEmpty(args.SourceLocaleWebvttFilePath) ?
                             args.SourceLocaleWebvttFilePath :
                             args.TargetLocaleWebvttFilePath).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            editingWebvttFileMetadata,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.DeleteTargetLocale:
                    {
                        await fileClient.DeleteTargetLocaleAsync(
                            args.VideoOrAudioFileId,
                            args.TypedTargetLocales.First(),
                            args.DeleteAssociations).ConfigureAwait(false);
                        break;
                    }

                case Mode.UploadVideoOrAudioFile:
                    {
                        if (!File.Exists(args.SourceVideoOrAudioFilePath))
                        {
                            throw new FileNotFoundException(args.SourceVideoOrAudioFilePath);
                        }

                        Console.WriteLine($"Uploading file: {args.SourceVideoOrAudioFilePath}");
                        var videoFile = await fileClient.UploadVideoFileAsync<TVideoFileMetadata>(
                            name: Path.GetFileName(args.SourceVideoOrAudioFilePath),
                            description: null,
                            locale: args.TypedSourceLocale,
                            speakerCount: null,
                            videoFilePath: args.SourceVideoOrAudioFilePath).ConfigureAwait(false);

                        Console.WriteLine(JsonConvert.SerializeObject(
                            videoFile,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.UploadVideoOrAudioFileIfNotExist:
                    {
                        if (!File.Exists(args.SourceVideoOrAudioFilePath))
                        {
                            throw new FileNotFoundException(args.SourceVideoOrAudioFilePath);
                        }

                        var fileContentSha256 = Sha256Helper.GetSha256WithExtensionFromFile(args.SourceVideoOrAudioFilePath);
                        var videoFile = await fileClient.QueryVideoFileWithLocaleAndFileContentSha256Async(
                            args.TypedSourceLocale,
                            fileContentSha256).ConfigureAwait(false);
                        if (videoFile == null)
                        {
                            Console.WriteLine($"Uploading file: {args.SourceVideoOrAudioFilePath}");
                            videoFile = await fileClient.UploadVideoFileAsync<TVideoFileMetadata>(
                                name: Path.GetFileName(args.SourceVideoOrAudioFilePath),
                                description: null,
                                locale: args.TypedSourceLocale,
                                speakerCount: null,
                                videoFilePath: args.SourceVideoOrAudioFilePath).ConfigureAwait(false);
                        }

                        Console.WriteLine(JsonConvert.SerializeObject(
                            videoFile,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.UploadVideoOrAudioFileAndCreateTranslation:
                    {
                        if (string.IsNullOrEmpty(args.SourceVideoOrAudioFilePath))
                        {
                            throw new ArgumentException($"Please provide at least one of {nameof(args.VideoOrAudioFileId)} or {nameof(args.SourceVideoOrAudioFilePath)}");
                        }

                        if (args.TypedSourceLocale == null || string.IsNullOrEmpty(args.TypedSourceLocale.Name))
                        {
                            throw new ArgumentNullException(nameof(args.TypedSourceLocale));
                        }

                        if (!File.Exists(args.SourceVideoOrAudioFilePath))
                        {
                            throw new FileNotFoundException(args.SourceVideoOrAudioFilePath);
                        }

                        VideoFileMetadata videoOrAudioFile = null;
                        if (args.ReuseExistingVideoOrAudioFile)
                        {
                            var fileContentSha256 = Sha256Helper.GetSha256WithExtensionFromFile(args.SourceVideoOrAudioFilePath);
                            videoOrAudioFile = await fileClient.QueryVideoFileWithLocaleAndFileContentSha256Async(
                                args.TypedSourceLocale,
                                fileContentSha256).ConfigureAwait(false);
                        }

                        if (videoOrAudioFile == null)
                        {
                            Console.WriteLine($"Uploading file: {args.SourceVideoOrAudioFilePath}");
                            videoOrAudioFile = await fileClient.UploadVideoFileAsync<TVideoFileMetadata>(
                                name: Path.GetFileName(args.SourceVideoOrAudioFilePath),
                                description: null,
                                locale: args.TypedSourceLocale,
                                speakerCount: null,
                                videoFilePath: args.SourceVideoOrAudioFilePath).ConfigureAwait(false);
                            Console.WriteLine($"Uploaded new video file with ID {videoOrAudioFile.ParseIdFromSelf()} uploaded.");
                        }
                        else
                        {
                            Console.WriteLine($"Reuse existing video file with ID {videoOrAudioFile.ParseIdFromSelf()}.");
                        }

                        Console.WriteLine(JsonConvert.SerializeObject(
                            videoOrAudioFile,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));

                        var displayName = $"{videoOrAudioFile.DisplayName} : {videoOrAudioFile.Locale.Name} => {string.Join(",", args.TypedTargetLocales.Select(x => x.Name))}";
                        var translation = await DoCreateTranslationAsync(
                            client: translationClient,
                            videoFileId: videoOrAudioFile.ParseIdFromSelf(),
                            baseTargetLocaleId: null,
                            displayName: displayName,
                            args: args).ConfigureAwait(false);
                        if (translation == null)
                        {
                            return ExitCode.GenericError;
                        }

                        break;
                    }

                case Mode.QueryVideoOrAudioFiles:
                    {
                        var videoFiles = await fileClient.QueryVideoFilesAsync().ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            videoFiles,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryVideoOrAudioFile:
                    {
                        var videoFile = await fileClient.QueryVideoFileAsync<TVideoFileMetadata>(args.Id).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            videoFile,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.DeleteVideoOrAudioFile:
                    {
                        var response = await fileClient.DeleteVideoFileAsync(args.Id, args.DeleteAssociations).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            ((HttpStatusCode)response.StatusCode).AsString()),
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings);
                        break;
                    }

                case Mode.CreateTranslation:
                    {
                        var displayName = string.Empty;
                        if (args.VideoOrAudioFileId != Guid.Empty)
                        {
                            var videoOrAudioFile = await fileClient.QueryVideoFileAsync<TVideoFileMetadata>(
                                args.VideoOrAudioFileId).ConfigureAwait(false);
                            if (videoOrAudioFile == null)
                            {
                                throw new InvalidDataException($"Failed to find video or audio file with ID {args.VideoOrAudioFileId}");
                            }

                            displayName = $"{videoOrAudioFile.DisplayName} : {videoOrAudioFile.Locale.Name} => {string.Join(",", args.TypedTargetLocales.Select(x => x.Name))}";
                        }
                        else
                        {
                            displayName = $"New language based on target locale: {args.BaseTargetLocaleId}";
                        }

                        var translation = await DoCreateTranslationAsync(
                            client: translationClient,
                            videoFileId: args.VideoOrAudioFileId,
                            baseTargetLocaleId: args.BaseTargetLocaleId,
                            displayName: displayName,
                            args: args).ConfigureAwait(false);
                        if (translation == null)
                        {
                            return ExitCode.GenericError;
                        }

                        break;
                    }

                case Mode.QueryTranslations:
                    {
                        var translations = await translationClient.GetTranslationsAsync().ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            translations,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryTranslation:
                    {
                        var translation =  await translationClient.QueryTaskByIdUntilTerminatedAsync<Translation>(
                            id: args.Id,
                            additionalHeaders: args.ResponseIntermediateZip ?
                                new Dictionary<string, string>()
                                {
                                    { VideoTranslationPrivateConstant.HttpHeaders.ResponseIntermediateZipFileUrl, bool.TrueString }
                                } : null,
                            printFirstQueryResult: true).ConfigureAwait(false);
                        if (translation == null)
                        {
                            Console.WriteLine($"Failed to find translation with ID: {args.Id}");
                        }

                        break;
                    }

                case Mode.DeleteTranslation:
                    {
                        var response = await translationClient.DeleteTranslationAsync(args.Id).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            ((HttpStatusCode)response.StatusCode).AsString()),
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings);
                        break;
                    }

                case Mode.CreateConsent:
                    {
                        var consentCreate = new VideoTranslationConsentCreate()
                        {
                            UserProvidedId = Path.GetFileNameWithoutExtension(args.ConsentAudioFilePath) + DateTime.Now.ToIdSuffix(),
                            DisplayName = Path.GetFileName(args.ConsentAudioFilePath),
                            Description = Path.GetFileName(args.ConsentAudioFilePath),
                            CompanyName = args.ConsentCompanyName,
                            VoiceTalentName = args.ConsentTalentName,
                            Locale = args.TypedLocale,
                        };

                        var consent = await consentClient.CreateConsentAsync(
                            consent: consentCreate,
                            audioFilePath: args.ConsentAudioFilePath).ConfigureAwait(false);

                        consent = await consentClient.QueryTaskByIdUntilTerminatedAsync<VideoTranslationConsent>(
                            id: args.Id,
                            printFirstQueryResult: true).ConfigureAwait(false);
                        if (consent == null)
                        {
                            Console.WriteLine($"Failed to find consent with ID: {args.Id}");
                        }

                        Console.WriteLine(JsonConvert.SerializeObject(
                            consent,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryConsents:
                    {
                        var consents = await consentClient.GetConsentsAsync().ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            consents,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.QueryConsent:
                    {
                        var consent = await consentClient.QueryTaskByIdUntilTerminatedAsync<VideoTranslationConsent>(
                            id: args.Id,
                            printFirstQueryResult: true).ConfigureAwait(false);
                        if (consent == null)
                        {
                            Console.WriteLine($"Failed to find consent with ID: {args.Id}");
                        }

                        Console.WriteLine(JsonConvert.SerializeObject(
                            consent,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.DeleteConsent:
                    {
                        var response = await consentClient.DeleteConsentAsync(args.Id).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(
                            ((HttpStatusCode)response.StatusCode).AsString()),
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings);
                        break;
                    }

                case Mode.QueryDefaultConsentTemplate:
                    {
                        var template = await defaultConsentTemplatetClient.GetDefaultConsentTemplateAsync(args.TypedLocale).ConfigureAwait(false);
                        if (template == null)
                        {
                            Console.WriteLine($"Failed to find template with locale: {args.TypedLocale}");
                        }

                        Console.WriteLine(JsonConvert.SerializeObject(
                            template,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                case Mode.UpdateCustomLexiconFileIdToTargetLocale:
                    {
                        ArgumentNullException.ThrowIfNull(args.TypedTargetLocaleId);

                        var targetLocaleUpdate = new VideoFileTargetLocaleUpdate()
                        {
                            ResetTtsCustomLexiconFileAccId = args.TypedTtsCustomLexiconFileIdInAudioContentCreation == null,
                            TtsCustomLexiconFileIdInAudioContentCreation = args.TypedTtsCustomLexiconFileIdInAudioContentCreation,
                        };

                        var response = await targetLocaleClient.UpdateTargetLocaleAsync(
                            targetLocaleId: args.TypedTargetLocaleId.Value,
                            targetLocaleUpdate: targetLocaleUpdate).ConfigureAwait(false);
                        Console.WriteLine(JsonConvert.SerializeObject(response,
                            Formatting.Indented,
                            CustomContractResolver.WriterSettings));
                        break;
                    }

                default:
                    throw new NotSupportedException(args.Mode.AsString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to run with exception: {e.Message}");
            return ExitCode.GenericError;
        }

        Console.WriteLine();
        Console.WriteLine("Process completed successfully.");

        return ExitCode.NoError;
    }

    private static async Task<Translation> DoCreateTranslationAsync<TDeploymentEnvironment>(
        VideoTranslationClient<TDeploymentEnvironment> client,
        Guid? videoFileId,
        Guid? baseTargetLocaleId,
        string displayName,
        Arguments<TDeploymentEnvironment> args)
        where TDeploymentEnvironment : Enum
    {
        var targetLocaleDefinition = new TranslationTargetLocaleCreate();
        var filePaths = new Dictionary<string, string>();
        var targetLocales = new Dictionary<CultureInfo, TranslationTargetLocaleCreate>();

        if (args.TypedTargetLocales.Count() == 1)
        {
            targetLocales[args.TypedTargetLocales.Single()] = targetLocaleDefinition;

            if (!string.IsNullOrEmpty(args.TargetLocaleWebvttFilePath))
            {
                if (!File.Exists(args.TargetLocaleWebvttFilePath))
                {
                    throw new FileNotFoundException(args.TargetLocaleWebvttFilePath);
                }

                targetLocaleDefinition.WebVttFileName = Path.GetFileName(args.TargetLocaleWebvttFilePath);
                filePaths[targetLocaleDefinition.WebVttFileName] = args.TargetLocaleWebvttFilePath;
            }
        }
        else
        {
            foreach (var targetLocale in args.TypedTargetLocales)
            {
                targetLocales[targetLocale] = null;
            }
        }

        var translationDefinition = new TranslationCreate()
        {
            BaseTargetLocaleId = baseTargetLocaleId,
            VideoFileId = videoFileId,
            VoiceKind = args.VoiceKind,
            AlignKind = null,
            DisplayName = displayName,
            TargetLocales = targetLocales,
            EnableFeatures = args.EnableFeatures,
            ProfileName = args.ProfileName,
            PersonalVoiceModelName = args.PersonalVoiceModelName,
            IsAssociatedWithTargetLocale = args.IsAssociatedWithTargetLocale,
            IsNewTargetLocaleCreation = args.isNewTargetLocaleCreation,
            WebvttSourceKind = args.TypedWebvttSourceKind,
            WithoutSubtitleInTranslatedVideoFile = args.WithoutSubtitleInTranslatedVideoFile,
            ExportPersonalVoicePromptAudioMetadata = args.ExportPersonalVoicePromptAudioMetadata,
            TargetLocaleSubtitleMaxCharCountPerSegment = args.SubtitleMaxCharCountPerSegment,
            SpeakerCount = args.SpeakerCount,
            EnableLipSync = args.EnableLipSync,
            TtsCustomLexiconFileUrl = args.TypedTtsCustomLexiconFileUrl,
            TtsCustomLexiconFileIdInAudioContentCreation = args.TypedTtsCustomLexiconFileIdInAudioContentCreation,
        };

        var translation = await client.CreateTranslationAsync(
            translation: translationDefinition,
            sourceLocaleWebVttFilePath: args.SourceLocaleWebvttFilePath,
            filePaths: filePaths,
            additionalProperties: args.TypedCreateTranslationAdditionalProperties,
            additionalHeaders: args.ResponseIntermediateZip ?
                new Dictionary<string, string>()
                {
                    { VideoTranslationPrivateConstant.HttpHeaders.KeepIntermediateZipFile, bool.TrueString }
                } : null).ConfigureAwait(false);
        if (translation == null)
        {
            return translation;
        }

        Console.WriteLine();
        Console.WriteLine($"New translation created with ID {translation.ParseIdFromSelf()}.");

        // Print resposne of creating instead of first query state to make sure resposne of creating correct.
        Console.WriteLine(JsonConvert.SerializeObject(
            translation,
            Formatting.Indented,
            CustomContractResolver.WriterSettings));
        var createdTranslation = await client.QueryTaskByIdUntilTerminatedAsync<Translation>(
            id: translation.ParseIdFromSelf(),
            additionalHeaders: args.ResponseIntermediateZip ?
                new Dictionary<string, string>()
                {
                    { VideoTranslationPrivateConstant.HttpHeaders.ResponseIntermediateZipFileUrl, bool.TrueString }
                } : null,
            printFirstQueryResult: false).ConfigureAwait(false);
        if (createdTranslation == null)
        {
            Console.WriteLine($"Failed to find translation with ID: {translation.ParseIdFromSelf()}");
            return null;
        }

        return createdTranslation;
    }
}