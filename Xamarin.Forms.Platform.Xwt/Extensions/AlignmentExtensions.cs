using System;
using Xamarin.Forms;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public static class AlignmentExtensions
	{
		public static Xwt.WidgetPlacement ToWidgetPlacement (this LayoutAlignment a)
		{
			switch (a) {
			case LayoutAlignment.Center: return WidgetPlacement.Center;
			case LayoutAlignment.Start: return WidgetPlacement.Start;
			case LayoutAlignment.End: return WidgetPlacement.End;
			case LayoutAlignment.Fill: return WidgetPlacement.Fill;
			}
			return default (WidgetPlacement);
		}
	}
}
