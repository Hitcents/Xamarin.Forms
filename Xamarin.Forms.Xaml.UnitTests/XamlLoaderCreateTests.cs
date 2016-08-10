using NUnit.Framework;
using System;
using System.Windows.Input;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class XamlLoaderCreateTests
	{
#pragma warning disable 0618 //retaining legacy call to obsolete code
		[Test]
		public void CreateFromXaml ()
		{
			var xaml = @"
				<ContentView xmlns=""http://xamarin.com/schemas/2014/forms""
							 xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
						 	 x:Class=""Xamarin.Forms.Xaml.UnitTests.FOO"">
					<Label Text=""Foo""  x:Name=""label""/>
				</ContentView>";

			var view = XamlLoader.Create (xaml);
			Assert.That (view, Is.TypeOf<ContentView> ());
			Assert.AreEqual ("Foo", ((Label)((ContentView)view).Content).Text);
		}

		[Test]
		public void CreateFromXamlDoesntFailOnMissingEventHandler ()
		{
			var xaml = @"
				<Button xmlns=""http://xamarin.com/schemas/2014/forms""
						xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
						Clicked=""handleClick"">
				</Button>";

			Button button = null;
			Assert.DoesNotThrow (() => button = XamlLoader.Create (xaml, true) as Button);
			Assert.NotNull (button);
		}
#pragma warning restore 0618

		[Test]
		public void NestedMarkupExtensionInsideDataTemplate()
		{
			var listView = new ListView();
			string xaml = @"
				<ListView xmlns=""http://xamarin.com/schemas/2014/forms"" xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"" xmlns:toe=""clr-namespace:Xamarin.Forms.Xaml.UnitTests;assembly=Xamarin.Forms.Xaml.UnitTests"">
					<ListView.ItemTemplate>
						<DataTemplate>
							<ViewCell>
								<Button Command=""{toe:Navigate Operation=Forward, Type={x:Type Grid}}"" />
							</ViewCell>
						</DataTemplate>
					</ListView.ItemTemplate>
				</ListView>";
			XamlLoader.Load(listView, xaml);
			listView.ItemsSource = new string[2];
			
			var cell = (ViewCell)listView.TemplatedItems[0];
			var button = (Button)cell.View;
			Assert.IsNotNull(button.Command);

			cell = (ViewCell)listView.TemplatedItems[1];
			button = (Button)cell.View;
			Assert.IsNotNull(button.Command);
		}
	}

	public enum NavigationOperation
	{
		Forward,
		Back,
		Replace,
	}

	[ContentProperty(nameof(Operation))]
	public class NavigateExtension : IMarkupExtension<ICommand>
	{
		public NavigationOperation Operation { get; set; }

		public Type Type { get; set; }

		public ICommand ProvideValue(IServiceProvider serviceProvider)
		{
			return new Command(() => { });
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return ProvideValue(serviceProvider);
		}
	}
}