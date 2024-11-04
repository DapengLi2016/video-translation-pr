//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.VideoTranslation;

using Flurl;
using Flurl.Http;
using Microsoft.SpeechServices.CommonLib.Util;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.VideoTranslation.Public20240730Preview;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

public class DefaultConsentTemplateClient<TDeploymentEnvironment> :
    HttpClientBase<TDeploymentEnvironment>
    where TDeploymentEnvironment : Enum
{
    public DefaultConsentTemplateClient(HttpClientConfigBase<TDeploymentEnvironment> config)
        : base(config)
    {
    }

    public override string ControllerName => "defaultconsenttemplates";

    public async Task<GlobalConsentTemplate> GetDefaultConsentTemplateAsync(CultureInfo locale)
    {
        if (string.IsNullOrEmpty(locale?.Name))
        {
            throw new ArgumentNullException(nameof(locale));
        }

        var url = BuildRequestBase();

        url = url.AppendPathSegment(locale.Name);

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ReceiveJson<GlobalConsentTemplate>()
                    .ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }

                Console.Write($"Response failed with error: {await ex.GetResponseStringAsync().ConfigureAwait(false)}");
                throw;
            }
        }).ConfigureAwait(false);
    }
}
