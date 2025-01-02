//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System;

[Verb("createTranslationAndIterationAndWaitUntilTerminated", HelpText = "Create translation and create first iteration until terminated.")]
public partial class CreateTranslationAndIterationAndWaitUntilTerminatedOptions : CreateTranslationAndIterationAndWaitUntilTerminatedBaseOptions
{
    [Option('v', "videoFileAzureBlobUrl", Required = false, HelpText = "Specify video file Azure blob URL.")]
    public Uri VideoFileAzureBlobUrl { get; set; }

    [Option('a', "audioFileAzureBlobUrl", Required = false, HelpText = "Specify audio file Azure blob URL.")]
    public Uri AudioFileAzureBlobUrl { get; set; }

    [Option('p', "enableLipSync", Required = false, HelpText = "Specify whether enable lip sync.")]
    public bool? EnableLipSync { get; set; }

    [Option('c', "ttsCustomLexiconFileUrl", Required = false, HelpText = "Specify whether TTS custom custom lexicon url.")]
    public Uri TtsCustomLexiconFileUrl { get; set; }

    // If use Guid?, not support by error: is defined with a bad format
    [Option('d', "ttsCustomLexiconFileIdInAudioContentCreation", Required = false, HelpText = "Specify whether TTS custom custom lexicon file ID in Audio Content Creation.")]
    public Guid TtsCustomLexiconFileIdInAudioContentCreation { get; set; }
}

