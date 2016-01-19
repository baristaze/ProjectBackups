using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Crosspl.ObjectModel;

namespace Crosspl.UnitTests
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod]
        public void TestUserToken()
        {
            UserToken token = new UserToken();
            token.UserId = 1; // existing test account.
            token.OAuthProvider = OAuthProvider.Facebook;
            token.OAuthUserId = long.MaxValue.ToString();
            token.OAuthAccessToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            token.ExpireTimeUTC = DateTime.UtcNow.AddDays(3);

            Database.InsertOrUpdate(token, Constants.ConnectionString);

            UserToken token1x = new UserToken();
            token1x.UserId = 1; // existing test account.
            token1x.OAuthProvider = OAuthProvider.Facebook;
            if (!Database.Select(token1x, Constants.ConnectionString))
            {
                throw new ApplicationException("insert/select failed");
            }

            if(token.OAuthUserId != token1x.OAuthUserId)
            {
                throw new ApplicationException("select failed");
            }

            if (token.OAuthAccessToken != token1x.OAuthAccessToken)
            {
                throw new ApplicationException("select failed");
            }

            token.ExpireTimeUTC = token.ExpireTimeUTC.AddDays(30);
            Database.InsertOrUpdate(token, Constants.ConnectionString);
            Database.Delete(token, Constants.ConnectionString);
        }
    }
}
