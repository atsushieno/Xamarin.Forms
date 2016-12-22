using System;
using System.ComponentModel;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class PageRenderer : VisualElementRenderer<Page>
	{
		IPageController PageController => Element as IPageController;

		protected override void Dispose (bool disposing)
		{
			PageController?.SendDisappearing ();
			base.Dispose (disposing);
		}

		bool appearing_sent;

		protected override void OnBoundsChanged ()
		{
			base.OnBoundsChanged ();
			if (appearing_sent)
				return;
			appearing_sent = true;
			PageController.SendAppearing ();
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
		{
			Page view = e.NewElement;
			base.OnElementChanged (e);

			var originalSensitive = Sensitive;

			UpdateBackgroundColor (view);
			UpdateBackgroundImage (view);

			Sensitive = originalSensitive;
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
				UpdateBackgroundImage (Element);
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor (Element);
		}

		void UpdateBackgroundColor (Page view)
		{
			if (view.BackgroundColor != Color.Default)
				SetBackgroundColor (view.BackgroundColor);
		}

		void UpdateBackgroundImage (Page view)
		{
			if (!string.IsNullOrEmpty (view.BackgroundImage))
				throw new NotImplementedException ();// this.SetBackground (Context.Resources.GetDrawable (view.BackgroundImage));
		}
	}
}