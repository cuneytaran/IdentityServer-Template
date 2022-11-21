// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace IdentityServerHost.Quickstart.UI
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = false;//logout olduğunda geldiği adrese yönlemesi için false yaptık.
        public static bool AutomaticRedirectAfterSignOut = true;//otomatik geldiği adrese yönlenmesin. false identity sayfasında geldiğiniz yere gitmek isterseniz tıklayın gibi mesaj veriyor.

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
