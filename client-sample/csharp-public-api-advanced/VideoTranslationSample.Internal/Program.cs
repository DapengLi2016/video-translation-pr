﻿namespace Microsoft.SpeechServices.VideoTranslation.ApiSampleCode.PublicPreview;

using CommandLine;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.Attributes;
using Microsoft.SpeechServices.CommonLib.Enums;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;
using Microsoft.SpeechServices.VideoTranslationLib.Internal.HttpClient;
using Microsoft.SpeechServices.VideoTranslationSample.Advanced.HttpClient;
using Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VideoTranslationPublicPreviewLib.HttpClient;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        var types = LoadVerbs();

        var exitCode = await Parser.Default.ParseArguments(args, types)
            .MapResult(
                options => RunAndReturnExitCodeAsync(options),
                _ => Task.FromResult(1));

        if (exitCode == 0)
        {
            Console.WriteLine("Process completed successfully.");
        }
        else
        {
            Console.WriteLine($"Failure with exit code: {exitCode}");
        }

        return exitCode;
    }

    static async Task<int> RunAndReturnExitCodeAsync(object options)
    {
        var optionsBase = options as BaseOptions;
        ArgumentNullException.ThrowIfNull(optionsBase);
        try
        {
            return await DoRunAndReturnExitCodeAsync(optionsBase).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to run with exception: {e.Message}");
            return CommonPublicConst.ExistCodes.GenericError;
        }
    }

    //load all types using Reflection
    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }

    static async Task<int> DoRunAndReturnExitCodeAsync(BaseOptions baseOptions)
    {
        ArgumentNullException.ThrowIfNull(baseOptions);

        var env = DeploymentEnvironmentAttribute.ParseFromRegionIdentifier<DeploymentEnvironment>(baseOptions.Region);
        var regionConfig = new InternalRegionConfig(
            env,
            baseOptions.AccessBackendApi);

        var httpConfig = new InternalHttpClientConfig(
            regionConfig: regionConfig,
            subKey: baseOptions.SubscriptionKey)
        {
            ApiVersion = string.IsNullOrEmpty(baseOptions.ApiVersion) ?
                CommonPublicConst.ApiVersions.ApiVersion20240520Preview : baseOptions.ApiVersion,
        };

        var translationClient = new TranslationClient(httpConfig);

        var iterationClient = new IterationClient(httpConfig);

        var operationClient = new OperationClient(httpConfig);

        var segmentClient = new SegmentClient(httpConfig);

        var configClient = new ConfigurationClient(httpConfig);

        switch (baseOptions)
        {
            case CreateOrUpdateEventHubConfigOptions options:
                {
                    var config = await configClient.CreateOrUpdateEventHubConfigAsync(
                        new EventHubConfig()
                        {
                            IsEnabled = options.IsEnabled,
                            EventHubNamespaceHostName = options.EventHubNamespaceHostName,
                            EventHubName = options.EventHubName,
                            ManagedIdentityClientId = (options.ManagedIdentityClientId == Guid.Empty) ?
                                null : options.ManagedIdentityClientId,
                            EnabledEvents = options.EnabledEvents,
                        }).ConfigureAwait(false);
                    Console.WriteLine("EventHub configuration created or updated:");
                    Console.WriteLine(JsonConvert.SerializeObject(
                        config,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case QueryEventHubConfigOptions options:
                {
                    var config = await configClient.GetEventHubConfigAsync().ConfigureAwait(false);
                    Console.WriteLine("EventHub configuration:");
                    Console.WriteLine(JsonConvert.SerializeObject(
                        config,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case PingEventHubOptions options:
                {
                    await configClient.PingEventHubAsync().ConfigureAwait(false);
                    Console.WriteLine("Requested send ping event.");
                    break;
                }

            case CreateTranslationOptions options:
                {
                    if (string.IsNullOrWhiteSpace(options.VideoFileAzureBlobUrl?.OriginalString) &&
                        string.IsNullOrWhiteSpace(options.AudioFileAzureBlobUrl?.OriginalString))
                    {
                        throw new ArgumentException($"Please provide either {nameof(options.VideoFileAzureBlobUrl)} or {nameof(options.AudioFileAzureBlobUrl)}");
                    }

                    var translation = new Translation()
                    {
                        Id = options.TranslationId,
                        DisplayName = options.TranslationName,
                        Description = options.TranslationDescription,
                        Input = new TranslationInput()
                        {
                            EnableLipSync = options.EnableLipSync,
                            SourceLocale = options.SourceLocale,
                            TargetLocale = options.TargetLocale,
                            SpeakerCount = options.SpeakerCount,
                            VoiceKind = options.VoiceKind,
                            VideoFileUrl = options.VideoFileAzureBlobUrl,
                            AudioFileUrl = options.AudioFileAzureBlobUrl,
                        },
                    };

                    translation = await translationClient.CreateTranslationAndWaitUntilTerminatedAsync(
                        translation: translation).ConfigureAwait(false);

                    Console.WriteLine();
                    Console.WriteLine("Created translation:");
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translation,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case QueryTranslationsOptions options:
                {
                    var translations = await translationClient.GetTranslationsAsync().ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translations,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case QuerySegmentsOptions options:
                {
                    var segments = await segmentClient.GetSegmentsAsync(
                        options.TranslationId,
                        options.IterationId).ConfigureAwait(false);
                    var response = JsonConvert.SerializeObject(
                        segments,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings);

                    // It may show not correct chars for Chinese chars.
                    Console.WriteLine(response);
                    // await File.WriteAllTextAsync(@"E:\temp\a.txt", response).ConfigureAwait(false);
                    break;
                }

            case QueryTranslationOptions options:
                {
                    var translation = await translationClient.GetTranslationAsync(
                        options.TranslationId).ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translation,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case DeleteTranslationOptions options:
                {
                    var response = await translationClient.DeleteTranslationAsync(
                        options.TranslationId).ConfigureAwait(false);
                    Console.WriteLine(response.StatusCode);
                    break;
                }

            case CreateIterationAndWaitUntilTerminatedOptions options:
                {
                    var iteration = new Iteration()
                    {
                        Id = options.IterationId,
                        DisplayName = string.IsNullOrEmpty(options.IterationName) ?
                            options.IterationId : options.IterationName,
                        Input = new IterationInput()
                        {
                            SpeakerCount = options.SpeakerCount,
                            SubtitleMaxCharCountPerSegment = options.SubtitleMaxCharCountPerSegment,
                            ExportSubtitleInVideo = options.ExportSubtitleInVideo,
                            WebvttFile = options.WebvttFile,
                            ExportSegmentRawTtsAudioFiles = options.ExportSegmentRawTtsAudioFiles,
                            EnableVideoSpeedAdjustment = options.EnableVideoSpeedAdjustment,
                            TtsCustomLexiconFileIdInAudioContentCreation = options.TtsCustomLexiconFileIdInAudioContentCreation,
                            TtsCustomLexiconFileUrl = options.TtsCustomLexiconFileUrl,
                            EnableOcrCorrectionFromSubtitle = options.EnableOcrCorrectionFromSubtitle,
                            ExportTargetLocaleSubtitleASSFile = options.ExportTargetLocaleSubtitleASSFile,
                        }
                    };

                    var iterationResponse = await iterationClient.CreateIterationAndWaitUntilTerminatedAsync(
                        translationId: options.TranslationId,
                        iteration: iteration,
                        additionalHeaders: null).ConfigureAwait(false);
                    break;
                }

            case QueryIterationsOptions options:
                {
                    var translations = await translationClient.GetIterationsAsync(
                        options.TranslationId).ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translations,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case QueryIterationOptions options:
                {
                    var translations = await translationClient.GetIterationAsync(
                        translationId: options.TranslationId,
                        iterationId: options.IterationId).ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translations,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case CreateTranslationAndIterationAndWaitUntilTerminatedOptions options:
                {
                    var iteration = new Iteration()
                    {
                        Id = options.IterationId,
                        DisplayName = options.IterationId,
                        Input = new IterationInput()
                        {
                            SpeakerCount = options.SpeakerCount,
                            SubtitleMaxCharCountPerSegment = options.SubtitleMaxCharCountPerSegment,
                            ExportSubtitleInVideo = options.ExportSubtitleInVideo,
                            TtsCustomLexiconFileUrl = options.TtsCustomLexiconFileUrl,
                            TtsCustomLexiconFileIdInAudioContentCreation = options.TtsCustomLexiconFileIdInAudioContentCreation == Guid.Empty ?
                                null : options.TtsCustomLexiconFileIdInAudioContentCreation,
                            ExportSegmentRawTtsAudioFiles = options.ExportSegmentRawTtsAudioFiles,
                            EnableVideoSpeedAdjustment = options.EnableVideoSpeedAdjustment,
                            ExportTargetLocaleSubtitleASSFile = options.ExportTargetLocaleSubtitleASSFile,
                            WebvttFile = options.WebvttFileAzureBlobUrl == null ? null : new WebvttFile()
                            {
                                Kind = options.WebvttFileKind == WebvttFileKind.None ?
                                    throw new ArgumentException($"Please specify {nameof(options.WebvttFileKind)}") :
                                    options.WebvttFileKind,
                                Url = options.WebvttFileAzureBlobUrl,
                            }
                        }
                    };

                    if (string.IsNullOrWhiteSpace(options.VideoFileAzureBlobUrl?.OriginalString) &&
                        string.IsNullOrWhiteSpace(options.AudioFileAzureBlobUrl?.OriginalString))
                    {
                        throw new ArgumentException($"Please provide either {nameof(options.VideoFileAzureBlobUrl)} or {nameof(options.AudioFileAzureBlobUrl)}");
                    }

                    var translation = new Translation()
                    {
                        Id = options.TranslationId,
                        DisplayName = options.TranslationId,
                        Description = options.TranslationId,
                        Input = new TranslationInput()
                        {
                            EnableLipSync = options.EnableLipSync,
                            EnableProsodyTransfer = options.EnableProsodyTransfer,
                            SpeakerCount = options.SpeakerCount,
                            SubtitleMaxCharCountPerSegment = options.SubtitleMaxCharCountPerSegment,
                            ExportSubtitleInVideo = options.ExportSubtitleInVideo,
                            SourceLocale = options.SourceLocale,
                            TargetLocale = options.TargetLocale,
                            VoiceKind = options.VoiceKind,
                            VideoFileUrl = options.VideoFileAzureBlobUrl,
                            AudioFileUrl = options.AudioFileAzureBlobUrl,
                        }
                    };

                    (translation, iteration) = await translationClient.CreateTranslationAndIterationAndWaitUntilTerminatedAsync(
                        translation: translation,
                        iteration: iteration).ConfigureAwait(false);

                    break;
                }

            default:
                throw new NotSupportedException();
        }

        return CommonPublicConst.ExistCodes.NoError;
    }
}
