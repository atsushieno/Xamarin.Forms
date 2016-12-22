using System;
using System.ComponentModel;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class LabelRenderer : ViewRenderer<Xamarin.Forms.Label, Xwt.Label>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);

			// FIXME: remove sizespec
			if (this.Control == null)
				SetNativeControl (new Xwt.Label () { WidthRequest = 80, HeightRequest = 40 }, this);
			var view = this.Control;

			if (e.OldElement == null) {
				UpdateText ();
				UpdateLineBreakMode ();
				UpdateGravity ();
			} else {
				UpdateText ();
				if (e.OldElement.LineBreakMode != e.NewElement.LineBreakMode)
					UpdateLineBreakMode ();
				if (e.OldElement.HorizontalTextAlignment != e.NewElement.HorizontalTextAlignment || e.OldElement.VerticalTextAlignment != e.NewElement.VerticalTextAlignment)
					UpdateGravity ();
			}
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName || e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
				UpdateGravity ();
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateText ();
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				UpdateText ();
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
				UpdateLineBreakMode ();
			else if (e.PropertyName == Label.TextProperty.PropertyName || e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText ();
		}

		void UpdateColor ()
		{
			Color c = Element.TextColor;

			if (c.IsDefault)
				Control.TextColor = c.ToXwtColor ();
		}

		void UpdateFont ()
		{
#pragma warning disable 618 // We will need to update this when .Font goes away
			Font f = Element.Font;
#pragma warning restore 618

			var font = f.ToXwtFont ();
			Control.Font = font;
		}

		void UpdateGravity ()
		{
			Label label = Element;

			Control.ExpandHorizontal = label.HorizontalOptions.Expands;
			Control.HorizontalPlacement = label.HorizontalOptions.Alignment.ToWidgetPlacement ();
			Control.ExpandVertical = label.VerticalOptions.Expands;
			Control.VerticalPlacement = label.VerticalOptions.Alignment.ToWidgetPlacement ();
		}

		void UpdateLineBreakMode ()
		{
			switch (Element.LineBreakMode) {
			case LineBreakMode.NoWrap:
				Control.Wrap = WrapMode.None;
				Control.Ellipsize = EllipsizeMode.None;
				break;
			case LineBreakMode.WordWrap:
				Control.Ellipsize = EllipsizeMode.None;
				Control.Wrap = WrapMode.Word;
				break;
			case LineBreakMode.CharacterWrap:
				Control.Ellipsize = EllipsizeMode.None;
				Control.Wrap = WrapMode.Character;
				break;
			case LineBreakMode.HeadTruncation:
				Control.Wrap = WrapMode.None;
				Control.Ellipsize = EllipsizeMode.Start;
				break;
			case LineBreakMode.TailTruncation:
				Control.Wrap = WrapMode.None;
				Control.Ellipsize = EllipsizeMode.End;
				break;
			case LineBreakMode.MiddleTruncation:
				Control.Wrap = WrapMode.None;
				Control.Ellipsize = EllipsizeMode.Middle;
				break;
			}
		}

		void UpdateText ()
		{
			if (Element.FormattedText != null) {
				FormattedString formattedText = Element.FormattedText ?? Element.Text;
				// FIXME: implement
				throw new NotImplementedException ();
			} else {
				Control.Text = Element.Text;
				UpdateColor ();
				UpdateFont ();
			}
		}
	}
}
