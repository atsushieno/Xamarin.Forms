using System;
using System.ComponentModel;
using System.Diagnostics;
using Xwt;
using static System.String;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class ButtonRenderer : ViewRenderer<Button, Xwt.Button>
	{
		float _defaultFontSize;
		bool _drawableEnabled;
		bool _isDisposed;
		int _imageHeight = -1;

		public ButtonRenderer()
		{
			AutoPackage = false;
		}

		Xwt.Button NativeButton
		{
			get { return Control; }
		}

		public void OnViewAttachedToWindow(Xwt.Widget attachedView)
		{
			UpdateText();
		}

		public void OnViewDetachedFromWindow(Xwt.Widget detachedView)
		{
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			UpdateText();
			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				if (Control == null) {
					SetNativeControl (new Xwt.Button () { WidthRequest = 80, HeightRequest = 40 }, this);

					Debug.Assert (Control != null, "Control != null");

					Control.Clicked += delegate { throw new NotImplementedException (); };
				}

			}
			else
			{
				if (_drawableEnabled)
				{
					throw new NotImplementedException ();
				}
			}

			UpdateAll();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Button.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateEnabled();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateDrawable();
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
				UpdateBitmap();
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
				UpdateText();

			if (_drawableEnabled &&
				(e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName || e.PropertyName == Button.BorderRadiusProperty.PropertyName ||
				 e.PropertyName == Button.BorderWidthProperty.PropertyName))
			{
				throw new NotImplementedException ();
				Control.Surface.Reallocate ();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		protected override void UpdateBackgroundColor()
		{
			// Do nothing, the drawable handles this now
		}

		void UpdateAll()
		{
			UpdateFont();
			UpdateText();
			UpdateBitmap();
			UpdateTextColor();
			UpdateEnabled();
			UpdateDrawable();
		}

		void UpdateBitmap()
		{
			//throw new NotImplementedException ();
		}

		void UpdateDrawable()
		{
			//throw new NotImplementedException ();
		}

		void UpdateEnabled()
		{
			Control.Sensitive = Element.IsEnabled;
		}

		void UpdateFont()
		{
			Button button = Element;
			if (button.Font == Xamarin.Forms.Font.Default && _defaultFontSize == 0f)
				return;

			//throw new NotImplementedException ();
		}

		void UpdateText()
		{
			var oldText = NativeButton.Label;
			NativeButton.Label = Element.Text;

			// If we went from or to having no text, we need to update the image position
			if (IsNullOrEmpty(oldText) != IsNullOrEmpty(NativeButton.Label))
			{
				UpdateBitmap();
			}
		}

		void UpdateTextColor()
		{
			//throw new NotImplementedException ();
		}
	}
}