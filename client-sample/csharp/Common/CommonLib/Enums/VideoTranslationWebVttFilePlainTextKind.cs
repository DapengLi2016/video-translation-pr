﻿// <copyright file="VideoTranslationWebVttFilePlainTextKind.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice;

public enum VideoTranslationWebVttFilePlainTextKind
{
    None = 0,

    // User can upload webvtt with plain text format, the content of plain text is source locale.
    SourceLocalePlainText,

    // User can upload webvtt with plain text format, the content of plain text is target locale.
    TargetLocalePlainText,
}
