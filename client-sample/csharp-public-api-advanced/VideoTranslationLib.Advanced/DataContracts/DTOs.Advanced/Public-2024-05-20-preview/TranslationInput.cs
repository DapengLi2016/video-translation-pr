//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240520Preview;

using System;
using System.Globalization;

public partial class TranslationInput
{
    // Below are advanced input
    public Uri AudioFileUrl { get; set; }

    public bool? EnableLipSync { get; set; }
}
