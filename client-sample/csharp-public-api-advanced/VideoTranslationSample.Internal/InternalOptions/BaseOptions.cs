﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;

public partial class BaseOptions
{
    [Option("accessBackendApi", Required = false, HelpText = "Specify whether access backend api")]
    public bool AccessBackendApi { get; set; }
}
