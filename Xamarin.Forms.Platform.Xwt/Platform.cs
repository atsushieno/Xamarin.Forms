using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RectangleF = Xwt.Rectangle;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class Platform : BindableObject, IPlatform, INavigation, IDisposable
	{
		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer),
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var view = bindable as VisualElement;
				if (view != null)
					view.IsPlatformEnabled = newvalue != null;
			});

		readonly int _alertPadding = 10;

		readonly List<Page> _modals;
		readonly PlatformRenderer _renderer;
		bool _animateModals = true;
		bool _disposed;

		internal Platform(XwtFormsApplication application)
		{
			XwtApplication = application;

			// ## FIXME: copied from Android Platform.cs
			//_defaultActionBarTitleTextColor = SetDefaultActionBarTitleTextColor ();

			_renderer = new PlatformRenderer (this);
			_modals = new List<Page> ();

			/* ## FIXME: copied from Android Platform.cs
			//FormsApplicationActivity.BackPressed += HandleBackPressed;

			_toolbarTracker.CollectionChanged += ToolbarTrackerOnCollectionChanged;
			*/
		}

		internal XwtFormsApplication XwtApplication { get; private set; }

		internal Page Page { get; set; }

		void IDisposable.Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;

			Page.DescendantRemoved -= HandleChildRemoved;
			MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
			MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
			MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

			DisposeModelAndChildrenRenderers(Page);
			foreach (var modal in _modals)
				DisposeModelAndChildrenRenderers(modal);

			_renderer.Dispose();
		}

		internal Xwt.Widget GetWidget ()
		{
			return _renderer;
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException("InsertPageBefore is not supported globally on iOS, please use a NavigationPage.");
		}

		IReadOnlyList<Page> INavigation.ModalStack
		{
			get { return _modals; }
		}

		IReadOnlyList<Page> INavigation.NavigationStack
		{
			get { return new List<Page>(); }
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		async Task<Page> INavigation.PopModalAsync(bool animated)
		{
			throw new NotImplementedException ();
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException("PopToRootAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on iOS, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page modal)
		{
			return ((INavigation)this).PushModalAsync(modal, true);
		}

		Task INavigation.PushModalAsync(Page modal, bool animated)
		{
			_modals.Add(modal);
			modal.Platform = this;

			modal.DescendantRemoved += HandleChildRemoved;

			return PresentModal(modal, _animateModals && animated);
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on iOS, please use a NavigationPage.");
		}

		/*
		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			var renderView = GetRenderer(view);
			if (renderView == null || renderView.Widget == null)
				return new SizeRequest(Size.Zero);

			return renderView.GetDesiredSize((int) widthConstraint, (int) heightConstraint);
		}
		*/

		SizeRequest IPlatform.GetNativeSize (VisualElement view, double widthConstraint, double heightConstraint)
		{
			Performance.Start ();

			var viewRenderer = GetRenderer (view);

			// negative numbers have special meanings to xwt they don't to us
			widthConstraint = widthConstraint <= -1 ? double.PositiveInfinity : widthConstraint;
			heightConstraint = heightConstraint <= -1 ? double.PositiveInfinity : heightConstraint;

			int width = !double.IsPositiveInfinity (widthConstraint) ? (int) widthConstraint : 0;
			//? MeasureSpecFactory.MakeMeasureSpec ((int)widthConstraint, MeasureSpecMode.AtMost)
			//: MeasureSpecFactory.MakeMeasureSpec (0, MeasureSpecMode.Unspecified);

			int height = !double.IsPositiveInfinity (heightConstraint) ? (int) heightConstraint : 0;
			//? MeasureSpecFactory.MakeMeasureSpec ((int)heightConstraint, MeasureSpecMode.AtMost)
			//: MeasureSpecFactory.MakeMeasureSpec (0, MeasureSpecMode.Unspecified);

			SizeRequest rawResult = viewRenderer.GetDesiredSize (width, height);
			if (rawResult.Minimum == Size.Zero)
				rawResult.Minimum = rawResult.Request;
			var result = new SizeRequest (new Size (rawResult.Request.Width, rawResult.Request.Height),
				new Size (rawResult.Minimum.Width, rawResult.Minimum.Height));

			Performance.Stop ();
			return result;
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer> (element.GetType ()) ?? new DefaultRenderer ();
			renderer.SetElement (element);
			return renderer;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement bindable)
		{
			return (IVisualElementRenderer)bindable.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement bindable, IVisualElementRenderer value)
		{
			bindable.SetValue(RendererProperty, value);
		}

		protected override void OnBindingContextChanged()
		{
			SetInheritedBindingContext(Page, BindingContext);

			base.OnBindingContextChanged();
		}

		internal void DidAppear()
		{
			_animateModals = false;
			Application.Current.NavigationProxy.Inner = this;
			_animateModals = true;
		}

		internal void DisposeModelAndChildrenRenderers(Element view)
		{
			IVisualElementRenderer renderer;
			foreach (VisualElement child in view.Descendants())
			{
				renderer = GetRenderer(child);
				child.ClearValue(RendererProperty);

				if (renderer != null)
				{
					throw new NotImplementedException ();
					renderer.Dispose();
				}
			}

			renderer = GetRenderer((VisualElement)view);
			if (renderer != null)
			{
				throw new NotImplementedException ();
				renderer.Dispose();
			}
			view.ClearValue(RendererProperty);
		}

		internal void DisposeRendererAndChildren(IVisualElementRenderer rendererToRemove)
		{
			if (rendererToRemove == null)
				return;

			if (rendererToRemove.Element != null && GetRenderer(rendererToRemove.Element) == rendererToRemove)
				rendererToRemove.Element.ClearValue(RendererProperty);

			throw new NotImplementedException ();
			rendererToRemove.Dispose();
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;
			if (Page != null)
				throw new NotImplementedException();
			Page = newRoot;

			Page.Platform = this;
			AddChild(Page);

			Page.DescendantRemoved += HandleChildRemoved;

			Application.Current.NavigationProxy.Inner = this;
		}

		void AddChild(VisualElement view)
		{
			if (GetRenderer(view) == null)
			{
				var viewRenderer = CreateRenderer(view);
				SetRenderer(view, viewRenderer);

				_renderer.AddChild (viewRenderer.Widget);
			}
			else
				Console.Error.WriteLine("Potential view double add");
		}

		void HandleChildRemoved(object sender, ElementEventArgs e)
		{
			var view = e.Element;
			DisposeModelAndChildrenRenderers(view);
		}

		bool PageIsChildOfPlatform(Page page)
		{
			while (!Application.IsApplicationOrNull(page.RealParent))
				page = (Page)page.RealParent;

			return Page == page || _modals.Contains(page);
		}

		async Task PresentModal(Page modal, bool animated)
		{
			var modalRenderer = GetRenderer(modal);
			if (modalRenderer == null)
			{
				modalRenderer = CreateRenderer(modal);
				SetRenderer(modal, modalRenderer);
			}

			throw new NotImplementedException ();

			// One might wonder why these delays are here... well thats a great question. It turns out iOS will claim the 
			// presentation is complete before it really is. It does not however inform you when it is really done (and thus 
			// would be safe to dismiss the VC). Fortunately this is almost never an issue
			throw new NotImplementedException ();
			await Task.Delay(5);
		}

		internal static void LayoutRootPage (Page page, double width, double height)
		{
			int statusBarHeight = 0; // ## FIXME: dummy

			if (page is MasterDetailPage)
				page.Layout (new Rectangle (0, 0, width, height));
			else {
				page.Layout (new Rectangle (0, statusBarHeight, width, height - statusBarHeight));
			}
		}

		internal class DefaultRenderer : VisualElementRenderer<VisualElement>
		{
		}
	}
}