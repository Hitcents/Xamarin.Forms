using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	internal interface IPlatformServices
	{
		bool IsInvokeRequired { get; }

		void BeginInvokeOnMainThread(Action action);

		Ticker CreateTicker();

		Assembly[] GetAssemblies();

		string GetMD5Hash(string input);

		double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes);

		Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken);

		IIsolatedStorageFile GetUserStoreForApplication();

		void OpenUriAction(string uri);

		void StartTimer(TimeSpan interval, Func<bool> callback);
	}
}