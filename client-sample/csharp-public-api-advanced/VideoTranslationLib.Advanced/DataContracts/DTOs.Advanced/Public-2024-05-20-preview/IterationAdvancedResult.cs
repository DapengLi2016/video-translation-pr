// <copyright file="IterationAdvancedResult.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

using System;

public class IterationAdvancedResult
{
    // If created with keep intermediate zip file, then always response intermediate zip.
    public Uri IntermediateZipFileUrl { get; set; }
}
