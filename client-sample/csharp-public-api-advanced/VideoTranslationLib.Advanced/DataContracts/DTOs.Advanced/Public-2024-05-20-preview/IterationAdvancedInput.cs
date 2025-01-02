// <copyright file="IterationAdvancedInput.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

public class IterationAdvancedInput
{
    public string ProfileName { get; set; }

    public bool? KeepIntermediateZipFile { get; set; }

    public bool? DisableCache { get; set; }
}
