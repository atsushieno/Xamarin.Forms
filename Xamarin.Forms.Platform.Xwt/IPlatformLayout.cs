namespace Xamarin.Forms.Platform.XwtBackend
{
	internal interface IPlatformLayout
	{
		void OnLayout(bool changed, int l, int t, int r, int b);
	}
}