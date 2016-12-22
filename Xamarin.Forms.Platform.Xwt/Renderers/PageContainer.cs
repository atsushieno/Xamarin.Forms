using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class PageContainer : Canvas
	{
		public PageContainer(IVisualElementRenderer child)
		{
			AddChild (child.Widget);
			Child = child;
		}

		public IVisualElementRenderer Child { get; set; }

		protected override void OnBoundsChanged ()
		{
			Child.UpdateLayout();
		}

		protected override void OnChildPreferredSizeChanged ()
		{
			base.OnChildPreferredSizeChanged ();
		}
	}
}