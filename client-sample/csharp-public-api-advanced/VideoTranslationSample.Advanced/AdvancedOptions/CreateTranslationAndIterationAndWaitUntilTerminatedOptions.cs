//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System;
using System.Globalization;

[Verb("createTranslationAndIterationAndWaitUntilTerminated", HelpText = "Create translation and create first iteration until terminated.")]
public partial class CreateTranslationAndIterationAndWaitUntilTerminatedOptions : CreateTranslationAndIterationAndWaitUntilTerminatedBaseOptions
{
    [Option("sourceLocale", Required = false, HelpText = "Specify source locale of the video.")]
    public CultureInfo SourceLocale { get; set; }

    [Option("videoFileAzureBlobUrl", Required = false, HelpText = "Specify video file Azure blob URL.")]
    public Uri VideoFileAzureBlobUrl { get; set; }

    [Option("audioFileAzureBlobUrl", Required = false, HelpText = "Specify audio file Azure blob URL.")]
    public Uri AudioFileAzureBlobUrl { get; set; }

    [Option("enableLipSync", Required = false, HelpText = "Specify whether enable lip sync.")]
    public bool? EnableLipSync { get; set; }

    [Option("ttsCustomLexiconFileUrl", Required = false, HelpText = "Specify whether TTS custom custom lexicon url.")]
    public Uri TtsCustomLexiconFileUrl { get; set; }

    // If use Guid?, not support by error: is defined with a bad format
    [Option("ttsCustomLexiconFileIdInAudioContentCreation", Required = false, HelpText = "Specify whether TTS custom custom lexicon file ID in Audio Content Creation.")]
    public Guid TtsCustomLexiconFileIdInAudioContentCreation { get; set; }

    [Option("keepHighFidelityBackgroundAudio", Required = false, HelpText = "Specify whether to keep high fidelity background audio.")]
    public bool KeepHighFidelityBackgroundAudio { get; set; }

    [Option("exportSegmentRawTtsAudioFiles", Required = false, HelpText = "Export segment raw TTS audio files.")]
    public bool ExportSegmentRawTtsAudioFiles { get; set; }

    [Option("enableVideoSpeedAdjustment", Required = false, HelpText = "Specify whether enable video speed adjustment.")]
    public bool EnableVideoSpeedAdjustment { get; set; }
}

