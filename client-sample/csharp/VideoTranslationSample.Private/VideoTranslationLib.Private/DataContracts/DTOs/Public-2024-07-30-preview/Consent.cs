// <copyright file="Consent.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240730Preview;

using System.ComponentModel.DataAnnotations;

public class Consent : StatefulResourceBase
{
    [Required]
    public ConsentInput Input { get; set; }

    public string FailureReason { get; set; }
}
