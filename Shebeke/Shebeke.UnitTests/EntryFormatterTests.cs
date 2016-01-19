using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Shebeke.ObjectModel;
using Shebeke.Web.Services;

namespace Shebeke.UnitTests
{
    [TestClass]
    public class EntryFormatterTests
    {
        [TestMethod]
        public void TestEntryFormatterV1()
        {
            string text = "This is a [photo-1] and [photo-55] \n this is another \n [PHOTO-543534532] however this is not a [photo-0]\n";
            text += "There is also a link here http://zaman.com.tr/archive/Getir.aspx?id=343&name=dsfadh;sad%2fhasan&!</>foobar ayrica \n";
            text += "One more link google.com ayrica \n";
            text += "bol miktarda da html tags < > & ' \" \" \" \n";
            text += "this is **bold** however this is * * not bold** \n";
            text += "this is //italic and still \n italic till here // however\n this is / / not italic// \n";
            text += "this is ||upper|| however this is | | not upper|| \n";

            EntryFormatter formatter = new EntryFormatter("");
            List<long> ids = formatter.DetectImageIds(text);
            Console.WriteLine(ids.Count);

            List<ImageFile> imageList = new List<ImageFile>();

            ImageFile image = new ImageFile();
            image.Id = 543534532;
            image.CloudUrl = "http://shebeke.net/images/bk5.jpg";
            imageList.Add(image);

            ImageFile image2 = new ImageFile();
            image2.Id = 55;
            image2.CloudUrl = "http://shebeke.net/images/bkg.png";
            imageList.Add(image2);                       

            string html = formatter.GetEncodedHtml(text, imageList);
            Console.Write(html);
        }
    }
}
