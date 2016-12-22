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
		Xwt.VBox layout = new Xwt.VBox ();

		public XwtFormsApplication ()
		{
			this.Closed += (o, e) => Xwt.Application.Exit ();
		}

		protected void LoadApplication (Xamarin.Forms.Application application)
		{
			if (application == null)
				throw new ArgumentNullException ("application");

			this.application = application;
			Xamarin.Forms.Application.Current = application;

			application.PropertyChanged += AppOnPropertyChanged;

			SetMainPage ();
		}

		void AppOnPropertyChanged (object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "MainPage")
				InternalSetPage (application.MainPage);
		}

		void InternalSetPage (Page page)
		{
			if (!Forms.IsInitialized)
				throw new InvalidOperationException ("Call Forms.Init (Activity, Bundle) before this");

			if (platform != null) {
				platform.SetPage (page);
				return;
			}

			MessagingCenter.Subscribe (this, Page.AlertSignalName, (Page sender, AlertArguments arguments) => {
				var buttons = new List<Xwt.Command> ();
				if (arguments.Accept != null)
					buttons.Add (new Xwt.Command (arguments.Accept));
				buttons.Add (new Xwt.Command (arguments.Cancel));
				var result = Xwt.MessageDialog.AskQuestion (arguments.Title, arguments.Message, buttons.ToArray ()).Label;
				arguments.SetResult (result == arguments.Accept);
			});

			/*
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
			layout.PackStart (platform.GetWidget ());
			this.Content = layout;
		}

		void SetMainPage ()
		{
			InternalSetPage (application.MainPage);
		}
	}
}
