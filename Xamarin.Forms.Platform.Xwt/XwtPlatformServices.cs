using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using System.Windows.Threading;
using System.Net.Http;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;

namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class XwtPlatformServices : IPlatformServices
	{
		static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider ();

		public void BeginInvokeOnMainThread(Action action)
		{
			Xwt.Application.Invoke (action);
		}

		public Ticker CreateTicker()
		{
			return new XwtTicker ();
		}

		public virtual Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		public string GetMD5Hash(string input)
		{
			byte [] bytes = Checksum.ComputeHash (Encoding.UTF8.GetBytes (input));
			var ret = new char [32];
			for (var i = 0; i < 16; i++) {
				ret [i * 2] = (char)Hex (bytes [i] >> 4);
				ret [i * 2 + 1] = (char)Hex (bytes [i] & 0xf);
			}
			return new string (ret);
		}

		static int Hex (int v)
		{
			if (v < 10)
				return '0' + v;
			return 'a' + v - 10;
		}

		public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
		{
			switch (size) {
			case NamedSize.Default:
				if (typeof (Button).IsAssignableFrom (targetElementType))
					return 14;
				if (typeof (Label).IsAssignableFrom (targetElementType))
					return 18;
				if (typeof (Editor).IsAssignableFrom (targetElementType) || typeof (Entry).IsAssignableFrom (targetElementType) || typeof (SearchBar).IsAssignableFrom (targetElementType))
					return 12;
				return 14;
			case NamedSize.Micro:
				return 10;
			case NamedSize.Small:
				return 12;
			case NamedSize.Medium:
				return 14;
			case NamedSize.Large:
				return 18;
			default:
				throw new ArgumentOutOfRangeException ("size");
			}
		}

		public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage streamResponse = await client.GetAsync(uri.AbsoluteUri).ConfigureAwait(false);

				if (!streamResponse.IsSuccessStatusCode)
				{
					Log.Warning("HTTP Request", $"Could not retrieve {uri}, status code {streamResponse.StatusCode}");
					return null;
				}

				return await streamResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
			}
		}

		public IIsolatedStorageFile GetUserStoreForApplication()
		{
			return new XwtIsolatedStorageFile (IsolatedStorageFile.GetUserStoreForAssembly ()); // GetUserForApplication() throws on desktop...
		}

		public class XwtIsolatedStorageFile : IIsolatedStorageFile
		{
			readonly IsolatedStorageFile _isolatedStorageFile;

			public XwtIsolatedStorageFile (IsolatedStorageFile isolatedStorageFile)
			{
				_isolatedStorageFile = isolatedStorageFile;
			}

			public Task CreateDirectoryAsync (string path)
			{
				_isolatedStorageFile.CreateDirectory (path);
				return Task.FromResult (true);
			}

			public Task<bool> GetDirectoryExistsAsync (string path)
			{
				return Task.FromResult (_isolatedStorageFile.DirectoryExists (path));
			}

			public Task<bool> GetFileExistsAsync (string path)
			{
				return Task.FromResult (_isolatedStorageFile.FileExists (path));
			}

			public Task<DateTimeOffset> GetLastWriteTimeAsync (string path)
			{
				return Task.FromResult (_isolatedStorageFile.GetLastWriteTime (path));
			}

			public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access)
			{
				Stream stream = _isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access);
				return Task.FromResult (stream);
			}

			public Task<Stream> OpenFileAsync (string path, FileMode mode, FileAccess access, FileShare share)
			{
				Stream stream = _isolatedStorageFile.OpenFile (path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share);
				return Task.FromResult (stream);
			}
		}

		// FIXME: implement
		public bool IsInvokeRequired => true;

		public void OpenUriAction(Uri uri)
		{
			throw new NotImplementedException ();
		}

		public void StartTimer(TimeSpan interval, Func<bool> callback)
		{
			var timer = new DispatcherTimer { Interval = interval };
			timer.Start();
			timer.Tick += (sender, args) =>
			{
				bool result = callback();
				if (!result)
					timer.Stop();
			};
		}

		internal class XwtTimer : ITimer
		{
			readonly Timer _timer;

			public XwtTimer(Timer timer)
			{
				_timer = timer;
			}

			public void Change(int dueTime, int period)
			{
				_timer.Change(dueTime, period);
			}

			public void Change(long dueTime, long period)
			{
				Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
			}

			public void Change(TimeSpan dueTime, TimeSpan period)
			{
				_timer.Change(dueTime, period);
			}

			public void Change(uint dueTime, uint period)
			{
				Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(period));
			}
		}
	}
}