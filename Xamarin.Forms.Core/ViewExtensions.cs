using System;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public static class ViewExtensions
	{
		public static void CancelAnimations(VisualElement view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			view.AbortAnimation("LayoutTo");
			view.AbortAnimation("TranslateTo");
			view.AbortAnimation("RotateTo");
			view.AbortAnimation("RotateYTo");
			view.AbortAnimation("RotateXTo");
			view.AbortAnimation("ScaleTo");
			view.AbortAnimation("FadeTo");
			view.AbortAnimation("SizeTo");
		}

		public static Task<bool> FadeTo(this VisualElement view, double opacity, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			view.Animate("FadeTo", (v, x) => v.Opacity = x, view.Opacity, opacity, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> LayoutTo(this VisualElement view, Rectangle bounds, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			Rectangle start = view.Bounds;
			Func<double, Rectangle> computeBounds = progress =>
			{
				double x = start.X + (bounds.X - start.X) * progress;
				double y = start.Y + (bounds.Y - start.Y) * progress;
				double w = start.Width + (bounds.Width - start.Width) * progress;
				double h = start.Height + (bounds.Height - start.Height) * progress;

				return new Rectangle(x, y, w, h);
			};
			view.Animate("LayoutTo", (v, f) => v.Layout(computeBounds(f)), 0, 1, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> RelRotateTo(this VisualElement view, double drotation, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			return view.RotateTo(view.Rotation + drotation, length, easing);
		}

		public static Task<bool> RelScaleTo(this VisualElement view, double dscale, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			return view.ScaleTo(view.Scale + dscale, length, easing);
		}

		public static Task<bool> RotateTo(this VisualElement view, double rotation, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			view.Animate("RotateTo", (v, f) => v.Rotation = f, view.Rotation, rotation, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> RotateXTo(this VisualElement view, double rotation, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			view.Animate("RotateXTo", (v, f) => v.RotationX = f, view.Rotation, rotation, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> RotateYTo(this VisualElement view, double rotation, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			view.Animate("RotateYTo", (v, f) => v.RotationY = f, view.Rotation, rotation, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> ScaleTo(this VisualElement view, double scale, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (easing == null)
				easing = Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			view.Animate("ScaleTo", (v, f) => v.Scale = f, view.Scale, scale, length: length, easing: easing, finished: (v, f, a) => tcs.SetResult(a));
			return tcs.Task;
		}

		public static Task<bool> TranslateTo(this VisualElement view, double x, double y, uint length = 250, Easing easing = null)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			easing = easing ?? Easing.Linear;

			var tcs = new TaskCompletionSource<bool>();
			Action<IAnimatable, double> translateX = (v, f) => ((VisualElement)v).TranslationX = f;
			Action<IAnimatable, double> translateY = (v, f) => ((VisualElement)v).TranslationY = f;
			new Animation { { 0, 1, new Animation(translateX, view.TranslationX, x) }, { 0, 1, new Animation(translateY, view.TranslationY, y) } }.Commit(view, "TranslateTo", 16, length, easing,
				(v, f, a) => tcs.SetResult(a));

			return tcs.Task;
		}
	}
}