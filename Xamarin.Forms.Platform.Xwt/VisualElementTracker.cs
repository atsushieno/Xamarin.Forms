﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class VisualElementTracker : IDisposable
	{
		readonly EventHandler<EventArg<VisualElement>> _batchCommittedHandler;

		readonly PropertyChangedEventHandler _propertyChangedHandler;
		readonly EventHandler _sizeChangedEventHandler;
		bool _disposed;
		VisualElement _element;

		// Track these by hand because the calls down into iOS are too expensive
		bool _isInteractive;
		Rectangle _lastBounds;

		int _updateCount;

		public VisualElementTracker (IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException ("renderer");

			_propertyChangedHandler = HandlePropertyChanged;
			_sizeChangedEventHandler = HandleSizeChanged;
			_batchCommittedHandler = HandleRedrawNeeded;

			Renderer = renderer;
			renderer.ElementChanged += OnRendererElementChanged;
			SetElement (null, renderer.Element);
		}

		IVisualElementRenderer Renderer { get; set; }

		public void Dispose ()
		{
			Dispose (true);
		}

		public event EventHandler NativeControlUpdated;

		protected virtual void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing) {
				SetElement (_element, null);

				Renderer.ElementChanged -= OnRendererElementChanged;
				Renderer = null;
			}
		}

		void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName || e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName ||
				e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName || e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
				e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
				e.PropertyName == VisualElement.IsVisibleProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
				e.PropertyName == VisualElement.InputTransparentProperty.PropertyName || e.PropertyName == VisualElement.OpacityProperty.PropertyName)
				UpdateNativeControl (); // poorly optimized
		}

		void HandleRedrawNeeded (object sender, EventArgs e)
		{
			UpdateNativeControl ();
		}

		void HandleSizeChanged (object sender, EventArgs e)
		{
			UpdateNativeControl ();
		}

		void OnRendererElementChanged (object s, VisualElementChangedEventArgs e)
		{
			if (_element == e.NewElement)
				return;

			SetElement (_element, e.NewElement);
		}


		void SetElement (VisualElement oldElement, VisualElement newElement)
		{
			if (oldElement != null) {
				oldElement.PropertyChanged -= _propertyChangedHandler;
				oldElement.SizeChanged -= _sizeChangedEventHandler;
				oldElement.BatchCommitted -= _batchCommittedHandler;
			}

			_element = newElement;

			if (newElement != null) {
				newElement.BatchCommitted += _batchCommittedHandler;
				newElement.SizeChanged += _sizeChangedEventHandler;
				newElement.PropertyChanged += _propertyChangedHandler;

				UpdateNativeControl ();
			}
		}

		void UpdateNativeControl ()
		{
			if (_disposed)
				return;

			// FIXME: implement

			if (NativeControlUpdated != null)
				NativeControlUpdated (this, EventArgs.Empty);
		}
	}
}