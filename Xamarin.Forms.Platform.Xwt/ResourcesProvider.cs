
namespace Xamarin.Forms.Platform.XwtBackend
{
	internal class ResourcesProvider : ISystemResourcesProvider
	{
		ResourceDictionary _dictionary;

		public IResourceDictionary GetSystemResources()
		{
			_dictionary = new ResourceDictionary();

			UpdateStyles();

			return _dictionary;
		}

		public Style GetStyle(int style)
		{
			var result = new Style(typeof(Label));

			return result;
		}

		void UpdateStyles()
		{
		}
	}
}