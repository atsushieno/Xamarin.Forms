using System;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.XwtBackend;
using Xwt;
using Xamarin.Forms.ControlGallery.XwtBinding;
using System.IO.IsolatedStorage;
using System.IO;
using System.ComponentModel;

[assembly: Dependency (typeof (CacheService))]
[assembly: Dependency (typeof (StringProvider))]
[assembly: ExportRenderer (typeof (DisposePage), typeof (DisposePageRenderer))]
[assembly: ExportRenderer (typeof (DisposeLabel), typeof (DisposeLabelRenderer))]
//[assembly: ExportRenderer (typeof (CustomButton), typeof (CustomButtonRenderer))]
[assembly: ExportEffect (typeof (BorderEffect), "BorderEffect")]

namespace Xamarin.Forms.ControlGallery.XwtBinding
{
	public class ControlGalleryApplication : XwtFormsApplication
	{
		public static void Main (string [] args)
		{
			new ControlGalleryApplication ().Run (args);
		}

		void Run (string [] args)
		{
			LoadApplication (new Xamarin.Forms.Controls.App ());
			Show ();
			Xwt.Application.Run ();
		}
	}

	public class DisposePageRenderer : PageRenderer
	{
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				((DisposePage)Element).SendRendererDisposed ();
			}
			base.Dispose (disposing);

		}
	}

	public class DisposeLabelRenderer : LabelRenderer
	{
		protected override void Dispose (bool disposing)
		{

			if (disposing) {
				((DisposeLabel)Element).SendRendererDisposed ();
			}
			base.Dispose (disposing);
		}
	}

	public class StringProvider : IStringProvider
	{
		public string CoreGalleryTitle {
			get {
				return "Xwt CoreGallery";
			}
		}
	}

	public class CacheService : ICacheService
	{
		public void ClearImageCache ()
		{
			DeleteFilesInDirectory ("ImageLoaderCache");
		}

		static void DeleteFilesInDirectory (string directory)
		{
			using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication ()) {
				if (isolatedStorage.DirectoryExists (directory)) {
					var files = isolatedStorage.GetFileNames (Path.Combine (directory, "*"));
					foreach (string file in files) {
						isolatedStorage.DeleteFile (Path.Combine (directory, file));
					}
				}
			}
		}
	}

	public class BorderEffect : PlatformEffect
	{
		protected override void OnAttached ()
		{
			throw new NotImplementedException ();
		}

		protected override void OnDetached ()
		{
		}

		protected override void OnElementPropertyChanged (PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged (args);
		}
	}

}
