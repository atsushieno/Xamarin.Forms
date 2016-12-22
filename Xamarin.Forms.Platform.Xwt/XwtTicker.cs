using System;
using Xamarin.Forms.Internals;
using System.Windows.Threading;

namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class XwtTicker : Ticker
	{
		readonly DispatcherTimer _timer;

		public XwtTicker()
		{
			_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(15) };
			_timer.Tick += (sender, args) => SendSignals();
		}

		protected override void DisableTimer()
		{
			_timer.Stop();
		}

		protected override void EnableTimer()
		{
			_timer.Start();
		}
	}
}