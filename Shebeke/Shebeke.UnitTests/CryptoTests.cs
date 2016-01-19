using System;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Shebeke.ObjectModel;

namespace Shebeke.UnitTests
{
    [TestClass]
    public class CryptoTests
    {
        [TestMethod]
        public void TestAES()
        {
            string key = Crypto.GetRandomKeyFor128BitAES();
            System.Threading.Thread.Sleep(1000);
            string iv = Crypto.GetRandomKeyFor128BitAES();
            Console.WriteLine(key);
            Console.WriteLine(iv);

            UserAuthInfo user = new UserAuthInfo();
            user.FirstName = "Baris";
            user.LastName = "Taze";
            user.OAuthProvider = OAuthProvider.Facebook;
            user.OAuthUserId = "762118888";
            user.PhotoUrl = "https://someplace.somedomain.com/111222334556/picture?type=small";
            user.UserType = UserType.Regular;
            user.UserId = 1111111;
            user.OAuthAccessToken = "AAAEplmzPKZB8BAIdwoEC76KExNAmGqU8EAOWGmqFsa5gdekfnEAx8i8LqDAlm0VHz4JoOvFPnWqFWIArZBM0hQFzULaTChFFEiZAZBvoagZDZD";
            user.SplitId = 10;
            
            string sessionToken = user.ToString();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int LOOP = 1000;
            string cipher = null;
            for (int x = 0; x < LOOP; x++)
            {
                cipher = Crypto.EncryptAES(sessionToken, key, iv);
                string decrypt = Crypto.DecryptAES(cipher, key, iv);
                if (decrypt != sessionToken)
                {
                    throw new ApplicationException("AES failed");
                }
            }

            stopWatch.Stop();
            TimeSpan elapsedTime = stopWatch.Elapsed;
            Console.WriteLine("Elapsed Seconds: " + elapsedTime.TotalSeconds);
            Console.WriteLine("Average Seconds: " + elapsedTime.TotalSeconds / LOOP);
            Console.WriteLine("Last Cipher: " + cipher);
        }
    }
}
