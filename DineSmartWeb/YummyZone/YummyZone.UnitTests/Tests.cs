using System;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using YummyZone.ObjectModel;

namespace YummyZone.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Tests
    {
        public Tests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        
        /*
        [TestMethod]
        public void TestRelationMap()
        {
            Guid g1 = new Guid("00000000-0000-0000-0000-000000000001");
            Guid g2 = new Guid("11111111-1111-1111-1111-111111111111");
            Guid g3 = new Guid("22222222-2222-2222-2222-222222222222");

            MapVenueToMenu map = new MapVenueToMenu(g1, g2, g3, 5);
            Console.WriteLine(map.ReorderQuery());
            Console.WriteLine(map.ReorderForAllQuery());
            Console.WriteLine(map.SelectQuery());
            Console.WriteLine(map.SelectAllQuery(g1));
            Console.WriteLine(map.InsertOrUpdateQuery());            
        }

        [TestMethod]
        public void TestRaiseError1()
        {
            string query = @"RAISERROR ('Item cannot be deleted since it has children', 16, 1);";

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=YummyZone;Integrated Security=True"))
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = query;
                        int affectedRows = command.ExecuteNonQuery();                        
                        Console.WriteLine("Effected rows:" + affectedRows);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }           

        [TestMethod]
        public void TestRaiseError2()
        {
            string query = @"RAISERROR ('Item cannot be deleted since it has children', 16, 1); SELECT 100;";

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=YummyZone;Integrated Security=True"))
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = query;
                        object o = command.ExecuteScalar();
                        int affectedRows = Convert.ToInt32(o.ToString());
                        Console.WriteLine("Effected rows:" + affectedRows);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        [TestMethod]
        public void GenerateBase64EncodedString()
        {
            string plainText = "baristaze@gmail.com:DineSm@rtRocks!";
            string encodedText = AsciiBase64Codex.Encode(plainText);
            Console.WriteLine("'{0}' becomes '{1}'", plainText, encodedText);
        }

        */
        /*
        [TestMethod]
        public VenueList TestSelectByGeoLocationQuery()
        {
            VenueList venueList = new VenueList();

            try
            {
                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=DineSmartDB;Integrated Security=True"))
                {
                    conn.Open();
                    using (SqlCommand command = conn.CreateCommand())
                    {  
                        command.CommandText = Venue.SelectByGeoLocationQuery(47.643904, -122.105583, 1.0, 1.0, false);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Venue venue = new Venue();
                                venue.InitFromSqlReader(reader);
                                venueList.Add(venue);
                            }
                        }
                    }
                }

                Console.WriteLine(venueList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return venueList;
        }

        [TestMethod]
        public void TestVenueDBase()
        {
            try
            {
                Venue venue = TestSelectByGeoLocationQuery()[0];
                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=DineSmartDB;Integrated Security=True"))
                {
                    conn.Open();

                    SqlTransaction trans = conn.BeginTransaction();
                                        
                    Venue newVenue = new Venue();
                    newVenue.GroupId = venue.GroupId;
                    newVenue.ChainId = venue.ChainId;
                    newVenue.Latitude = (decimal)40.0;
                    newVenue.Longitude = (decimal)55.0;                    
                    newVenue.Name = "Osman";
                    newVenue.Status = VenueStatus.Draft;
                    newVenue.TimeZoneWinIndex = 12;
                    newVenue.WebURL = "bar";

                    try
                    {
                        Database.InsertOrUpdate(newVenue, conn, trans, 30);
                        Database.InsertOrUpdate(newVenue, conn, trans, 30);

                        newVenue.Name = "Osman 2";
                        newVenue.MapURL = "foo";
                        newVenue.WebURL = "bar";
                        newVenue.Status = VenueStatus.Active;
                        newVenue.LatitudeThresholdForRedeem = (decimal)0.4;
                        newVenue.LatitudeThresholdForSearch = (decimal)0.8;
                        newVenue.LongitudeThresholdForRedeem = (decimal)0.3;
                        newVenue.LongitudeThresholdForSearch = (decimal)1.2;
                        newVenue.RangeLimitInMilesForRedeem = (decimal)0.4;
                        newVenue.RangeLimitInMilesForSearch = (decimal)12.5;

                        Database.InsertOrUpdate(newVenue, conn, trans, 30);
                        Database.Select(newVenue, conn, trans, 30);
                        Database.SelectAll<Venue, VenueList>(conn, trans, venue.GroupId, 30);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        trans.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        [TestMethod]
        public void TestSelectByGeoLocationQuery()
        {
            double d = Venue.CalculateDistanceInMiles(
                        47.643904,
                        46.743904, 
                        -122.105583, 
                        -122.105583);

            Console.WriteLine(d);
        }
        

        [TestMethod]
        public void TestSettings()
        {
            try
           {
               using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=DineSmartDB;Integrated Security=True"))
               {
                   conn.Open();

                   int timeout = 5;

                   string settingName1 = "EnableFacebook";
                   Guid dinerId = new Guid("946ABAAD-C146-411C-849A-1BAB8C1904E7");
                   DinerSettingsItem item = new DinerSettingsItem(dinerId, settingName1, "1");
                   
                   Database.InsertOrUpdate(item, conn, null, timeout);
                   DinerSettingsItem itemx = new DinerSettingsItem(dinerId, settingName1, null);
                   Database.Select(itemx, conn, null, timeout);
                   if (item.Value != itemx.Value)
                   {
                       throw new ApplicationException("Settings test failed 1");
                   }

                   item.LastUpdateTimeUTC = DateTime.UtcNow;
                   Database.InsertOrUpdate(item, conn, null, timeout);
                   itemx = new DinerSettingsItem(dinerId, settingName1, null);
                   Database.Select(itemx, conn, null, timeout);
                   if (item.Value != itemx.Value)
                   {
                       throw new ApplicationException("Settings test failed 2");
                   }

                   item.Value = "0";
                   item.LastUpdateTimeUTC = DateTime.UtcNow;
                   Database.InsertOrUpdate(item, conn, null, timeout);
                   itemx = new DinerSettingsItem(dinerId, settingName1, null);
                   Database.Select(itemx, conn, null, timeout);
                   if (item.Value != itemx.Value)
                   {
                       throw new ApplicationException("Settings test failed 3");
                   }

                   if (Database.Delete(item, conn, null, timeout) != 1)
                   {
                       throw new ApplicationException("Settings test failed 4");
                   }

                   string settingName2 = "EnableTwitter";
                   DinerSettingsItem item2 = new DinerSettingsItem(dinerId, settingName2, "1");
                   Database.InsertOrUpdate(item, conn, null, timeout);
                   Database.InsertOrUpdate(item2, conn, null, timeout);
                   DinerSettingsItem itemx2 = new DinerSettingsItem(dinerId, settingName2, null);
                   Database.Select(itemx2, conn, null, timeout);
                   if (item2.Value != itemx2.Value)
                   {
                       throw new ApplicationException("Settings test failed 5");
                   }

                   DinerSettings settings = DinerSettings.ReadFromDBase(conn, null, dinerId);
                   if (settings.Count != 2)
                   {
                       throw new ApplicationException("Settings test failed 6");
                   }

                   settings = DinerSettings.Save(conn, null, item);
                   if (settings.Count != 2)
                   {
                       throw new ApplicationException("Settings test failed 7");
                   }

                   settings = DinerSettings.Save(conn, null, new DinerSettingsItem(dinerId, "Foo", "Bar"));
                   if (settings.Count != 3)
                   {
                       throw new ApplicationException("Settings test failed 8");
                   }

                   if (DinerSettings.Delete(conn, null, dinerId) <= 0)
                   {
                       throw new ApplicationException("Settings test failed 9");
                   }

                   settings = DinerSettings.ReadFromDBase(conn, null, dinerId);
                   if (settings.Count != 0)
                   {
                       throw new ApplicationException("Settings test failed 10");
                   }
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex);
           }
        }
        */
        /* 
       [TestMethod]
       public void TestSelectByGeoLocationQuery()
       {
           try
           {
               using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=DineSmartDB;Integrated Security=True"))
               {
                   conn.Open();
                   using (SqlCommand command = conn.CreateCommand())
                   {
                       command.CommandText = Venue.SelectByGeoLocationQuery(47.643904, -122.105583, 0.02, 0.02);
                       using (SqlDataReader reader = command.ExecuteReader())
                       {
                           while (reader.Read())
                           {
                               Venue venue = new Venue();
                               venue.InitFromSqlReader(reader);
                           }
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex);
           }
       }*/
    }
}
