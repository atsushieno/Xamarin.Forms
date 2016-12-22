using System;
using System.ComponentModel;
using Xwt;
using RectangleF = Xwt.Rectangle;
using SizeF = Xwt.Size;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public abstract class ViewRenderer : ViewRenderer<View, Widget>
	{
	}

	public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView> where TView : View where TNativeView : Widget
	{
		Xwt.Drawing.Color _defaultColor;
		Canvas _container;

		public TNativeView Control { get; private set; }

		/// <summary>
		/// Determines whether the native control is disposed of when this renderer is disposed
		/// Can be overridden in deriving classes 
		/// </summary>
		protected virtual bool ManageNativeControlLifetime => true;

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (disposing && Control != null && ManageNativeControlLifetime) {
				Control.Dispose ();
				Control = null;
			}
		}

		public override SizeRequest GetDesiredSize (int widthConstraint, int heightConstraint)
		{
			if (Control == null)
				return (base.GetDesiredSize (widthConstraint, heightConstraint));

			var view = _container == this ? (Widget) Control : _container;
			double width = Math.Max (view.WidthRequest, widthConstraint);
			double height = Math.Max (view.HeightRequest, heightConstraint);

			return new SizeRequest (new Size (width, height), new Size (view.MinWidth, view.MinHeight));
		}

		protected override void OnElementChanged (ElementChangedEventArgs<TView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null)
				e.OldElement.FocusChangeRequested -= ViewOnFocusChangeRequested;

			if (e.NewElement != null) {
				if (Control != null && e.OldElement != null && e.OldElement.BackgroundColor != e.NewElement.BackgroundColor || e.NewElement.BackgroundColor != Color.Default)
					SetBackgroundColor (e.NewElement.BackgroundColor);

				e.NewElement.FocusChangeRequested += ViewOnFocusChangeRequested;
			}

			UpdateIsEnabled ();
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (Control != null) {
				if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
					UpdateIsEnabled ();
				else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
					SetBackgroundColor (Element.BackgroundColor);
			}

			base.OnElementPropertyChanged (sender, e);
		}

		protected override void SetBackgroundColor (Color color)
		{
			if (Control == null)
				return;

			if (color == Color.Default)
				Control.BackgroundColor = _defaultColor;
			else
				Control.BackgroundColor = color.ToUIColor ();
		}

		protected void SetNativeControl (TNativeView widget, Canvas container)
		{
			_defaultColor = widget.BackgroundColor;
			Control = widget;
			_container = container;

			//if (Element.BackgroundColor != Color.Default)
			//	SetBackgroundColor (Element.BackgroundColor);

			// FIXME: should be better way to do this...
			this.WidthRequest = widget.WidthRequest;
			this.HeightRequest = widget.HeightRequest;
			this.ExpandHorizontal = widget.ExpandHorizontal;
			this.ExpandVertical = widget.ExpandVertical;
			this.Opacity = widget.Opacity;

			AddChild (Control);

			UpdateIsEnabled ();

			//throw new NotImplementedException ();
		}

		void UpdateIsEnabled ()
		{
			if (Element == null || Control == null)
				return;

			var uiControl = Control as Widget;
			if (uiControl == null)
				return;
			uiControl.Sensitive = Element.IsEnabled;
		}

		void ViewOnFocusChangeRequested (object sender, VisualElement.FocusRequestArgs focusRequestArgs)
		{
			if (Control == null)
				return;

			if (focusRequestArgs.Focus)
				Control.SetFocus ();
			focusRequestArgs.Result = focusRequestArgs.Focus;
		}
	}
}