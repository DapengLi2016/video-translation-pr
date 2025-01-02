//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System;

public partial class CreateIterationAndWaitUntilTerminatedOptions : BaseOptions
{
    [Option('p', "enableLipSync", Required = false, HelpText = "Specify whether enable lip sync.")]
    public bool? EnableLipSync { get; set; }

    [Option('c', "ttsCustomLexiconFileUrl", Required = false, HelpText = "Specify whether TTS custom custom lexicon url.")]
    public Uri TtsCustomLexiconFileUrl { get; set; }

    [Option('u', "ttsCustomLexiconFileIdInAudioContentCreation", Required = false, HelpText = "Specify whether TTS custom custom lexicon file ID in Audio Content Creation.")]
    public Guid? TtsCustomLexiconFileIdInAudioContentCreation { get; set; }
}

