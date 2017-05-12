// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Authentication.Cookies
{
    /// <summary>
    /// Used to setup defaults for all <see cref="CookieAuthenticationOptions"/>.
    /// </summary>
    public class CookieAuthenticationInitializer : IInitializeOptions<CookieAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public CookieAuthenticationInitializer(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        /// <summary>
        /// Invoked to initialize a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being initialized.</param>
        /// <param name="options">The options instance to initialize.</param>
        public void Initialize(string name, CookieAuthenticationOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (String.IsNullOrEmpty(options.CookieName))
            {
                options.CookieName = CookieAuthenticationDefaults.CookiePrefix + name;
            }
            if (options.TicketDataFormat == null)
            {
                if (options.DataProtectionProvider == null)
                {
                    // This shouldn't happen normally due to the EnsureDataProtection initialize options.
                    throw new InvalidOperationException("DataProtectionProvider must be provided.");
                }

                // Note: the purpose for the data protector must remain fixed for interop to work.
                var dataProtector = options.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", name, "v2");
                options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (options.CookieManager == null)
            {
                options.CookieManager = new ChunkingCookieManager();
            }
            if (!options.LoginPath.HasValue)
            {
                options.LoginPath = CookieAuthenticationDefaults.LoginPath;
            }
            if (!options.LogoutPath.HasValue)
            {
                options.LogoutPath = CookieAuthenticationDefaults.LogoutPath;
            }
            if (!options.AccessDeniedPath.HasValue)
            {
                options.AccessDeniedPath = CookieAuthenticationDefaults.AccessDeniedPath;
            }
        }

    }
}
