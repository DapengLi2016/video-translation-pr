//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System;

public partial class CreateIterationAndWaitUntilTerminatedOptions : BaseOptions
{
    [Option("ttsCustomLexiconFileUrl", Required = false, HelpText = "Specify whether TTS custom custom lexicon url.")]
    public Uri TtsCustomLexiconFileUrl { get; set; }

    [Option("ttsCustomLexiconFileIdInAudioContentCreation", Required = false, HelpText = "Specify whether TTS custom custom lexicon file ID in Audio Content Creation.")]
    public Guid TtsCustomLexiconFileIdInAudioContentCreation { get; set; }

    [Option("exportSegmentRawTtsAudioFiles", Required = false, HelpText = "Export segment raw TTS audio files.")]
    public bool ExportSegmentRawTtsAudioFiles { get; set; }

    [Option("enableVideoSpeedAdjustment", Required = false, HelpText = "Specify whether enable video speed adjustment.")]
    public bool EnableVideoSpeedAdjustment { get; set; }

    [Option("enableOcrCorrectionFromSubtitle", Required = false, HelpText = "Specify whether enable OCR correction from subtitle.")]
    public bool EnableOcrCorrectionFromSubtitle { get; set; }

    [Option("exportTargetLocaleSubtitleASSFile", Required = false, HelpText = "Specify whether export target locale subtitle file with ASS format.")]
    public bool ExportTargetLocaleSubtitleASSFile { get; set; }
}

