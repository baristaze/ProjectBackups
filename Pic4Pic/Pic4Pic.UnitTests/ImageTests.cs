using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void TestMultiImageUploader()
        {
            for (int x = 0; x < 10; x++)
            {
                _TestMultiImageUploader(true);
            }
        }

        private void _TestMultiImageUploader(bool withCleanUp)
        {
            using (FileStream fs = new FileStream(@"C:\Users\Baris\Desktop\me.jpg", FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;

                    // 
                    BlobStorageAccount bsa = TestConstants.GetTestBlobStorageAccount();
                    
                    // create image processer & uploader
                    MultiImageUploader uploader = new MultiImageUploader(
                        ms, bsa, 200, 200, 20);

                    // process and upload images
                    ImageFile[] imageMetaFiles = null;
                    Exception ex = null;
                    try
                    {
                        imageMetaFiles = uploader.SafeUploadAllOrNone();
                        if (imageMetaFiles == null || imageMetaFiles.Length != 4)
                        {
                            throw new ApplicationException("Multiple Image Loading Test failed");
                        }
                    }
                    catch(Exception e)
                    {
                        ex = e;
                    }
                    finally
                    {
                        if (imageMetaFiles != null && withCleanUp)
                        {
                            foreach (ImageFile img in imageMetaFiles)
                            {
                                ImageFile.DeleteFromCloud(bsa, img.CloudUrl);
                            }
                        }

                        if (ex != null)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }
    }
}
