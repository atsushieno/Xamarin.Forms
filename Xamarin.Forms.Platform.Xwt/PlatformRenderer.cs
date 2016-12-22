using System;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class PlatformRenderer : Canvas
	{
		public PlatformRenderer (Platform platform)
		{
			this.CanGetFocus = true;
		}
	}
}