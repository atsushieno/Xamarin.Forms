using System;
using Xwt;
using PointF = Xwt.Point;
using RectangleF = Xwt.Rectangle;
using SizeF = Xwt.Size;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public static class ColorExtensions
	{
		internal static readonly Xwt.Drawing.Color Black = Xwt.Drawing.Colors.Black;
		internal static readonly Xwt.Drawing.Color SeventyPercentGrey = new Xwt.Drawing.Color(0.7f, 0.7f, 0.7f, 1);

		public static Xwt.Drawing.Color ToXwtColor(this Color color)
		{
			return new Xwt.Drawing.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static Color ToColor(this Xwt.Drawing.Color color)
		{
			return new Color(color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Xwt.Drawing.Color ToUIColor(this Color color)
		{
			return new Xwt.Drawing.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}
	}

	public static class PointExtensions
	{
		public static Point ToPoint(this PointF point)
		{
			return new Point(point.X, point.Y);
		}
	}

	public static class SizeExtensions
	{
		public static SizeF ToSizeF(this Size size)
		{
			return new SizeF((float)size.Width, (float)size.Height);
		}
	}

	public static class RectangleExtensions
	{
		public static Rectangle ToRectangle(this RectangleF rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF ToRectangleF(this Rectangle rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}