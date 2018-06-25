using System;

namespace IdentityServer4.OperationalStore.CoreTests
{
    static class Unique
    {
        public static string S => Guid.NewGuid().ToString("N");
        public static string Url => $"https://{S}.domain.com";
    }
}