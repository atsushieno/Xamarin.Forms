using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.XwtBackend
{
	public class XwtFormsApplication : Xwt.Window
	{
		static XwtFormsApplication ()
		{
			Xwt.Application.Initialize ();
			Xamarin.Forms.Forms.Init ();
		}

		Xamarin.Forms.Application application;
		Platform platform;

		public XwtFormsApplication ()
		{
			this.Closed += (o, e) => Xwt.Application.Exit ();
		}

		protected void LoadApplication (Xamarin.Forms.Application application)
		{
			if (application == null)
				throw new ArgumentNullException ("application");

			//## FIXME: copied from FormsActivity:
			//(application as IApplicationController)?.SetAppIndexingProvider (new AndroidAppIndexProvider (this));

			this.application = application;
			Xamarin.Forms.Application.Current = application;

			//## FIXME: copied from FormsActivity:
			//SetSoftInputMode ();

			application.PropertyChanged += AppOnPropertyChanged;

			SetMainPage ();

			//## FIXME: not sure if it should be called here. Internal Forms Appllication lifecycle requires this?
			application.SendStart ();
		}

		void AppOnPropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "MainPage")
				InternalSetPage (application.MainPage);
		}

		void InternalSetPage (Page page)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException ("Call Forms.Init () before this");

			if (platform != null) {
				platform.SetPage (page);
				return;
			}

			/* ## FIXME: copied from FormsApplicationActivity.cs
			MessagingCenter.Subscribe (this, Page.BusySetSignalName, (Page sender, bool enabled) => {
				busyCount = Math.Max (0, enabled ? busyCount + 1 : busyCount - 1);
				UpdateProgressBarVisibility (busyCount > 0);
			});

			UpdateProgressBarVisibility (busyCount > 0);
			*/

			MessagingCenter.Subscribe (this, Page.AlertSignalName, (Page sender, AlertArguments arguments) => {
				var buttons = new List<Xwt.Command> ();
				if (arguments.Accept != null)
					buttons.Add (new Xwt.Command (arguments.Accept));
				buttons.Add (new Xwt.Command (arguments.Cancel));
				var result = Xwt.MessageDialog.AskQuestion (arguments.Title, arguments.Message, buttons.ToArray ()).Label;
				arguments.SetResult (result == arguments.Accept);
			});

			/* ## FIXME: copied from FormsApplicationActivity.cs
			MessagingCenter.Subscribe (this, Page.ActionSheetSignalName, (Page sender, ActionSheetArguments arguments) => {
				var builder = new AlertDialog.Builder (this);
				builder.SetTitle (arguments.Title);
				string [] items = arguments.Buttons.ToArray ();
				builder.SetItems (items, (sender2, args) => { arguments.Result.TrySetResult (items [args.Which]); });

				if (arguments.Cancel != null)
					builder.SetPositiveButton (arguments.Cancel, delegate { arguments.Result.TrySetResult (arguments.Cancel); });

				if (arguments.Destruction != null)
					builder.SetNegativeButton (arguments.Destruction, delegate { arguments.Result.TrySetResult (arguments.Destruction); });

				AlertDialog dialog = builder.Create ();
				builder.Dispose ();
				//to match current functionality of renderer we set cancelable on outside
				//and return null
				dialog.SetCanceledOnTouchOutside (true);
				dialog.CancelEvent += (sender3, e) => { arguments.SetResult (null); };
				dialog.Show ();
			});
			*/

			platform = new Platform (this);
			if (application != null)
				application.Platform = platform;
			platform.SetPage (page);
			this.Content = platform.GetWidget ();

		}

		void SetMainPage ()
		{
			InternalSetPage (application.MainPage);
		}

		protected override void OnBoundsChanged (Xwt.BoundsChangedEventArgs a)
		{
			base.OnBoundsChanged (a);
			if (Application.Current == null || Xamarin.Forms.Application.Current.MainPage == null)
				return;
			var width = platform.Page.Width;
			var height = platform.Page.Height;
			Platform.LayoutRootPage (application.MainPage, (int) width, (int) height); // FIXME: use valid size
		}
	}
}
