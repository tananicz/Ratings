using System.Drawing;

namespace Ratings.Helpers
{
    public static class AppHelper
    {
        public static async Task<byte[]> GetImageBytes(IFormFile imageFile)
        {
            MemoryStream ms = new MemoryStream();

            if (System.OperatingSystem.IsWindows())
            {
                using (Image uploadedImg = Image.FromStream(imageFile.OpenReadStream()))
                {
                    int squareSize = Math.Min(uploadedImg.Width, uploadedImg.Height);
                    Bitmap croppedImg = new Bitmap(squareSize, squareSize);

                    Rectangle cropRect = new Rectangle();
                    cropRect.Width = uploadedImg.Width;
                    cropRect.Height = uploadedImg.Height;

                    if (uploadedImg.Width > uploadedImg.Height)
                    {
                        cropRect.X = ((uploadedImg.Width - squareSize) / 2) * -1;
                        cropRect.Y = 0;
                    }
                    else
                    {
                        cropRect.X = 0;
                        cropRect.Y = ((uploadedImg.Height - squareSize) / 2) * -1;
                    }

                    using (Graphics g = Graphics.FromImage(croppedImg))
                    {
                        g.DrawImage(uploadedImg, cropRect);  
                    }

                    Bitmap resizedImg = new Bitmap(150, 150);
                    using (Graphics g = Graphics.FromImage(resizedImg))
                    {
                        g.DrawImage(croppedImg, 0, 0, 150, 150);
                    }

                    resizedImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            else
            {
                await imageFile.OpenReadStream().CopyToAsync(ms);
            }

            return ms.ToArray();
        }
    }
}
