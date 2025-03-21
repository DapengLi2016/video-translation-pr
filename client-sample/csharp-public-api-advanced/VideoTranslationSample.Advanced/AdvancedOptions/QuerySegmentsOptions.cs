//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslationSample.PublicPreview;

using CommandLine;

[Verb("querySegments", HelpText = "Query segments of iteration by translation ID and iteration ID.")]
public partial class QuerySegmentsOptions : BaseOptions
{
    [Option("translationId", Required = true, HelpText = "Specify translation ID.")]
    public string TranslationId { get; set; }

    [Option("iterationId", Required = true, HelpText = "Specify iteration ID.")]
    public string IterationId { get; set; }
}

