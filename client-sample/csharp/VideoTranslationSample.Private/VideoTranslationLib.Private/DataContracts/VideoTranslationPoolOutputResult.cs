﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation.DataContracts.Utility;

using Microsoft.SpeechServices.VideoTranslation.DataContracts.DTOs;

public class VideoTranslationPoolOutputResult
{
    public VideoTranslationPoolInputArgs InputArgs { get; set; }

    public string Error { get; set; }

    public VideoFileMetadata VideoFile { get; set; }

    public Translation Translation { get; set; }

    public bool Success
    {
        get
        {
            return string.IsNullOrWhiteSpace(this.Error) &&
                this.Translation?.Status == CommonLib.Enums.OneApiState.Succeeded;
        }
    }
}
