using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class FakeUserCreations
    {
        private class UserDifferentiator
        {
            public int TimeZoneIndex { get; set; }

            public String StateCode { get; set; }

            public String StateName { get; set; }

            public int SubRegionId { get; set; }

            public long BaseFacebookId { get; set; }

            public UserDifferentiator(int timeZoneIndex, String stateCode, String stateName, int subRegionId, long baseFacebookId)
            {
                this.TimeZoneIndex = timeZoneIndex;
                this.StateCode = stateCode;
                this.StateName = stateName;
                this.SubRegionId = subRegionId;
                this.BaseFacebookId = baseFacebookId;
            }
        }

        private const string PATH_IMAGES_WOMEN_DRAFT = @"D:\_CloudDrive\P4P\_StockImages\women\_selected";
        private const string PATH_IMAGES_WOMEN_FINAL = @"D:\_CloudDrive\P4P\_StockImages\women\_selected\_android_16x9";

        private const string PATH_IMAGES_MEN_DRAFT = @"D:\_VBoxShare\images";
        private const string PATH_IMAGES_MEN_FINAL = @"D:\_VBoxShare\images\_cropped";

        private const string PATH_NICKNAMES_MEN = @"D:\_CloudDrive\P4P\_Docs\nicknames\nicknames_men.txt";
        private const string PATH_NICKNAMES_WOMEN = @"D:\_CloudDrive\P4P\_Docs\nicknames\nicknames_women.txt";
        
        [TestMethod]
        public void TestFakeUsers()
        {
            // SelectAndTrimImages(PATH_IMAGES_WOMEN_DRAFT, PATH_IMAGES_WOMEN_FINAL);
            // SelectAndTrimImages(PATH_IMAGES_MEN_DRAFT, PATH_IMAGES_MEN_FINAL); 

            // location
            UserDifferentiator[] locs = new UserDifferentiator[] 
            { 
                //new UserDifferentiator(-8, "wa", "Washington", 1, -100101000),
                //new UserDifferentiator(-8, "wa", "Washington", 2, -100201000),
                //new UserDifferentiator(-8, "wa", "Washington", 3, -100301000),
                //new UserDifferentiator(-8, "wa", "Washington", 4, -100401000),
                //new UserDifferentiator(-8, "wa", "Washington", 5, -100501000),
                //new UserDifferentiator(-8, "wa", "Washington", 6, -100601000),

                //new UserDifferentiator(-8, "or", "Oregon", 0, -102001000),

                //new UserDifferentiator(-8, "ca", "California", 5, -103501000),
                //new UserDifferentiator(-8, "ca", "California", 6, -103601000),
                //new UserDifferentiator(-8, "ca", "California", 7, -103701000),
                //new UserDifferentiator(-8, "ca", "California", 8, -103801000),
                //new UserDifferentiator(-8, "ca", "California", 9, -103901000),
                
                //new UserDifferentiator(-8, "nv", "Nevada", 0, -105001000),
                //new UserDifferentiator(-7, "az", "Arizona", 0, -106001000),
                //new UserDifferentiator(-7, "co", "Colorado", 0, -107001000),

                //new UserDifferentiator(-6, "tx", "Texas", 1, -108101000),
                //new UserDifferentiator(-6, "tx", "Texas", 2, -108201000),
                //new UserDifferentiator(-7, "tx", "Texas", 3, -108301000),

                /*
                new UserDifferentiator(-6, "il", "Illinois", 0, -109001000),
                new UserDifferentiator(-6, "mn", "Minnesota", 0, -110001000),                
                new UserDifferentiator(-5, "ga", "Georgia", 0, -111001000),
                new UserDifferentiator(-5, "ma", "Massachusetts", 0, -112001000),
                new UserDifferentiator(-5, "mi", "Michigan", 0, -113001000),
                new UserDifferentiator(-5, "fl", "Florida", 0, -114001000), 
                new UserDifferentiator(-5, "ny", "New York", 0, -115001000),
                new UserDifferentiator(-5, "pa", "Pennsylvania", 0, -116001000), 
                new UserDifferentiator(-5, "nc", "North Carolina", 0, -117001000),
                new UserDifferentiator(-5, "dc", "District of Columbia", 0, -118001000),
                */
            };

            foreach (UserDifferentiator loc in locs)
            {
                CreateFakeUsers(
                    loc.StateCode, 
                    loc.StateName,
                    loc.SubRegionId,
                    Gender.Female, 
                    loc.BaseFacebookId, 
                    loc.TimeZoneIndex, 
                    PATH_IMAGES_WOMEN_FINAL, 
                    PATH_NICKNAMES_WOMEN);
                
                // DownloadImagesForFakeUsers(PATH_IMAGES_WOMEN_FINAL + @"\_cloud", loc.BaseFacebookId);
                
                CreateFakeUsers(
                    loc.StateCode, 
                    loc.StateName, 
                    loc.SubRegionId, 
                    Gender.Male, 
                    (loc.BaseFacebookId - 1000), 
                    loc.TimeZoneIndex, 
                    PATH_IMAGES_MEN_FINAL, 
                    PATH_NICKNAMES_MEN);
                
                // DownloadImagesForFakeUsers(PATH_IMAGES_MEN_FINAL + @"\_cloud", (loc.BaseFacebookId - 1000));                
            }
        }

        [TestMethod]
        public void DeleteAnImageFromBlob()
        {
            using (SqlConnection connection = new SqlConnection(this.GetDBConnectionString()))
            {
                connection.Open();

                Guid userId = new Guid("44E4B4B8-F46F-488B-9791-9A7BEE86FE24");
                List<ImageFile> images = ImageFile.ReadAllFromDBaseByUserId(connection, null, userId);

                foreach (ImageFile image in images)
                {
                    ImageFile.SafeDeleteFromCloud(this.GetBlobStorage(), image.CloudUrl);
                }
                /*
                String[] urls = new String[] {
                    "http://ginger.blob.core.windows.net/photos/img_e804508c8af747c2b91542c20011aa99.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_97e8d07bb1d94b708d1f5f8bf5aa4dbf.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_824c4b0a87fb4fc7b4396705c9c0ff44.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_a5a03b3e62ac417d87b16fb1900687c6.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_0d424d2d52534d2bb4167658efce86ca.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_50158088159343509dd77e2af2ab3d39.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_eb3da66831624418bf7d97ffe9566ec4.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_756ce090c50c45d4a7959f0062a0d001.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_f10f2538b1634f9189a8ca241e872917.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_abe75dadb257488f93d4d2e49c95c192.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_f213cd95225046c890b0e7c063a35d39.jpeg",
                    "http://ginger.blob.core.windows.net/photos/img_afa4d5444cfa43a990cffb46584eb0e3.jpeg",
                };

                foreach (String url in urls)
                {
                    ImageFile.SafeDeleteFromCloud(this.GetBlobStorage(), url);
                } 
                */
            }
        }

        [TestMethod]
        public void SendInitialMessage() 
        {
            using (SqlConnection connection = new SqlConnection(this.GetDBConnectionString()))
            {
                connection.Open();
                InstantMessage im = new InstantMessage();
                im.Id = Guid.NewGuid();
                im.SentTimeUTC = DateTime.UtcNow;
                im.UserId1 = User.WellKnownSystemUserId;
                im.UserId2 = new Guid("75816F7A-5D1C-4942-B2D6-72BCAD2E8B2C");
                im.Content = "Thank you for downloading pic4pic!\n\nShould you have any feedback or question, please send an email to appsicle@gmail.com\n\nEnjoy!\n";

                InstantMessage.CreateOnDBase(connection, null, im);
            }
        }

        [TestMethod]
        public void CreatePic4PicTeamUser()
        {
            DateTime utcNow = DateTime.UtcNow;

            // create user in memory
            User user = new User();
            user.CreateTimeUTC = utcNow;
            user.LastUpdateTimeUTC = utcNow;
            //user.Id = new Guid("11111111-1111-1111-1111-111111111111"); // 6C3CF22D-4726-466A-AAA5-F2DDB2D0E0B6
            user.Id = Guid.NewGuid();
            user.Username = "pic4pic team";
            user.Password = "G!ng@rSecure!X1234";
            user.SplitId = 0;
            user.UserType = UserType.SystemAdmin;
            user.UserStatus = UserStatus.Active;

            // create FB user in memory
            FacebookUser fbUser = new FacebookUser();

            fbUser.UserId = user.Id;
            fbUser.FacebookId = -9000000000;
            fbUser.Username = user.Username;

            fbUser.CreateTimeUTC = utcNow;
            fbUser.LastUpdateTimeUTC = utcNow;

            fbUser.CurrentLocationId = 0;
            fbUser.CurrentLocationCity = "Seattle";
            fbUser.CurrentLocationState = "Washington";
            fbUser.HometownId = fbUser.CurrentLocationId;
            fbUser.HometownCity = fbUser.CurrentLocationCity;
            fbUser.HometownState = fbUser.CurrentLocationState;
            fbUser.TimeZoneOffset = -7;

            fbUser.FirstName = "pic4pic";
            fbUser.LastName = "team";
            fbUser.FullName = "pic4pic team";
            fbUser.Gender = Gender.Unknown;

            fbUser.BirthDay = new DateTime(1990, 1, 1, 0, 0, 0);
            fbUser.EmailAddress = "appsicle@gmail.com";
            fbUser.FacebookUserName = "appsicle_pic4pic_team";
            fbUser.FacebookLink = "http://facebook.com/appsicle_pic4pic_team";
            fbUser.ISOLocale = "en_US";
            fbUser.IsVerified = false;
            fbUser.EducationLevel = EducationLevel.Master;
            fbUser.MaritalStatus = MaritalStatus.Single;
            fbUser.MaritalStatusAsText = "Single";
            fbUser.Profession = "Software Developer";

            // create user preferences
            UserPreferences pref = new UserPreferences();
            pref.UserId = user.Id;
            pref.LastUpdateTimeUTC = utcNow;
            pref.InterestedIn = Gender.Unknown;

            // upload images
            String imageFile = @"D:\_CloudDrive\P4P\Pic4Pic\team2.jpg";
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(imageFile);
            ImageFile[] imageMetaFiles = this.UploadImages(bmp);

            // save image meta files
            this.SaveMetaFilesOrFixCloud(imageMetaFiles);

            // prepare image IDs
            List<Guid> imageIDs = new List<Guid>();
            foreach (ImageFile imgFile in imageMetaFiles)
            {
                imageIDs.Add(imgFile.Id);
            }

            // create database entries
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Exception e = null;
                    try
                    {
                        this.CreateAllEntities(conn, trans, user, fbUser, null, null, pref, imageIDs);
                    }
                    catch (Exception ex)
                    {
                        e = ex;
                    }
                    finally
                    {
                        if (e == null)
                        {
                            trans.Commit();                                
                        }
                        else
                        {
                            trans.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }

        private void SelectAndTrimImages(string path, string targetPath)
        {
            int count = 0;
            int total = 0;
            StringBuilder notes = new StringBuilder();
            List<String> allowedExtensions = new List<String>(new String[] { "gif", "jpg", "jpeg", "tif", "bmp", "png" });
            foreach (string file in System.IO.Directory.EnumerateFiles(path))
            {
                FileInfo info = new FileInfo(path);
                if (info.Extension == null || !allowedExtensions.Contains(info.Extension.Trim('.').ToLower()))

                GC.Collect();
                System.Drawing.Bitmap bmp = null;
                try
                {
                    bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(file);
                }
                catch (OutOfMemoryException) 
                {
                    Console.WriteLine("Memory Exception with: " + file);
                }

                if (bmp == null) 
                {
                    continue;
                }

                total++;
                if (bmp.Height > 640 && bmp.Width > 360)
                {
                    int targetWidth = 0;
                    int targetHeight = 0;

                    float currentRatio = ((float)bmp.Height) / ((float)bmp.Width);
                    if (currentRatio >= (16.0 / 9.0))
                    {
                        // image is taller... keep width fix
                        targetWidth = Math.Min(1080, bmp.Width);
                        targetHeight = (int)((((float)targetWidth) * 16.0) / 9.0);
                    }
                    else
                    {
                        // image is wider... keep height fix
                        targetHeight = Math.Min(1920, bmp.Height);
                        targetWidth = (int)((((float)targetHeight) * 9.0) / 16.0);
                    }

                    GC.Collect();
                    System.Drawing.Bitmap bmp2 = Pic4Pic.ObjectModel.ImageHelper.CenterResize(bmp, targetWidth, targetHeight);

                    if (bmp2.Height > 640 && bmp2.Width > 360)
                    {
                        string f2 = targetPath + "\\" + String.Format("{0:000}.jpg", (count + 1));
                        bmp2.Save(f2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        notes.AppendLine(String.Format("{0:000}.jpg", (count + 1)) + "\t" + bmp2.Height + "\t" + bmp2.Width + "\t" + Path.GetFileName(file));
                        count++;
                    }
                }
            }

            Console.WriteLine(String.Format("{0}/{1}", count, total));
            File.WriteAllText(targetPath + "\\_notes.txt", notes.ToString());
        }

        private void CreateFakeUsers(String stateCode, String stateName, int areaId, Gender gender, long fbIdBase, int timeZoneOffset, string imagePath, String nicknameFilePath)
        {
            if (fbIdBase >= 0)
            {
                throw new ApplicationException("Do not use positive number for the FB ID");
            }

            int count = 0;
            int total = 0;

            DateTime timeUTC = DateTime.UtcNow;
            Random rand = new Random((int)(DateTime.Now.Ticks % Int32.MaxValue));

            string[] nicknames = File.ReadAllLines(nicknameFilePath);
            string[] cities = this.GetRandomCities(stateCode, areaId);            
            
            foreach (string file in System.IO.Directory.EnumerateFiles(imagePath, "*.jpg"))
            {
                int ageGroup = rand.Next(15, 65);
                if (ageGroup < 25)
                {
                    ageGroup = 1; // teenage
                }
                else if (ageGroup < 55)
                {
                    ageGroup = 2; // young & adult
                }
                else
                {
                    ageGroup = 3; // elder
                }

                string[] professions = null;
                EducationLevel[] educationLevels = null;

                if (gender == Gender.Female)
                {
                    professions = this.GetRandomProfessionsForWomen(ageGroup);
                    educationLevels = this.GetRandomEducationLevelsForWomen(ageGroup);
                }
                else
                {
                    professions = this.GetRandomProfessionsForMen(ageGroup);
                    educationLevels = this.GetRandomEducationLevelsForMen(ageGroup);
                }

                // create user in memory
                User user = new User();
                user.CreateTimeUTC = timeUTC;
                user.LastUpdateTimeUTC = timeUTC;
                user.Id = Guid.NewGuid();
                user.Username = nicknames[count].Trim().ToLower();

                int counter = 0;
                while (user.Username.Length < 6) 
                {
                    user.Username += (++counter).ToString();
                }

                user.Username += "__" + stateCode + "_" + areaId.ToString();

                user.Password = "Olc@y81";
                user.SplitId = 0;
                user.UserType = UserType.Guest; // means test
                user.UserStatus = UserStatus.Active;

                // create FB user in memory
                FacebookUser fbUser = new FacebookUser();

                fbUser.UserId = user.Id;
                fbUser.FacebookId = fbIdBase - count; // fbIdBase is negative
                fbUser.Username = user.Username;

                fbUser.CreateTimeUTC = timeUTC;
                fbUser.LastUpdateTimeUTC = timeUTC;

                fbUser.CurrentLocationId = 0;
                fbUser.CurrentLocationCity = cities[rand.Next(0, cities.Length - 1)];
                fbUser.CurrentLocationState = stateName;
                fbUser.HometownId = fbUser.CurrentLocationId;
                fbUser.HometownCity = fbUser.CurrentLocationCity;
                fbUser.HometownState = fbUser.CurrentLocationState;
                fbUser.TimeZoneOffset = timeZoneOffset;

                fbUser.FirstName = "Test";
                fbUser.LastName = "Test";
                fbUser.FullName = "Test Test";
                fbUser.Gender = gender;

                int birthYearMin = 1960;    // 55-years old
                int birthYearMax = 1995;    // 19-years old
                if (gender == Gender.Female) {
                    birthYearMin = 1970;    // 45-years old 
                }
                
                if (ageGroup == 1) 
                {
                    birthYearMin = 1992;    // not older than 22
                }
                else if (ageGroup == 3)
                {
                    birthYearMax = 1980;    // not younger than 35
                }
                else 
                {
                    birthYearMin = 1980;    // not older than 35
                    birthYearMax = 1992;    // not younger than 22
                }

                fbUser.BirthDay = new DateTime(rand.Next(birthYearMin, birthYearMax), rand.Next(1, 11), rand.Next(1, 28), 0, 0, 0);
                fbUser.EmailAddress = String.Format("test_{0:000000}_{1}@pic4pic.net", Math.Abs(fbUser.FacebookId), (gender == Gender.Male ? "m" : "w"));
                fbUser.FacebookUserName = String.Format("p4p_fb_test_{0:000000}_{1}", Math.Abs(fbUser.FacebookId), (gender == Gender.Male ? "m" : "w"));
                fbUser.FacebookLink = String.Format("http://facebook.com/p4p_test_{0:000000}_{1}", Math.Abs(fbUser.FacebookId), (gender == Gender.Male ? "m" : "w"));
                fbUser.ISOLocale = "en_US";
                fbUser.IsVerified = false;
                fbUser.EducationLevel = educationLevels[rand.Next(0, educationLevels.Length - 1)];
                fbUser.MaritalStatus = MaritalStatus.Single;
                fbUser.MaritalStatusAsText = "Single";
                if (DateTime.Now.Year - fbUser.BirthDay.Year >= 35) 
                {
                    if (rand.Next(0, 3) == 0)
                    {
                        fbUser.MaritalStatus = MaritalStatus.Married;
                        fbUser.MaritalStatusAsText = "Married";
                    }
                }

                fbUser.Profession = professions[rand.Next(0, professions.Length - 1)];

                // create user preferences
                UserPreferences pref = new UserPreferences();
                pref.UserId = user.Id;
                pref.LastUpdateTimeUTC = timeUTC;
                pref.InterestedIn = Gender.Male;
                if (gender == Gender.Male)
                {
                    pref.InterestedIn = Gender.Female;
                }

                // upload images
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(file);
                ImageFile[] imageMetaFiles = this.UploadImages(bmp);

                // save image meta files
                this.SaveMetaFilesOrFixCloud(imageMetaFiles);

                // prepare image IDs
                List<Guid> imageIDs = new List<Guid>();
                foreach (ImageFile imgFile in imageMetaFiles)
                {
                    imageIDs.Add(imgFile.Id);
                }

                // create database entries
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        Exception e = null;
                        try
                        {
                            this.CreateAllEntities(conn, trans, user, fbUser, null, null, pref, imageIDs);
                        }
                        catch (Exception ex)
                        {
                            e = ex;
                        }
                        finally
                        {
                            if (e == null)
                            {
                                trans.Commit();
                                count++;
                            }
                            else
                            {
                                trans.Rollback();
                                throw e;
                            }
                        }
                    }
                }

                total++;
            }

            Console.WriteLine(String.Format("{0}/{1}", count, total));
        }

        private void CreateAllEntities(SqlConnection conn, SqlTransaction trans, User user, FacebookUser fbUser, WorkHistory workHistory, EducationHistory eduHistory, UserPreferences pref, List<Guid> imageIds)
        {
            // save user
            user.CreateNewOnDBase(conn, trans);

            // save facebook user
            fbUser.CreateNewOnDBase(conn, trans);

            // save work history
            if (workHistory != null && workHistory.Count > 0)
            {
                workHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }

            // save education history
            if (eduHistory != null && eduHistory.Count > 0)
            {
                eduHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }

            // save User Preference
            pref.CreateNewOnDBase(conn, trans);

            // assign previously downloaded images to the user
            string concatenatedImageIds = String.Join(",", imageIds);
            if (ImageFile.UpdateAllImageOwnerships(conn, trans, user.Id, concatenatedImageIds) != imageIds.Count)
            {
                throw new Pic4PicArgumentException("Referenced photo doesn't exist", "PhotoUploadReference");
            }
        }

        private ImageFile[] UploadImages(Bitmap bmp)
        {
            using (MemoryStream clearImageData = new MemoryStream())
            {
                bmp.Save(clearImageData, System.Drawing.Imaging.ImageFormat.Bmp);

                // create image processer & uploader
                MultiImageUploader uploader = new MultiImageUploader(
                    clearImageData, this.GetBlobStorage(), 200, 200, 15);

                // process and upload images
                ImageFile[] imageMetaFiles = uploader.SafeUploadAllOrNone();

                // check result
                if (imageMetaFiles == null || imageMetaFiles.Length != 4)
                {
                    throw new Pic4PicException("Images couldn't be saved");
                }

                // fix ownership & flags
                for (int x = 0; x < imageMetaFiles.Length; x++)
                {
                    // set ownerships
                    imageMetaFiles[x].UserId = Guid.Empty;

                    // set flags
                    imageMetaFiles[x].IsProfilePicture = true;
                }

                return imageMetaFiles;
            }
        }

        private void SaveMetaFilesOrFixCloud(ImageFile[] imageMetaFiles)
        {
            // save all meta files or delete from cloud
            try
            {
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (ImageFile imf in imageMetaFiles)
                            {
                                imf.CreateOnDBase(conn, trans);
                            }

                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                foreach (ImageFile imf in imageMetaFiles)
                {
                    ImageFile.SafeDeleteFromCloud(this.GetBlobStorage(), imf.CloudUrl);
                }
                throw;
            }
        }

        private BlobStorageAccount GetBlobStorage()
        {
            BlobStorageAccount bsa = new BlobStorageAccount();
            bsa.ContainerName = "photos";
            bsa.UriTemplate = "http://{0}.blob.core.windows.net";
            
            bsa.AccountKey = "ChC10hlx4G7EWz4tVwDgZKjmx56zbyy9TGDkVmUWfgS0EW4lBXbPmsNkDfte44WfDm60cYBRagG89cqFHpI8mA==";
            bsa.AccountName = "ginger";
            //bsa.AccountKey = "3u5eL8jRgxu2mjmiVNH5D7JSP+zZqqtmjo15oo3miOY0KwKqffNhVFXTkNxFNiLaTl6tmSKSvDRs5Ilaeke5Ng==";
            //bsa.AccountName = "gingertest";
            
            return bsa;
        }

        private string GetDBConnectionString()
        {
            //return TestConstants.DBConnectionString;
            return TestConstants.DBConnectionStringProduction;
        }

        private string[] GetRandomCities(String stateCode, int subRegionId)
        {
            int usaCode = 1;
            stateCode = stateCode.Trim().ToLower();
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();

                List<Pic4Pic.ObjectModel.Region> regions = Pic4Pic.ObjectModel.Region.ReadAllFromDBase(conn, null, usaCode);
                Dictionary<string, Pic4Pic.ObjectModel.Region> regionsMap = Pic4Pic.ObjectModel.Region.MapItemsByNameAndCode(regions, true, true, true);
                if (!regionsMap.ContainsKey(stateCode))
                {
                    throw new ApplicationException("Unknown state: " + stateCode);
                }

                Pic4Pic.ObjectModel.Region state = regionsMap[stateCode];

                List<string> citiesMultipledByWeight = new List<string>();
                List<City> cities = City.ReadAllFromDBase(conn, null, usaCode, state.Id, subRegionId);
                foreach (City city in cities)
                {
                    for (int w = 0; w < city.WeightIndex; w++)
                    {
                        citiesMultipledByWeight.Add(city.Name);
                    }
                }

                return citiesMultipledByWeight.ToArray();
            }
        }

        private EducationLevel[] GetRandomEducationLevelsForWomen(int ageGroup)
        {
            if (ageGroup == 1) // teenage
            { 
                return new EducationLevel[] 
                    {
                        EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, 
                        EducationLevel.College
                    };
            }
            else 
            {
                return new EducationLevel[] 
                    {
                        EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, 
                        EducationLevel.College, EducationLevel.College, EducationLevel.College,
                        EducationLevel.Master
                    };
            }
        }

        private EducationLevel[] GetRandomEducationLevelsForMen(int ageGroup)
        {
            if (ageGroup == 1) // teenage
            { 
                return new EducationLevel[] 
                {
                    EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, 
                    EducationLevel.College, 
                    
                };
            }
            else 
            {
                return new EducationLevel[] 
                {
                    EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool, EducationLevel.HighSchool,
                    EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College, EducationLevel.College,
                    EducationLevel.Master, EducationLevel.Master,
                    EducationLevel.PhdOrAbove
                };
            }
        }

        private string[] GetRandomProfessionsForWomen(int ageGroup)
        {
            if (ageGroup == 1) // teenage
            { 
                return new string[] { 
                    null, null, null,
                    "Student", "Student", "Student", "Student", "Student", "Student", "Student", "Student",
                    "Waitress", "Waitress","Waitress",
                    "Cashier", "Cashier", "Cashier",
                    "Retail Salesperson",
                    "Office Clerk",
                 };
            }

            if (ageGroup == 3) // elder
            {  
                return new string[] { 
                    "Teacher", "Teacher", "Teacher",
                    "Retail Salesperson",
                    "Middle School Teacher",
                    "Receptionist",
                    "High School Teacher",
                    "Registered Nurse",
                    "Dental Hygienist",
                    "Physical Therapist",
                    "Pharmacist",
                    "HR Specialist",
                    "Account Manager",
                    "Sales Representative",
                    "Real Estate Agent",
                    "Occupational Therapist",
                    "Market Research Analyst",
                    "Dietitian",                
                };
            }

            return new string[] { 
                null, null, null,
                "Waitress", "Waitress","Waitress",
                "Cashier", "Cashier", "Cashier",
                "Teacher", "Teacher", "Teacher",
                "Nurse Practitioner",
                "Retail Salesperson",
                "Middle School Teacher",
                "Receptionist",
                "Administrative Assistant", "Administrative Assistant",
                "Event Planner",
                "High School Teacher",
                "Registered Nurse",
                "Dental Hygienist",
                "Physical Therapist",
                "Pharmacist",
                "Office Clerk",
                "HR Specialist",
                "Account Manager",
                "Sales Representative",
                "Preschool Teacher",
                "Real Estate Agent",
                "Graphic Designer",
                "Physician Assistant",
                "Occupational Therapist",
                "Market Research Analyst",
                "Dietitian",
                "Accounting Clerk",
            };
        }

        private string[] GetRandomProfessionsForMen(int ageGroup)
        {
            if (ageGroup == 1) // teenage
            {
                return new string[] { 
                    null, null, null,
                    "Student", "Student", "Student", "Student", "Student", "Student", "Student", "Student",
                    "Waiter", 
                    "Cashier", "Cashier", 
                    "Retail Salesperson",
                    "Office Clerk",
                 };
            }

            if (ageGroup == 3) // elder
            {
                return new string[] { 
                
                    "Software Engineer", "Software Engineer",
                    "Database Administrator",
                    "Mechanical Engineer", "Mechanical Engineer",
                    "Real Estate Agent", "Real Estate Agent", "Real Estate Agent",
                    "Physical Therapist",
                    "Teacher", "Teacher", "Teacher",
                    "Sales Representative", "Sales Representative", "Sales Representative",
                    "Driver", "Driver",
                    "Physician",
                    "Civil Engineer",
                    "Veterinarian",
                    "IT Manager",
                    "Sales Manager",
                    "Construction Manager",
                    "Construction Architect",
                    "Accountant", "Accountant", "Accountant", "Accountant", "Accountant",
                    "Lawyer","Lawyer",
                    "Financial Advisor",
                    "HR Specialist",
                    "Optician",
                    "PR Specialist",
                };
            }

            return new string[] { 
                null, null, null, null, null,
                "Software Developer", "Software Developer", "Software Developer", "Software Developer",
                "Web Developer", "Web Developer",
                "Computer Programmer",
                "Database Administrator", "Database Administrator",
                "System Administrator",
                "Mechanical Engineer", "Mechanical Engineer",
                "Real Estate Agent", "Real Estate Agent", "Real Estate Agent",
                "Physical Therapist",
                "Teacher", "Teacher",
                "Systems Analyst",
                "Sales Representative", "Sales Representative", "Sales Representative",
                "Paralegal", "Paralegal", "Paralegal",
                "Driver", "Driver",
                "Truck Driver", 
                "Massage Therapist", "Massage Therapist", 
                "Painter",
                "Groundskeeper",
                "Steelworker",
                "Dentist",
                "Pharmacist",
                "Physician",
                "Civil Engineer",
                "Veterinarian",
                "IT Manager",
                "Sales Manager",
                "Auto Mechanic","Auto Mechanic","Auto Mechanic",
                "Construction Manager",
                "Construction Architect",
                "Accountant", "Accountant",
                "Lawyer",
                "Financial Advisor",
                "Medical Assistant",
                "HR Specialist",
                "Optician",
                "PR Specialist",
                "Paramedic",
                "Office Clerk",
                "Customer Service Representative",
            };
        }

        private void DownloadImagesForFakeUsers(String path, long fbIdBase)
        {
            if (fbIdBase >= 0) 
            {
                throw new ApplicationException("Do not use positive number for the FB ID");
            }

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                for (int x = 0; x < 100; x++)
                {
                    long fbUserId = fbIdBase - x; // fbIdBase is negative
                    FacebookUser fbUser = FacebookUser.ReadFromDBase(conn, null, fbUserId);
                    if (fbUser != null)
                    {
                        List<ImageFile> imageFiles = ImageFile.ReadAllFromDBaseByUserId(conn, null, fbUser.UserId);
                        foreach (ImageFile image in imageFiles)
                        {
                            Bitmap bitmap = null;
                            try
                            {
                                bitmap = ImageHelper.DownloadImage(image.CloudUrl);
                            }
                            catch
                            { 
                            }

                            if (bitmap != null)
                            {
                                string filePath = path + "\\" + String.Format("{0:000}_{1}_{2}.jpg", (x + 1), (image.IsThumbnail ? 0 : 1), (image.IsBlurred ? 0 : 1));
                                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                }
            }        
        }        
    }
}
