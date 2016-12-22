using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class PageContainer : Canvas
	{
		public PageContainer(IVisualElementRenderer child, bool inFragment = false)
		{
			AddChild (child.Widget);
			Child = child;
			IsInFragment = inFragment;
		}

		public IVisualElementRenderer Child { get; set; }

		public bool IsInFragment { get; set; }

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