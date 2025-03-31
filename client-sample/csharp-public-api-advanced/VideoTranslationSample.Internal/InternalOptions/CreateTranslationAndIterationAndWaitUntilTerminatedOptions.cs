﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using System.Globalization;

public partial class CreateTranslationAndIterationAndWaitUntilTerminatedOptions : CreateTranslationAndIterationAndWaitUntilTerminatedBaseOptions
{
    [Option("enableProsodyTransfer", Required = false, HelpText = "Specify whether enable prosody transfer.")]
    public bool? EnableProsodyTransfer { get; set; }

    [Option("keepIntermediateZipFile", Required = false, HelpText = "Specify whether keep intermediate zip file.")]
    public bool? KeepIntermediateZipFile { get; set; }
}

