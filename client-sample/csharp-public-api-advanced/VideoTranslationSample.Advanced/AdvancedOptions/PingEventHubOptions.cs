//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;
using Microsoft.SpeechServices.Common.Client.Enums.VideoTranslation;
using System;
using System.Collections.Generic;

[Verb("pingEventHub", HelpText = "Ping EventHub with a predefined simple event.")]
public class PingEventHubOptions : BaseOptions
{
}

