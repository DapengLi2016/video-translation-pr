// <copyright file="VideoTranslationFileUploadKind.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.Common.Client;

public enum VideoTranslationFileUploadKind
{
    None = 0,

    MultipartFormFile,

    AzureBlobUrl,
}
