//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.CommonLib.TtsUtil;

using Microsoft.SpeechServices.Common;
using System;
using System.IO;

public static class CommonPathHelper
{
    // Avoid using Path.GetTempFileName due to it will create empty file without file name extension which may take up batch node disk.
    public static string GetTempFilePath(string fileExtension = null)
    {
        var fileName = Guid.NewGuid().ToString();
        if (!string.IsNullOrEmpty(fileExtension))
        {
            fileName = fileName.AppendExtensionName(fileExtension);
        }

        return Path.Combine(Path.GetTempPath(), fileName);
    }
}
