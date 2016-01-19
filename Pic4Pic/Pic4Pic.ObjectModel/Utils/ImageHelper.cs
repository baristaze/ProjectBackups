using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;

namespace Pic4Pic.ObjectModel
{
    public class ImageHelper
    {
        private class ImageMimeType
        {
            public ImageFormat Format { get; set; }
            public string MimeType { get; set; }

            public ImageMimeType(ImageFormat format, string mime)
            {
                this.Format = format;
                this.MimeType = mime;
            }
        }

        private static ImageMimeType[] imageFormats = new ImageMimeType[]
        {
                new ImageMimeType(ImageFormat.Bmp, "image/bmp"),
                new ImageMimeType(ImageFormat.Emf, "image/emf"),
                new ImageMimeType(ImageFormat.Exif, "image/exif"),
                new ImageMimeType(ImageFormat.Gif, "image/gif"),
                new ImageMimeType(ImageFormat.Icon, "image/icon"),
                new ImageMimeType(ImageFormat.Jpeg, "image/jpeg"),
                new ImageMimeType(ImageFormat.MemoryBmp, "image/memorybmp"),
                new ImageMimeType(ImageFormat.Png, "image/png"),
                new ImageMimeType(ImageFormat.Tiff, "image/tiff"),
                new ImageMimeType(ImageFormat.Wmf, "image/wmf")
        };

        public static Image Base64ToImage(string base64String, ref int contentLength)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream stream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Convert byte[] to Image
                stream.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(stream, true);
                contentLength = imageBytes.Length;
                return image;
            }
        }

        public static string GetContentType(Image image)
        {
            foreach (ImageMimeType imt in imageFormats)
            {
                if (imt.Format.Equals(image.RawFormat))
                {
                    return imt.MimeType;
                }
            }

            return "image/unknown";
        }

        public static Bitmap Pixelate(Bitmap image, int pixelateSize)
        {
            Rectangle pixelateArea = new Rectangle(0, 0, image.Width, image.Height);
            return Pixelate(image, pixelateArea, pixelateSize);
        }

        public static Bitmap Pixelate(Bitmap image, Rectangle rectangle, int pixelateSize)
        {
            // make an exact copy of the bitmap provided    
            Bitmap pixelated = new Bitmap(image.Width, image.Height);     
            using (Graphics graphics = Graphics.FromImage(pixelated))
            {
                Rectangle destRect = new Rectangle(0, 0, image.Width, image.Height);
                Rectangle srcRect = new Rectangle(0, 0, image.Width, image.Height);
                graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
            }
            
            // look at every pixel in the rectangle while making sure we're within the image bounds    
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += pixelateSize)    
            {        
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += pixelateSize)        
                {            
                    int offsetX = pixelateSize / 2;            
                    int offsetY = pixelateSize / 2;    
         
                    // make sure that the offset is within the boundry of the image            
                    while (xx + offsetX >= image.Width)
                    {
                        offsetX--;
                    }

                    while (yy + offsetY >= image.Height)
                    {
                        offsetY--;
                    }

                    // get the pixel color in the center of the soon to be pixelated area            
                    Color pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);
             
                    // for each pixel in the pixelate size, set it to the center color            
                    for (int x = xx; x < xx + pixelateSize && x < image.Width; x++)
                    {
                        for (int y = yy; y < yy + pixelateSize && y < image.Height; y++)
                        {
                            pixelated.SetPixel(x, y, pixel);
                        }
                    }
                }    
            }     
            
            return pixelated;
        }

        public static Bitmap FixedSize(Bitmap imgPhoto, int Width, int Height, bool needToFill)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (!needToFill)
            {
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }
            }
            else
            {
                if (nPercentH > nPercentW)
                {
                    nPercent = nPercentH;
                    destX = (int)Math.Round((Width - (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = (int)Math.Round((Height - (sourceHeight * nPercent)) / 2);
                }
            }

            if (nPercent > 1)
            {
                nPercent = 1;
            }

            int destWidth = (int)Math.Round(sourceWidth * nPercent);
            int destHeight = (int)Math.Round(sourceHeight * nPercent);

            System.Drawing.Bitmap bmPhoto = new Bitmap(
                destWidth <= Width ? destWidth : Width,
                destHeight < Height ? destHeight : Height,
                              PixelFormat.Format32bppRgb);

            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //                 imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static Bitmap CenterResize(Bitmap imgPhoto, int width, int height) 
        {
            int destX = (int)Math.Round((imgPhoto.Width - width) / 2.0);
            int destY = (int)Math.Round((imgPhoto.Height - height) / 2.0);
           
            System.Drawing.Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(0, 0, width, height),
                new Rectangle(destX, destY, width, height),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static Bitmap DownloadImage(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new Pic4PicArgumentException("The Image URL may not be empty", "ImageURL");
            }

            url = url.Trim();
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            if (response.ContentLength <= 0)
            {
                throw new Pic4PicArgumentException("The content pointed by URL is empty", "ImageURL");
            }

            if (response.ContentType == null || !response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new Pic4PicArgumentException("The content pointed by URL is not an image", "ImageURL");
            }

            Bitmap bitmap = (Bitmap)Bitmap.FromStream(response.GetResponseStream());
            return bitmap;
        }
    }
}
