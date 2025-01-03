//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System;

// Due to it changed VideoFileAzureBlobUrl from required to optional, so can't reference the partial from VideoTranslationSample
[Verb("createTranslation", HelpText = "Create translation.")]
public partial class CreateTranslationOptions : CreateTranslationBaseOptions
{
    [Option('t', "translationId", Required = true, HelpText = "Specify translation ID.")]
    public string TranslationId { get; set; }

    [Option('v', "videoFileAzureBlobUrl", Required = false, HelpText = "Specify video file Azure blob URL.")]
    public Uri VideoFileAzureBlobUrl { get; set; }

    [Option('a', "audioFileAzureBlobUrl", Required = false, HelpText = "Specify audio file Azure blob URL.")]
    public Uri AudioFileAzureBlobUrl { get; set; }

    [Option('p', "enableLipSync", Required = false, HelpText = "Specify whether enable lip sync.")]
    public bool? EnableLipSync { get; set; }
}

