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

		protected override void OnBoundsChanged ()
		{
			base.OnBoundsChanged ();
			var pageContainer = Parent as PageContainer;
			if (pageContainer != null && pageContainer.IsInFragment)
				return;
			PageController.SendAppearing ();
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
		{
			Page view = e.NewElement;
			base.OnElementChanged (e);

			UpdateBackgroundColor (view);
			UpdateBackgroundImage (view);

			Sensitive = true;
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