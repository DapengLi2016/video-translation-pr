//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

// Whether or not enable lip sync is fixed in translation, to simplify API schema.
public partial class IterationInput
{
    // Below properties are not released to public doc.
    public Uri TtsCustomLexiconFileUrl { get; set; }

    public Guid? TtsCustomLexiconFileIdInAudioContentCreation { get; set; }
}