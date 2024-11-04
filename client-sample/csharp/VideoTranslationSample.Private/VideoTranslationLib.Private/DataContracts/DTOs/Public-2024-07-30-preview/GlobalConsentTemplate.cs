// <copyright file="GlobalConsentTemplate.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240730Preview;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class GlobalConsentTemplate
{
    // For default consent template, use locale name as ID.
    // In the future, when support customize consent template, need check ID not duplicate with this global ID.
    public string Id { get; set; }

    public CultureInfo Locale { get; set; }

    public string Template { get; set; }

    public IReadOnlyDictionary<string, GlobalConsentTemplateSlot> Slots { get; init; }
}
