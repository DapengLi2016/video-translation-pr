//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;

using Microsoft.SpeechServices.Common.Client;
using Microsoft.SpeechServices.CommonLib.Enums;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using System;
using System.Collections.Generic;
using System.Globalization;

public class VideoFileMetadata : StatelessResourceBase
{
    public VideoTranslationFileKind FileKind { get; set; }

    public CultureInfo Locale { get; set; }

    public int? SpeakerCount { get; set; }

    public IEnumerable<CultureInfo> TargetLocales { get; set; }

    public Uri VideoFileUri { get; set; }

    public Uri AudioFileUri { get; set; }

    public TimeSpan? Duration { get; set; }

    // For backword compatiable, only response status when specify it explicitly.
    // When ApiVersion is null or < 2, Status and LastActionDateTime is null for compatiable.
    // When ApiVersion >= 2, response Status and LastActionDateTime
    public OneApiState? Status { get; set; }

    public DateTime? LastActionDateTime { get; set; }

    public VideoTranslationFileUploadKind? UploadKind { get; init; }
}
