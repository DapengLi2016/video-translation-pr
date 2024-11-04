// <copyright file="ConsentInput.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240730Preview;

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.SpeechServices.CustomVoice.TtsLib.Util;

public class ConsentInput
{
    [Required]
    public CultureInfo Locale { get; set; }

    // VoiceTalentName and CompanyName are required for LocaleDefaultTemplate kind.
    public string VoiceTalentName { get; set; }

    public string CompanyName { get; set; }

    public Uri AudioFileUrl { get; set; }
}
