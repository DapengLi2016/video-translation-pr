// <copyright file="VideoFileTargetLocaleUpdate.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation;

using System;

public class VideoFileTargetLocaleUpdate
{
    public Guid? TtsCustomLexiconFileIdInAudioContentCreation { get; set; }

    public bool ResetTtsCustomLexiconFileAccId { get; set; }
}
