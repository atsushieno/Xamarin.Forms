using System;
using System.Linq;
using Xamarin.Forms;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public static class FontExtensions
	{
		public static Xwt.Drawing.Font ToXwtFont (this Font font)
		{
			if (font.IsDefault)
				return Xwt.Drawing.Font.SystemFont;
			
			var faces = Xwt.Drawing.Font.GetAvailableFontFaces (font.FontFamily);
			if (!faces.Any ())
				faces = Xwt.Drawing.Font.GetAvailableFontFaces (Xwt.Drawing.Font.SystemFont.Family);
			var face = faces.First ();
			// FIXME: consider font.NamedSize etc.
			return face.Font.WithSize (font.FontSize)
				   .WithStyle ((font.FontAttributes & FontAttributes.Italic) != 0 ? Xwt.Drawing.FontStyle.Italic : Xwt.Drawing.FontStyle.Normal)
				   .WithWeight ((font.FontAttributes & FontAttributes.Bold) != 0 ? Xwt.Drawing.FontWeight.Bold : Xwt.Drawing.FontWeight.Normal);
		}

		public static Xwt.Drawing.FontFace ToFontFace (this Font font)
		{
			return Xwt.Drawing.Font.GetAvailableFontFaces (font.FontFamily).FirstOrDefault () ??
				Xwt.Drawing.Font.GetAvailableFontFaces (Xwt.Drawing.Font.SystemFont.Family).First ();
		}
	}
}
