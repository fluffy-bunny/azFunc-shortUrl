using System.Collections.Generic;


namespace ShorturlRequestUserTracker
{
    public class MiddlewareOptions
    {
        public AuthorizationHookOptions AuthorizationHookOptions { get; set; }
    }
    public class AuthorizationHookOptions
    {
        public AuthorizationHookOptions()
        {
            // Set default value.
        }
        public List<string> TrackedClaims { get; set; }
        public string SubOption1 { get; set; }
    }
}
