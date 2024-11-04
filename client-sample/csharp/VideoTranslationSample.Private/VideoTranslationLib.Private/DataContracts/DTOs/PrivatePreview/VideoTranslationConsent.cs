//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation;

using System;
using System.Globalization;

public class VideoTranslationConsent : StatefulResourceBase
{
    public CultureInfo Locale { get; set; }

    // VoiceTalentName and CompanyName are required for LocaleDefaultTemplate kind.
    public string VoiceTalentName { get; set; }

    public string CompanyName { get; set; }

    public Uri AudioFileUrl { get; set; }

    public string FailureReason { get; set; }
}
