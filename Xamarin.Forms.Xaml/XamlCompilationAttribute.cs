using System;
using System.Reflection;

namespace Xamarin.Forms.Xaml
{
	[Flags]
	public enum XamlCompilationOptions
	{
		Skip = 1 << 0,
		Compile = 1 << 1
	}

	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, Inherited = false)]
	public sealed class XamlCompilationAttribute : Attribute
	{
		public XamlCompilationAttribute(XamlCompilationOptions xamlCompilationOptions)
		{
			XamlCompilationOptions = xamlCompilationOptions;
		}

		public XamlCompilationAttribute(XamlCompilationOptions xamlCompilationOptions, string filter)
		{
			XamlCompilationOptions = xamlCompilationOptions;
			Filter = filter;
		}

		public XamlCompilationOptions XamlCompilationOptions { get; set; }

		/// <summary>
		/// Filter to run a regex on assembly-wide XamlCompilationAttribute
		/// </summary>
		public string Filter { get; set; }
	}

	internal static class XamlCExtensions
	{
		public static bool IsCompiled(this Type type)
		{
			var attr = type.GetTypeInfo().GetCustomAttribute<XamlCompilationAttribute>();
			if (attr != null)
				return attr.XamlCompilationOptions == XamlCompilationOptions.Compile;
			attr = type.GetTypeInfo().Module.GetCustomAttribute<XamlCompilationAttribute>();
			if (attr != null)
				return attr.XamlCompilationOptions == XamlCompilationOptions.Compile;
			attr = type.GetTypeInfo().Assembly.GetCustomAttribute<XamlCompilationAttribute>();
			if (attr != null)
				return attr.XamlCompilationOptions == XamlCompilationOptions.Compile;

			return false;
		}
	}
}