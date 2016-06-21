using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("TypeName")]
	public class TypeExtension : IMarkupExtension<Type>
	{
		public string TypeName { get; set; }

		public Type ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");
			var typeResolver = serviceProvider.GetService(typeof (IXamlTypeResolver)) as IXamlTypeResolver;
			if (typeResolver == null)
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");

            //HACK: this fixes an extreme case in our app where we had our version of ItemsView in a ListView header, just taking this out for now and seeing what this will break
            if (string.IsNullOrEmpty(TypeName))
                return null;

			return typeResolver.Resolve(TypeName, serviceProvider);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<Type>).ProvideValue(serviceProvider);
		}
	}
}