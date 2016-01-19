using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public enum OAuthProvider
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Facebook,
    }

    public class OAuthProviderHelper
    {
        public static OAuthProvider GetOAuthProviderFromString(string oauthProvider)
        {
            if (!String.IsNullOrWhiteSpace(oauthProvider))
            {
                oauthProvider = oauthProvider.Trim();

                if (0 == String.Compare(oauthProvider, "Facebook", StringComparison.InvariantCultureIgnoreCase))
                {
                    return OAuthProvider.Facebook;
                }
            }

            return OAuthProvider.None;
        }
    }
}
