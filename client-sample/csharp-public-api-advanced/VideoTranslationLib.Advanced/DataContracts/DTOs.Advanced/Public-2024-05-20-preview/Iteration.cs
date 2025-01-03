// <copyright file="Iteration.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

public partial class Iteration : StatefulResourceBase
{
    // AdvancedInput and AdvancedResult is not decleared in release doc.
    public IterationAdvancedInput AdvancedInput { get; set; }

    // For general purpose, AdvancedResult will be null to align with release doc.
    // Should only response AdvancedResult when specify response it explicitly in AdvancedInput.
    public IterationAdvancedResult AdvancedResult { get; set; }
}
