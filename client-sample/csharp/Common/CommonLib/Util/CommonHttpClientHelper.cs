// <copyright file="HttpClientHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.SpeechServices.CustomVoice
{
    using Flurl.Http;
    using Microsoft.SpeechServices.CustomVoice.TtsLib.TtsUtil;
    using Polly;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public static class CommonHttpClientHelper
    {
        public static async Task<(bool success, string error)> IsUriExistAsync(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));


            var (success, error, resposne) = await ExceptionHelper.HasRunWithoutExceptionAsync(async () =>
            {
                return await url.HeadAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            return (success && (resposne.StatusCode == (int)HttpStatusCode.OK), error);
        }
    }
}
