// <copyright file="Segment.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

using System;

public class Segment
{
    public Guid Id { get; set; }

    public TimeSpan Offset { get; set; }

    public TimeSpan Duration { get; set; }

    public string SourceLocaleText { get; set; }

    public string TranslatedText { get; set; }

    public Uri TranslatedRawTtsAudioFileUrl { get; set; }
}
