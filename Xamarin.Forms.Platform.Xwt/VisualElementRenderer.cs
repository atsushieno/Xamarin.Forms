using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xwt;

namespace Xamarin.Forms.Platform.XwtBackend
{
	[Flags]
	public enum VisualElementRendererFlags
	{
		Disposed = 1 << 0,
		AutoTrack = 1 << 1,
		AutoPackage = 1 << 2
	}

	public abstract class VisualElementRenderer<TElement> : Canvas, IVisualElementRenderer, IEffectControlProvider where TElement : VisualElement
	{
		readonly List<EventHandler<VisualElementChangedEventArgs>> _elementChangedHandlers = new List<EventHandler<VisualElementChangedEventArgs>> ();
		PropertyChangedEventHandler _propertyChangeHandler;
		VisualElementPackager _packager;

		VisualElement IVisualElementRenderer.Element => Element;

		public TElement Element { get; private set; }

		VisualElementRendererFlags _flags = VisualElementRendererFlags.AutoPackage | VisualElementRendererFlags.AutoTrack;

		protected bool AutoPackage {
			get { return (_flags & VisualElementRendererFlags.AutoPackage) != 0; }
			set {
				if (value)
					_flags |= VisualElementRendererFlags.AutoPackage;
				else
					_flags &= ~VisualElementRendererFlags.AutoPackage;
			}
		}

		protected bool AutoTrack {
			get { return (_flags & VisualElementRendererFlags.AutoTrack) != 0; }
			set {
				if (value)
					_flags |= VisualElementRendererFlags.AutoTrack;
				else
					_flags &= ~VisualElementRendererFlags.AutoTrack;
			}
		}

		/*
		public Canvas NativeView {
			get { return this; }
		}
		*/

		public VisualElementTracker Tracker { get; private set; }

		public Canvas Widget {
			get { return this; }
		}

		public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;


		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged {
			add { _elementChangedHandlers.Add (value); }
			remove { _elementChangedHandlers.Remove (value); }
		}

		public virtual SizeRequest GetDesiredSize (int widthConstraint, int heightConstraint)
		{
			double width = Math.Max (WidthRequest, widthConstraint);
			double height = Math.Max (HeightRequest, heightConstraint);

			return new SizeRequest (new Size (width, height), new Size (MinWidth, MinHeight));
		}

		public void RegisterEffect (Effect effect)
		{
			throw new NotImplementedException ();
		}

		void IVisualElementRenderer.SetElement (VisualElement element)
		{
			if (!(element is TElement))
				throw new ArgumentException ("element is not of type " + typeof (TElement), nameof (element));

			SetElement ((TElement)element);
		}

		public void SetElement (TElement element)
		{
			if (element == null)
				throw new ArgumentNullException (nameof (element));

			TElement oldElement = Element;
			Element = element;

			Performance.Start ();

			if (oldElement != null) {
				oldElement.PropertyChanged -= _propertyChangeHandler;
			}

			// element may be allowed to be passed as null in the future
			if (element != null) {
				Color currentColor = oldElement != null ? oldElement.BackgroundColor : Color.Default;
				if (element.BackgroundColor != currentColor)
					UpdateBackgroundColor ();
			}

			if (_propertyChangeHandler == null)
				_propertyChangeHandler = OnElementPropertyChanged;

			element.PropertyChanged += _propertyChangeHandler;

			if (oldElement == null) {
				//SetOnClickListener (this);
				//SetOnTouchListener (this);
				//SoundEffectsEnabled = false;
			}

			OnElementChanged (new ElementChangedEventArgs<TElement> (oldElement, element));

			if (AutoPackage && _packager == null)
				SetPackager (new VisualElementPackager (this));

			if (AutoTrack && Tracker == null)
				SetTracker (new VisualElementTracker (this));

			//if (element != null)
			//	SendVisualElementInitialized (element, this);

			var controller = (IElementController)oldElement;
			if (controller != null && controller.EffectControlProvider == this)
				controller.EffectControlProvider = null;

			controller = element;
			if (controller != null)
				controller.EffectControlProvider = this;

			if (element != null && !string.IsNullOrEmpty (element.AutomationId))
				SetAutomationId (element.AutomationId);

			Performance.Stop ();
		}

		protected virtual void OnElementChanged (ElementChangedEventArgs<TElement> e)
		{
			var args = new VisualElementChangedEventArgs (e.OldElement, e.NewElement);
			for (var i = 0; i < _elementChangedHandlers.Count; i++)
				_elementChangedHandlers [i] (this, args);

			EventHandler<ElementChangedEventArgs<TElement>> changed = ElementChanged;
			if (changed != null)
				changed (this, e);
		}

		protected virtual void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				SetBackgroundColor (Element.BackgroundColor);
			else if (e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName)
				UpdateClipToBounds ();
		}

		protected virtual void SetBackgroundColor (Color color)
		{
			BackgroundColor = color.ToXwtColor ();
		}

		protected virtual void SetOpacity (double opacity)
		{
			Opacity = opacity;
		}

		protected virtual void SetAutomationId (string id)
		{
			//throw new NotImplementedException ();
		}

		protected void SetPackager (VisualElementPackager packager)
		{
			_packager = packager;
			packager.Load ();
		}

		protected void SetTracker (VisualElementTracker tracker)
		{
			Tracker = tracker;
		}

		protected virtual void UpdateBackgroundColor ()
		{
			BackgroundColor = Element.BackgroundColor.ToXwtColor ();
		}

		void UpdateClipToBounds ()
		{
			throw new NotImplementedException ();
		}

		public void UpdateLayout ()
		{
			Performance.Start ();
			Tracker?.UpdateLayout ();
			Performance.Stop ();
		}

		protected override void OnReallocate ()
		{
			base.OnReallocate ();
			UpdateLayout ();
		}
	}
}

