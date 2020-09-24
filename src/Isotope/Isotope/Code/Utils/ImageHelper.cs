﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Helper utilities for image processing.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Known sizes for media thumbnails.
        /// </summary>
        public static IReadOnlyDictionary<MediaSize, Size> Sizes = new Dictionary<MediaSize, Size>
        {
            [MediaSize.Small] = new Size(200, 200),
            [MediaSize.Large] = new Size(1024, 768),
        };

        /// <summary>
        /// Generates a proportional thumbnail.
        /// </summary>
        public static Image ResizeToFit(Image source, Size maxSize)
        {
            var propSize = GetInscribeSize(source.Size, maxSize);
            var srcRect = new Rectangle(Point.Empty, source.Size);
            var destRect = new Rectangle(Point.Empty, propSize);

            var bmp = new Bitmap(propSize.Width, propSize.Height, PixelFormat.Format32bppArgb);
            using var gfx = Graphics.FromImage(bmp);

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
        /// Generates a filling thumbnail.
        /// </summary>
        public static Image ResizeToFill(Image source, Size maxSize)
        {
            var srcRect = GetFillRectangle(source.Size, maxSize);
            var destRect = new Rectangle(Point.Empty, maxSize);

            var bmp = new Bitmap(maxSize.Width, maxSize.Height, PixelFormat.Format32bppArgb);
            using var gfx = Graphics.FromImage(bmp);

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
        private static Size GetInscribeSize(Size size, Size maxSize)
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
        private static Rectangle GetFillRectangle(Size size, Size maxSize)
        {
            var xRatio = (double)maxSize.Width / size.Width;
            var yRatio = (double)maxSize.Height / size.Height;
            var ratio = Math.Max(yRatio, xRatio);
            var newSize = new Size((int)(maxSize.Width / ratio), (int)(maxSize.Height / ratio));
            var pos = new Point((size.Width - newSize.Width) / 2, (size.Height - newSize.Height) / 2);
            return new Rectangle(pos, newSize);
        }
    }
}