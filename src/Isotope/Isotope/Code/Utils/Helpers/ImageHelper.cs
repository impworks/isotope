using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Isotope.Data.Models;

namespace Isotope.Code.Utils.Helpers
{
    /// <summary>
    /// Helper utilities for image processing.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Known sizes for media thumbnails.
        /// </summary>
        public static IReadOnlyDictionary<MediaSize, ImagePreset> ImagePresets = new Dictionary<MediaSize, ImagePreset>
        {
            [MediaSize.Small] = new ImagePreset
            {
                MediaSize = MediaSize.Small,
                Size = new Size(200, 200),
                ResizeFunc = ResizeToFill,
                Codec = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg"),
                CodecArgs = new EncoderParameters { Param = new [] { new EncoderParameter(Encoder.Quality, 80L) } }
            },
            [MediaSize.Large] = new ImagePreset
            {
                MediaSize = MediaSize.Large,
                Size = new Size(1024, 768),
                ResizeFunc = ResizeToFit,
                Codec = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg"),
                CodecArgs = new EncoderParameters { Param = new[] { new EncoderParameter(Encoder.Quality, 90L) } }
            }
        };

        /// <summary>
        /// Generates a proportional thumbnail.
        /// </summary>
        public static Image ResizeToFit(Image source, Size maxSize)
        {
            return GetPortion(source, new Rectangle(Point.Empty, source.Size), GetInscribeSize(source.Size, maxSize));
        }

        /// <summary>
        /// Generates a filling thumbnail.
        /// </summary>
        public static Image ResizeToFill(Image source, Size maxSize)
        {
            return GetPortion(source, GetFillRectangle(source.Size, maxSize), maxSize);
        }
        
        /// <summary>
        /// Generates a proportional thumbnail.
        /// </summary>
        public static Image GetPortion(Image source, Rect rect, Size size)
        {
            return GetPortion(
                source,
                new Rectangle(
                    (int) (rect.X * source.Width),
                    (int) (rect.Y * source.Height),
                    (int) (rect.Width * source.Width),
                    (int) (rect.Height * source.Height)
                ),
                size
            );
        }

        /// <summary>
        /// Cuts and resizes an image portion.
        /// </summary>
        public static Image GetPortion(Image source, Rectangle srcRect, Size size)
        {
            var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            using var gfx = Graphics.FromImage(bmp);

            var destRect = new Rectangle(Point.Empty, size);
            
            gfx.Clear(Color.Transparent);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(
                source,
                destRect,
                srcRect,
                GraphicsUnit.Pixel
            );

            return bmp;
        }

        /// <summary>
        /// Calculates the rectangle into which the image thumbnail will be inscribed.
        /// </summary>
        public static Size GetInscribeSize(Size size, Size maxSize)
        {
            // do not upscale small images
            if (size.Width < maxSize.Width && size.Height < maxSize.Height)
                return size;

            var xRatio = (double) maxSize.Width / size.Width;
            var yRatio = (double) maxSize.Height / size.Height;
            var ratio = Math.Min(yRatio, xRatio);
            return new Size((int) (size.Width * ratio), (int) (size.Height * ratio));
        }

        /// <summary>
        /// Returns the rectangle that best fills the given size (from center).
        /// </summary>
        public static Rectangle GetFillRectangle(Size size, Size maxSize)
        {
            var xRatio = (double)maxSize.Width / size.Width;
            var yRatio = (double)maxSize.Height / size.Height;
            var ratio = Math.Max(yRatio, xRatio);
            var newSize = new Size((int)(maxSize.Width / ratio), (int)(maxSize.Height / ratio));
            var pos = new Point((size.Width - newSize.Width) / 2, (size.Height - newSize.Height) / 2);
            return new Rectangle(pos, newSize);
        }

        /// <summary>
        /// Saves thumbnails for an image.
        /// </summary>
        public static void CreateThumbnails(Image originalImage, string originalPath)
        {
            foreach (var preset in ImagePresets.Values)
            {
                var path = MediaHelper.GetSizedMediaPath(originalPath, preset.MediaSize);
                using var image = preset.ResizeFunc(originalImage, preset.Size);
                image.Save(path, preset.Codec, preset.CodecArgs);
            }
        }

        /// <summary>
        /// Rotates the image if it has an EXIF orientation tag.
        /// </summary>
        public static void ApplyExifRotation(Image img)
        {
            const int EXIF_ORIENTATION = 274;
            
            if (!img.PropertyIdList.Contains(EXIF_ORIENTATION))
                return;

            var prop = img.GetPropertyItem(EXIF_ORIENTATION);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }

        /// <summary>
        /// Returns the small thumbnail rect in 0..1 coordinate space.
        /// </summary>
        public static Rect GetDefaultThumbnailRect(Size size)
        {
            var targetSize = ImagePresets[MediaSize.Small].Size;
            var rect = GetFillRectangle(size, targetSize);
            var w = 1.0 / size.Width;
            var h = 1.0 / size.Height;
            return new Rect
            {
                X = rect.X * w,
                Y = rect.Y * h,
                Width = rect.Width * w,
                Height = rect.Height * h,
            };
        }
    }

    public class ImagePreset
    {
        public MediaSize MediaSize { get; set; }
        public Size Size { get; set; }
        public Func<Image, Size, Image> ResizeFunc { get; set; }
        public ImageCodecInfo Codec { get; set; }
        public EncoderParameters CodecArgs { get; set; }
    }
}
