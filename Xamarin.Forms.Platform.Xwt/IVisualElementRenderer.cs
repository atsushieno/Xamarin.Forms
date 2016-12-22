using System;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		VisualElement Element { get; }

		VisualElementTracker Tracker { get; }

		Canvas Widget { get; }

		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		SizeRequest GetDesiredSize (int widthConstraint, int heightConstraint);

		void SetElement (VisualElement element);
		void UpdateLayout ();

		double WidthRequest { get; set; }
		double HeightRequest { get; set; }
	}
}