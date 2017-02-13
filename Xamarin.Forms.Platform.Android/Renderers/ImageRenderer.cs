using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;

namespace Xamarin.Forms.Platform.Android
{
	public class ImageRenderer : ViewRenderer<Image, AImageView>
	{
		Bitmap _bitmap;
		ImageSource _oldSource;
		bool _isDisposed;

		IElementController ElementController => Element as IElementController;

		public ImageRenderer()
		{
			AutoPackage = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			RecycleBitmap();
			_oldSource = null;
			_isDisposed = true;

			base.Dispose(disposing);
		}

		protected override AImageView CreateNativeControl()
		{
			return new FormsImageView(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var view = CreateNativeControl();
				SetNativeControl(view);
			}

			UpdateBitmap(e.OldElement?.Source);
			UpdateAspect();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Image.SourceProperty.PropertyName)
				UpdateBitmap(_oldSource);
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();
		}

		void RecycleBitmap()
		{
			if (!_isDisposed)
			{
				var imageView = Control;
				if (imageView != null)
					imageView.SetImageBitmap(null);
			}

			if (_bitmap != null)
			{
				_bitmap.Recycle();
				_bitmap.Dispose();
				_bitmap = null;
			}
		}

		void UpdateAspect()
		{
			AImageView.ScaleType type = Element.Aspect.ToScaleType();
			Control.SetScaleType(type);
		}

		async void UpdateBitmap(ImageSource oldSource)
		{
			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			ImageSource source = Element.Source;
			IImageSourceHandler handler;

			if (oldSource != null && Equals(oldSource, source))
				return;
			if (oldSource is FileImageSource && source is FileImageSource && ((FileImageSource)oldSource).File == ((FileImageSource)source).File)
				return;

			var oldBitmap = _bitmap;
			_bitmap = null;

			((IImageController)Element).SetIsLoading(true);

			var formsImageView = Control as FormsImageView;
			if (formsImageView != null)
				formsImageView.SkipInvalidate();

			Control.SetImageResource(global::Android.Resource.Color.Transparent);

			Bitmap bitmap = null;
			if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			{
				try
				{
					bitmap = await handler.LoadImageAsync(source, Context);
				}
				catch (TaskCanceledException)
				{
				}
				catch (IOException ex)
				{
					Log.Warning("Xamarin.Forms.Platform.Android.ImageRenderer", "Error updating bitmap: {0}", ex);
				}
			}

			//NOTE: if we get here and the source changed before the image handler could load the image
			if (Element == null || !Equals(Element.Source, source))
			{
				if (bitmap != null)
				{
					bitmap.Recycle();
					bitmap.Dispose();
				}
				return;
			}

			if (oldBitmap != null)
			{
				oldBitmap.Recycle();
				oldBitmap.Dispose();
			}

			if (!_isDisposed)
			{
				Control.SetImageBitmap(_bitmap = bitmap);
				_oldSource = bitmap == null ? null : source;

				((IImageController)Element).SetIsLoading(false);
				((IVisualElementController)Element).NativeSizeChanged();
			}
			else
			{
				RecycleBitmap();
				_oldSource = null;
			}
		}
	}
}