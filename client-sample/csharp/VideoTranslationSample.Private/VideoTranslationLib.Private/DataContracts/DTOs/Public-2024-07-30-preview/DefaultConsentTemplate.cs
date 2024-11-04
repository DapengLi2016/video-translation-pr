// <copyright file="DefaultConsentTemplate.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240730Preview;

using System.Collections.Generic;
using System.Globalization;

public class DefaultConsentTemplate
{
    public CultureInfo Locale { get; set; }

    public string Template { get; set; }

    public IReadOnlyDictionary<string, DefaultConsentTemplateSlot> Slots { get; init; }
}
